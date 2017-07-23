using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Urunium.Redux
{
    /// <summary>
    /// Default implementation of store.
    /// </summary>
    /// <typeparam name="TState">Type of State</typeparam>
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
