using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Urunium.Redux.Logic
{
    /// <summary>
    /// Factory class to create <see cref="IMultiDispatcher"/>.
    /// </summary>
    /// <example>
    /// <code>
    /// Example of how multi-dispatcher generally will be used.
    /// <![CDATA[
    ///     using(var dispatcher = MultiDispatcher.Create(store))
    ///     {
    ///         dispatcher.DispatchImmediately("Long runing process has began");
    ///         dispatcher.Dispatch("1");
    ///         dispatcher.Dispatch("2");
    ///         dispatcher.Dispatch("3");
    ///     }
    /// ]]>
    /// </code>
    /// </example>
    public static class MultiDispatcher
    {
        /// <summary>
        /// Create an instance of IMultiDispatcher
        /// </summary>
        /// <typeparam name="TState">Type of Application's state</typeparam>
        /// <param name="store">Instance of store for which multi-dispatcher needs to be created.</param>
        /// <returns>Instance of <see cref="IMultiDispatcher"/></returns>
        public static IMultiDispatcher Create<TState>(IStore<TState> store)
        {
            return new MultiDispatchableStore<TState>(store);
        }

        /// <summary>
        /// Multi-dispatcher is essentially a store enhancer. But this enhancer is applied only within the scope 
        /// of IMultiDispatcher, and gets unapplied once IMultiDispatcher is disposed.
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        private class MultiDispatchableStore<TState> : Enhance.StoreEnhancer<TState>, IMultiDispatcher
        {
            private IStore<TState> _store;
            private Delegate _stateChanged = null;
            private bool _dispatchInvokedAtLeastOnce = false;
            public MultiDispatchableStore(IStore<TState> store) : base(store)
            {
                _store = store;
                _stateChanged = ShiftAllEvents(_store);
            }

            public IMultiDispatcher BeginScope()
            {
                var scope = new MultiDispatchableStore<TState>(this);
                scope._stateChanged = _stateChanged;
                return scope;
            }

            public void Dispose()
            {
                if (_dispatchInvokedAtLeastOnce)
                {
                    _stateChanged?.DynamicInvoke(this, new EventArgs());
                }
                // move back all event handlers to underlying store.
                UnShiftAllEvents(_stateChanged, _store);
                _stateChanged = null;
            }

            // Dispatch and fire statechange event immediately
            public void DispatchImmediate<TAction>(TAction action)
            {
                _store.Dispatch(action);
                _stateChanged?.DynamicInvoke(this, new EventArgs());
            }

            protected override void OnDispatch<TAction>(TAction action, Action<TAction> forward)
            {
                _dispatchInvokedAtLeastOnce = true;
                // since all StateChange eventhandler has been removed from underlying store, just
                // forward the action to underlying store.
                forward(action);
            }

            /// <summary>
            /// Remove all the eventhandlers from StateChanged event of original store (<see cref="_store"/>), and return them
            /// as a combined multi cast delegate.
            /// <a href="https://stackoverflow.com/a/16089998/605113">Reference</a>
            /// </summary>
            /// <param name="originalStore"></param>
            /// <returns>Combined Delegate of StateChanged event handlers attached to original store.</returns>
            private Delegate ShiftAllEvents(IStore<TState> originalStore)
            {
                // @ref: https://stackoverflow.com/a/16089998/605113
                Type theType = originalStore.GetType();
                Delegate finalDelegate = null;

                //Even though the events are public, the FieldInfo associated with them is private
                foreach (System.Reflection.FieldInfo field in theType.GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance))
                {
                    //eventInfo will be null if this is a normal field and not an event.
                    System.Reflection.EventInfo eventInfo = theType.GetEvent(field.Name);
                    if (eventInfo != null && field.Name == nameof(StateChanged))
                    {
                        MulticastDelegate multicastDelegate = field.GetValue(originalStore) as MulticastDelegate;
                        if (multicastDelegate != null)
                        {
                            List<Delegate> oldHandlers = new List<Delegate>();
                            foreach (Delegate @delegate in multicastDelegate.GetInvocationList())
                            {
                                eventInfo.RemoveEventHandler(originalStore, @delegate);
                                oldHandlers.Add(@delegate);
                            }

                            if (oldHandlers.Count > 0)
                            {
                                finalDelegate = Delegate.Combine(oldHandlers.ToArray());
                            }
                        }
                    }
                }

                return finalDelegate;
            }

            /// <summary>
            /// Add back all events from sourceDelegate to StateChanged event of original store targetStore.
            /// </summary>
            /// <param name="sourceDelegate">Delegate which holds all the event handlers. This is returned by <see cref="ShiftAllEvents(IStore{TState})"/></param>
            /// <param name="targetStore">Store to which StateChanged event handlers needs to be added back.</param>
            private void UnShiftAllEvents(Delegate sourceDelegate, IStore<TState> targetStore)
            {
                Type theType = targetStore.GetType();

                foreach (System.Reflection.FieldInfo field in theType.GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance))
                {
                    System.Reflection.EventInfo eventInfo = theType.GetEvent(field.Name);
                    if (eventInfo != null && field.Name == nameof(StateChanged))
                    {
                        MulticastDelegate multicastDelegate = sourceDelegate as MulticastDelegate;
                        if (multicastDelegate != null)
                        {
                            List<Delegate> oldHandlers = new List<Delegate>();
                            foreach (Delegate @delegate in multicastDelegate.GetInvocationList())
                            {
                                eventInfo.AddEventHandler(targetStore, @delegate);
                                Delegate.Remove(multicastDelegate, @delegate);
                            }
                        }
                    }
                }
            }
        }
    }
}
