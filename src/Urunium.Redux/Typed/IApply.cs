using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Urunium.Redux.Typed
{
    public interface IApply<TState, TAction>
    {
        TState Apply(TState previousState, TAction action);
    }
}
