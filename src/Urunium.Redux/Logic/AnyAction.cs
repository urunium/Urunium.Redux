using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Urunium.Redux.Logic
{
    /// <summary>
    /// Utility class for ability to handle any type of action.
    /// </summary>
    /// <example>
    /// <code>
    /// <![CDATA[
    /// class MyLogic : LogicBase&lt;AppState, AnyAction&gt;
    /// {
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
