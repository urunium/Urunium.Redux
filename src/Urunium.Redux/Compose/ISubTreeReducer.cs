using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Urunium.Redux.Compose
{
    /// <summary>
    /// Reducer that works on a part (sub-tree) of application state.
    /// Hence helps in dividing reducers into small reducers, processing independently
    /// smaller part of the state tree.
    /// </summary>
    /// <typeparam name="TState">Type of full state</typeparam>
    /// <typeparam name="TPart">Type of property which this reducer will deal with.</typeparam>
    public interface ISubTreeReducer<TState, TPart> : IReducer<TPart>
    {
        System.Linq.Expressions.Expression<Func<TState, TPart>> PropertySelector { get; }
    }
}
