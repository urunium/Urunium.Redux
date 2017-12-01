using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Urunium.Redux.Logic
{
    /// <summary>
    /// A scope under which `Dispatch` doesn't immediately fire state changed event. State is 
    /// changed silently and one state changed event is fired when scope gets disposed.
    /// </summary>
    public interface IMultiDispatcher : IDisposable
    {
        /// <summary>
        /// Dispatch action to store, without firing StateChange event. All state changes are done
        /// silently, and single StateChange event is fired at the end, when scope gets disposed.
        /// </summary>
        /// <typeparam name="TAction">Type of action to dispatch</typeparam>
        /// <param name="action">Action object to dispatch</param>
        void Dispatch<TAction>(TAction action);

        /// <summary>
        /// Dispatch and cause to fire StateChange immediately. Even when under multi-dispatcher
        /// scope, there may be some changes that makes sense to be reflected in UI immediately.
        /// Typically DispatchImmediately is intended to dispatch in-progress actions before actual
        /// dispatches begin.
        /// Warning:
        /// - Dispatching immediately in middle of dispatch sequence may cause UI to render partally correct state.
        /// </summary>
        /// <typeparam name="TAction">Type of action to dispatch</typeparam>
        /// <param name="action">Action object to dispatch</param>
        void DispatchImmediate<TAction>(TAction action);

        /// <summary>
        /// Begin a nested scope of multi-dispatcher.
        /// </summary>
        /// <returns></returns>
        IMultiDispatcher BeginScope();
    }
}
