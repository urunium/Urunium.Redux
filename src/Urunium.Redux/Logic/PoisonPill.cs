using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Urunium.Redux.Logic
{
    /// <summary>
    /// All logic must handle this Action type, and cancel it's process when received.
    /// Meaning all the logic currently running can be killed by dispatching PoisionPill at once.
    /// This action is meant to be used in conjunction with Logic.
    /// </summary>
    public class PoisonPill
    {
        /// <summary>
        /// Reason describing why this poison-pill was supplied.
        /// </summary>
        public string Reason { get; }

        /// <summary>
        /// Decides fatality of poison-pill. Setting <see cref="Kill"/> to false will just cancel
        /// all task in progress but not kill the system.
        /// </summary>
        public bool Kill { get; }

        /// <summary>
        /// Instanciate new <see cref="PoisonPill"/>
        /// </summary>
        /// <param name="reason">Reason describing why this poison-pill was supplied.</param>
        /// <param name="kill">Decides fatality of poison-pill. Setting <see cref="Kill"/> 
        /// to false will just cancel all task in progress but not kill the system
        /// </param>
        public PoisonPill(string reason, bool kill = true)
        {
            Reason = reason;
            Kill = kill;
        }
    }
}
