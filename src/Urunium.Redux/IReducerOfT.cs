using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Urunium.Redux
{
    /// <summary>
    /// Reducer takes in a state, applies an action and returns resultant state.
    /// Action can be any object that is supported by reducer, there isn't any hard
    /// and fast rule for action. Could even be primitive types.
    /// </summary>
    /// <typeparam name="TState">Type of state</typeparam>
    public interface IReducer<TState>
    {
        /// <summary>
        /// Apply an action to old state and return a new state.
        /// </summary>
        /// <param name="previousState">Existing state</param>
        /// <param name="action">Action that needs to be applied</param>
        /// <returns>New state</returns>
        TState Apply(TState previousState, object action);
    }
}
