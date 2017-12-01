using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Urunium.Redux
{
    /// <summary>
    /// Default implementation of <see cref="IStore{TState}"/>.
    /// </summary>
    /// <typeparam name="TState">Type of State</typeparam>
    /// <example>
    /// <code>
    /// <![CDATA[
    /// class IncrementAction { }
    /// class DecrementAction { }
    /// class Counter : IReducer<int>
    /// {
    ///     public int Apply(int previousState, object action)
    ///     {
    ///         switch (action)
    ///         {
    ///             case IncrementAction inc:
    ///                 return previousState + 1;
    ///             case DecrementAction dec:
    ///                 return previousState - 1;
    ///         }
    ///         // Unsupported actions should return previousState unchanged.
    ///         return previousState;
    ///     }
    /// }
    /// // Using Reducer in store.
    /// var rootReducer = new Counter();
    /// var initialState = 0;
    /// var store = new Store<int>(rootReducer, initialState);
    /// store.StateChanged += (sender, eventArgs) => 
    /// {
    ///     // update ui
    ///     Console.WriteLine(store.State);
    /// };
    ///     
    /// store.Dispatch(new IncrementAction());
    /// // 1
    /// store.Dispatch(new IncrementAction());
    /// // 2
    /// store.Dispatch(new DecrementAction());
    /// // 1
    /// ]]>
    /// </code>
    /// </example>
    public class Store<TState> : IStore<TState>
    {
        private IReducer<TState> _reducer;
        private TState _state;

        /// <summary>
        /// Get application's State object which needs to be held by this store.
        /// </summary>
        public TState State => _state;

        /// <summary>
        /// Can be subscribed to get notified about change in state.
        /// </summary>
        public event EventHandler<EventArgs> StateChanged;

        /// <summary>
        /// Store should take in the root reducer, and initial state.
        /// </summary>
        /// <param name="rootReducer"></param>
        /// <param name="initialState"></param>
        public Store(IReducer<TState> rootReducer, TState initialState)
        {
            _reducer = rootReducer;
            _state = initialState;
        }

        /// <summary>
        /// Dispatch action to reducers which will then apply the actions.
        /// Also, notifies about state change by firing StageChanged event.
        /// </summary>
        /// <typeparam name="TAction">Type of action</typeparam>
        /// <param name="action">Action Object</param>
        public void Dispatch<TAction>(TAction action)
        {
            var previousState = _state;
            var nextState = _reducer.Apply(_state, action);
            if (!nextState.Equals(previousState))
            {
                _state = nextState;
                StateChanged?.Invoke(this, new EventArgs());
            }
        }
    }
}
