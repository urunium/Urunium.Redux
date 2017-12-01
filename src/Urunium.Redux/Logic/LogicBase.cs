using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Urunium.Redux.Logic
{
    /// <summary>
    /// Abstract base class for logics. Implementing vanilla <see cref="ILogic{TState, TAction}"/> works but
    /// leaves more to be desired. LogicBase splits Preprocess into Transformation and Validation.
    /// Gives ability to replace any of Preprocess/Processing steps with custom implementation.
    /// </summary>
    /// <typeparam name="TState">Type of application's state.</typeparam>
    /// <typeparam name="TAction">Type of action supported by this Logic. Use <see cref="AnyAction"/> if logic needs to
    /// support "any action"
    /// </typeparam>
    public abstract class LogicBase<TState, TAction> : ILogic<TState, TAction>
    {
        /// <summary>
        /// Type of action that when dispatched will cancel this process
        /// </summary>
        public abstract Type CancelType { get; }

        /// <summary>
        /// Priority of logic. Priority is calculated in descending order. i.e. Greater the number higher the priority
        /// </summary>
        public abstract uint Priority { get; }

        /// <summary>
        /// Indicate that this is a long running process, so other logics must continue while this is running in background 
        /// (i.e. don't wait for it to complete). Long running processes are given separate thread, through use of: 
        /// <see cref="TaskCreationOptions.LongRunning"/>. Implementor doesn't need to concern with running it in separate thread. 
        /// Note:
        /// - <see cref="ILogic{TState, TAction}.PreProcess(IStore{TState}, TAction)"/> cannot be long running.
        /// - Only <see cref="ILogic{TState, TAction}.Process(Func{TState}, TAction, IMultiDispatcher)"/> can be long running.
        /// </summary>
        public virtual bool IsLongRunning { get; } = false;

        private bool _killed = false;

        /// <summary>
        /// Cancel a process
        /// </summary>
        /// <typeparam name="TCancel">
        /// Type of cancel action. Will always be object of type registered through <see cref="CancelType"/>.
        /// </typeparam>
        /// <param name="cancelAction">
        /// Action used to cancel this process.
        /// </param>
        public virtual void Cancel<TCancel>(TCancel cancelAction)
        {
            CancellationTokenSource.Cancel();

            if ((cancelAction as object) is PoisonPill poisionPill)
            {
                _killed = poisionPill.Kill;
            }
        }

        /// <summary>
        /// Cancelation token source, that can be used to request cancelation.
        /// </summary>
        protected CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();

        /// <summary>
        /// First step of <see cref="ILogic{TState, TAction}.PreProcess(IStore{TState}, TAction)"/>. 
        /// Transform actions before your reducers are run, so that reducer gets augumented action.
        /// It is async operation, meaning transformation logic may for example communicate with server
        /// to decide.
        /// </summary>
        /// <param name="getState"></param>
        /// <param name="action"></param>
        /// <param name="cancellationToken">Token to request cancelation.</param>
        /// <returns></returns>
        protected virtual Task<TAction> OnTransfrom(Func<TState> getState, TAction action, CancellationToken cancellationToken)
        {
            return Task.FromResult(action);
        }

        /// <summary>
        /// Validation occurs after Transformation, but before action are received by reducers. Validation decides whether or not
        /// reducers will receive this action at all. Validation decissions can be made based on current state and action that is
        /// being dispatched. It is an async operation, hence is flexible for validation to occur remotely on server, or for scenarios
        /// where validation may depend on some remote entities etc.
        /// </summary>
        /// <param name="getState">Function to get the current state</param>
        /// <param name="action">Action being dispatched</param>
        /// <param name="dispatcher">Multi-Dispatcher</param>
        /// <param name="cancellationToken">Token to request cancelation</param>
        /// <returns>Asynchronously returns <see cref="ValidationResult"/></returns>
        protected virtual Task<ValidationResult> OnValidate(Func<TState> getState, TAction action, IMultiDispatcher dispatcher, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return Task.FromResult(ValidationResult.Failure(new ValidationException("Cancel requested")));
            }
            return Task.FromResult(ValidationResult.Success());
        }

        /// <summary>
        /// Hook to modify logic to decide what happens when validation error occurs. Default implementation is to dispatch the error
        /// and let reducers/UI to decide.
        /// </summary>
        /// <param name="e">Validation error.</param>
        /// <param name="action">Action which got validated.</param>
        /// <param name="dispatcher">Multi-dispatcher.</param>
        protected virtual void OnValidationError(ValidationException e, TAction action, IMultiDispatcher dispatcher)
        {
            dispatcher.Dispatch(e);
        }

        /// <summary>
        /// Asynchronously process the action that has been dispatched to store.
        /// </summary>
        /// <param name="getState">Get latest state</param>
        /// <param name="action">Action that has been dispatched to store</param>
        /// <param name="dispatcher">Multi-dispatcher</param>
        /// <param name="cancellationToken">Token to request cancelation</param>
        /// <returns></returns>
        protected virtual Task OnProcess(Func<TState> getState, TAction action, IMultiDispatcher dispatcher, CancellationToken cancellationToken)
        {
            return Task.WhenAll();
        }

        async Task<PreProcessResult> ILogic<TState, TAction>.PreProcess(IStore<TState> store, TAction action)
        {
            if (_killed)
            {
                // Once fatal poision pill is received don't continue.
                return new PreProcessResult(false, action);
            }

            CancellationTokenSource = new CancellationTokenSource();
            // Transformation occurs first
            action = await OnTransfrom(() => store.State, action, CancellationTokenSource.Token);

            // Validation occurs second.
            ValidationResult validationResult;
            using (var dispatcher = MultiDispatcher.Create(store))
            {
                validationResult = await OnValidate(() => store.State, action, dispatcher, CancellationTokenSource.Token);
            }

            if (!validationResult.IsValid)
            {
                // If validation fails.
                using (var dispatcher = MultiDispatcher.Create(store))
                {
                    OnValidationError(validationResult.Error, action, dispatcher);
                }
                return new PreProcessResult(continueToNext: false, action: action);
            }
            // When validation passes.
            return new PreProcessResult(continueToNext: true, action: action);
        }

        Task ILogic<TState, TAction>.Process(Func<TState> getState, TAction action, IMultiDispatcher dispatcher)
        {
            if (_killed)
            {
                // Once fatal poision pill is received don't continue.
                TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
                tcs.SetCanceled();
                return tcs.Task;
            }

            return OnProcess(getState, action, dispatcher, CancellationTokenSource.Token);
        }
    }
}
