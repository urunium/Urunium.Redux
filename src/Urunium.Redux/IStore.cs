using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Urunium.Redux
{
    /// <summary>
    /// Store is responsible for holding state. Dispatch action to reducer, 
    /// and notify about change in state to subscribers. Default implementation 
    /// for this interface is <see cref="Store{TState}"/>.
    /// </summary>
    /// <typeparam name="TState">Type of application's state.</typeparam>
    public interface IStore<TState>
    {
        /// <summary>
        /// Application's State object which needs to be held by this store.
        /// </summary>
        TState State { get; }

        /// <summary>
        /// Dispatch action to reducers which will then apply the actions.
        /// Also, notifies about state change by firing StageChanged event.
        /// </summary>
        /// <typeparam name="TAction">Type of action</typeparam>
        /// <param name="action">Action Object</param>
        void Dispatch<TAction>(TAction action);

        /// <summary>
        /// Can be subscribed to get notified about change in state.
        /// </summary>
        event EventHandler<EventArgs> StateChanged;
    }
}
