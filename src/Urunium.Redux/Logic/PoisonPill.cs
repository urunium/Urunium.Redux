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
    /// </summary>
    public class PoisonPill
    {
        public string Reason { get; }

        public bool Kill { get; }

        public PoisonPill(string reason, bool kill = true)
        {
            Reason = reason;
            Kill = kill;
        }
    }
}
