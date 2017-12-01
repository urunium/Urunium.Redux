using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Urunium.Redux.Logic
{
    /// <summary>
    /// Utility class for ability to handle any type of action. To be used in conjunction with
    /// <see cref="LogicBase{TState, TAction}"/> class. To get the instance of action that was
    /// actually dispatched use <see cref="AnyAction.ActualAction"/> property.
    /// </summary>
    /// <example>
    /// <code>
    /// <![CDATA[
    /// 
    /// interface IAction
    /// {
    ///     long TimeStamp { get; set; }
    /// }
    /// 
    /// // An implementation of logic that can handle any action.
    /// class AddTimeStamp<AppState> : LogicBase<AppState, AnyAction>
    /// {
    ///     public override Type CancelType => null; // Nothing can cancel this.
    /// 
    ///     public override uint Priority => 0; // No priority. Bigger number gets higer priority.
    /// 
    ///     protected override Task<AnyAction> OnTransfrom(Func<AppState> getState, AnyAction action, CancellationToken cancellationToken)
    ///     {
    ///         if (action.ActualAction is IAction actualAction)
    ///         {
    ///             actualAction.TimeStamp = DateTime.UtcNow.Ticks;
    ///         }
    ///         return base.OnTransfrom(getState, action, cancellationToken);
    ///     }
    /// }
    /// ]]>
    /// </code>
    /// </example>
    public class AnyAction
    {
        /// <summary>
        /// Actual action that was dispatched.
        /// </summary>
        public object ActualAction { get; }

        /// <summary>
        /// Internal constructor. AnyAction is not supposed to be actually dispatched.
        /// It is just utility functionality to be used by LogicEhancer to locate Logics
        /// that needs to be invoked for all dispatched action irrespective of type.
        /// </summary>
        /// <param name="action">Actual Action that was dispatched.</param>
        internal AnyAction(object action)
        {
            ActualAction = action;
        }
    }
}
