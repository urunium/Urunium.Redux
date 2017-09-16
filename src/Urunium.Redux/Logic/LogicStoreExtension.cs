using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urunium.Redux.Enhance;

namespace Urunium.Redux.Logic
{
    /// <summary>
    /// Extension class to configure Logic in store.
    /// </summary>
    public static class LogicStoreExtension
    {
        /// <summary>
        /// Enhance your store to handle business logics.
        /// </summary>
        /// <typeparam name="TState">Application's state.</typeparam>
        /// <param name="originalStore">Original store that will be enhanced with ability to handle business logic.</param>
        /// <param name="configurator">Instance of <see cref="ILogicConfiguration{TState}"/>.</param>
        /// <returns>Enhanced store, that now can handle business logics.</returns>
        public static IStore<TState> ConfigureLogic<TState>(this IStore<TState> originalStore, Action<ILogicConfiguration<TState>> configurator)
        {
            LogicEnhancer<TState> finalStore = originalStore.FindEnhancer<LogicEnhancer<TState>, TState>() ?? new LogicEnhancer<TState>(originalStore);
            configurator.Invoke(finalStore);
            return finalStore;
        }

        /// <summary>
        /// Enhancer to enable business logic processing when actions are dispatched into store.
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        private class LogicEnhancer<TState> : Enhance.StoreEnhancer<TState>, ILogicConfiguration<TState>
        {
            List<object> _logics = new List<object>();

            public LogicEnhancer(IStore<TState> store)
                : base(store)
            {
            }

            /// <summary>
            /// Add a business logic that will listen to particular action beign dispatched to store.
            /// </summary>
            /// <typeparam name="TAction">
            /// Type of action that business logic is interested in processing. use <see cref="AnyAction"/> if business logic needs
            /// to process all the action being dispatched.
            /// </typeparam>
            /// <param name="logics">
            /// Array of logics; processing a particular action type. Multiple calls are needed to handle different action types.
            /// </param>
            public void AddLogics<TAction>(params ILogic<TState, TAction>[] logics)
            {
                _logics.AddRange(logics);
            }

            /// <summary>
            /// Customize what happens when an action is dispatched to store.
            /// </summary>
            /// <typeparam name="TAction"></typeparam>
            /// <param name="action"></param>
            /// <param name="forward"></param>
            protected override void OnDispatch<TAction>(TAction action, Action<TAction> forward)
            {
                // Kill all processors if poision pill is received.
                if(action is PoisonPill)
                {
                    var allProcessors = (from x in _logics.OfType<ICancelable>() select x as ICancelable);
                    foreach(var processor in allProcessors)
                    {
                        processor.Cancel(action);
                    }
                    // @thoughts:
                    // Poision pill will never be actually forwarded to reducers.
                    // There should be no processor listening to poision pill.
                    return;
                }

                // Kill all processors which has registered current action as their CancelType.
                var cancelables = (from x in _logics.OfType<ICancelable>() where x.CancelType == typeof(TAction) select x as ICancelable);
                if (cancelables.Any())
                {
                    foreach (var cancelable in cancelables)
                    {
                        cancelable.Cancel(action);
                    }
                    // @thoughts:
                    // Even cancelation action type of one processor can be processed by another processor.
                    // So not returning.
                }


                List<object> logicChain = new List<object>();
                // Create a chain of logics registered for this action type, or `AnyAction` type. The logics are ordered in
                // descending order of priority, i.e. logic with highest priority assigned will run first. In case of same
                // priority, order of logic will depend on algorithm used by OrderByDescending method.
                logicChain.AddRange(
                    (from x in _logics.OfType<ILogic<TState, TAction>>() select x as IPriority)
                    .Concat((from x in _logics.OfType<ILogic<TState, AnyAction>>() select x as IPriority))
                    .OrderByDescending(x => x.Priority)
                );

                // Executor function. Since logics are aync operations, but OnDispatch is `void` method. Wanted to avoid
                // making it async void method, also not wanting to take dependency in any third party library yet.
                var executor = new Func<Task>(async () =>
                {
                    try
                    {
                        // Transformations and validations
                        bool continueProcessing = await ExecutePreProcessors(action, logicChain);

                        if (continueProcessing)
                        {
                            // dispatch
                            forward(action);

                            // Processing
                            await ExecuteProcessMethod(action, logicChain);
                        }
                    }
                    catch(Exception e)
                    {
                        // Any exception that happens while processing this particular action will be dispatched. Another
                        // business logic may want to handle it. But may be there is a chance of never ending loop :D can't be too sure.
                        Store.Dispatch(e);
                    }
                });

                executor.Invoke();
            }

            /// <summary>
            /// Execute pre-processors.
            /// </summary>
            /// <typeparam name="TAction"></typeparam>
            /// <param name="action"></param>
            /// <param name="logicChain"></param>
            /// <returns></returns>
            private async Task<bool> ExecutePreProcessors<TAction>(TAction action, List<object> logicChain)
            {
                bool continueProcessing = true;
                PreProcessResult result = null;
                foreach (var logic in logicChain)
                {
                    switch (logic)
                    {
                        case ILogic<TState, AnyAction> anyActionLogic:
                            result = await anyActionLogic.PreProcess(Store, new AnyAction(action));
                            break;
                        case ILogic<TState, TAction> actionLogic:
                            result = await actionLogic.PreProcess(Store, action);
                            break;
                    }

                    continueProcessing = result?.ContinueToNextStep == true;
                    if (!continueProcessing)
                    {
                        // Break if pre-processing (validation) fails.
                        break;
                    }
                }

                return continueProcessing;
            }

            /// <summary>
            /// Execute business logic process.
            /// </summary>
            /// <typeparam name="TAction"></typeparam>
            /// <param name="action"></param>
            /// <param name="logicChain"></param>
            /// <returns></returns>
            private async Task ExecuteProcessMethod<TAction>(TAction action, List<object> logicChain)
            {
                foreach (var logic in logicChain)
                {
                    switch (logic)
                    {
                        case ILogic<TState, AnyAction> anyActionLogic:
                            if (anyActionLogic.IsLongRunning)
                            {
                                ProcessLongRunningLogic((dispatcher) => anyActionLogic.Process(() => Store.State, new AnyAction(action), dispatcher));
                            }
                            else
                            {
                                using (var dispatcher = MultiDispatcher.Create(Store))
                                {
                                    await (anyActionLogic.Process(() => Store.State, new AnyAction(action), dispatcher) ?? Task.WhenAll());
                                }
                            }
                            break;
                        case ILogic<TState, TAction> actionLogic:
                            if (actionLogic.IsLongRunning)
                            {
                                ProcessLongRunningLogic((dispatcher) => actionLogic.Process(() => Store.State, action, dispatcher));
                            }
                            else
                            {
                                using (var dispatcher = MultiDispatcher.Create(Store))
                                {
                                    await (actionLogic.Process(() => Store.State, action, dispatcher) ?? Task.WhenAll());
                                }
                            }
                            break;
                    }
                }
            }

            private void ProcessLongRunningLogic(Func<IMultiDispatcher, Task> longRunningProcess)
            {
                // Run long running process in it's own thread.
                Task.Factory.StartNew(async () =>
                {
                    using (var dispatcher = MultiDispatcher.Create(Store))
                    {
                        await longRunningProcess(dispatcher);
                    }
                }, TaskCreationOptions.LongRunning).Unwrap();
            }
        }
    }
}
