using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Urunium.Redux.Logic
{
    /// <summary>
    /// Result of preprocessing logic.
    /// </summary>
    public class PreProcessResult
    {
        /// <summary>
        /// Whether or not continue to execute the chain of logic.
        /// </summary>
        public bool ContinueToNextStep { get; }

        /// <summary>
        /// Action that was dispatched
        /// </summary>
        public object Action { get; }

        /// <summary>
        /// Immutable constructor to instanciate new <see cref="PreProcessResult"/>.
        /// </summary>
        /// <param name="continueToNext"></param>
        /// <param name="action"></param>
        public PreProcessResult(bool continueToNext, object action)
        {
            ContinueToNextStep = continueToNext;
            Action = action;
        }
    }
}
