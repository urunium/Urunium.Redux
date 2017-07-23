using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Urunium.Redux.Compose
{
    /// <summary>
    /// Root reducer that will compose various reducers.
    /// </summary>
    /// <typeparam name="TState">Type of state</typeparam>
    public class ReducerComposer<TState> : IReducer<TState>
    {
        private List<IReducer<TState>> _reducers = new List<IReducer<TState>>();

        /// <summary>
        /// Add a reducer.
        /// </summary>
        /// <param name="stateReducer"></param>
        /// <returns></returns>
        public ReducerComposer<TState> AddStateReducer(IReducer<TState> stateReducer)
        {
            _reducers.Add(stateReducer);
            return this;
        }

        /// <summary>
        /// Apply action to state using registered reducers.
        /// </summary>
        /// <param name="previousState">State that needs to be transformed</param>
        /// <param name="action">Action that needs to be applied to state.</param>
        /// <returns>Resulting state after applying action.</returns>
        public TState Apply(TState previousState, object action)
        {
            var nextState = previousState;
            foreach(var reducer in _reducers)
            {
                nextState = reducer.Apply(nextState, action);
            }
            return nextState;
        }
    }
}
