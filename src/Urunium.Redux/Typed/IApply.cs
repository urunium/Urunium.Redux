using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Urunium.Redux.Typed
{
    /// <summary>
    /// Helper interface to be used with <see cref="TypedReducer{TState}"/>. Note that <see cref="TypedReducer{TState}"/>
    /// does work without implementing <see cref="IApply{TState, TAction}"/> interface, but implementing this interface
    /// helps in making implementation more testable, and in visual studio it also helps in generating template methods.
    /// </summary>
    /// <typeparam name="TState">Type of state this reducer handles.</typeparam>
    /// <typeparam name="TAction">Type of action to be applied to state.</typeparam>
    /// <example>
    /// <code>
    /// <![CDATA[
    /// public class Counter : Typed.TypedReducer<int>, IApply<int, Increment>, IApply<int, Decrement>;
    /// {
    ///    public int Apply(int previousState, Increment action)
    ///    {
    ///        return previousState + 1;
    ///    }
    ///    
    ///    public int Apply(int previousState, Decrement action)
    ///    {
    ///        return previousState - 1;
    ///    }
    /// }
    /// ]]>
    /// </code>
    /// </example>
    public interface IApply<TState, TAction>
    {
        /// <summary>
        /// Reducer function that applies specific type of TAction to given TState.
        /// </summary>
        /// <param name="previousState">The state currently stored in Store</param>
        /// <param name="action">Action to be applied to state</param>
        /// <returns>New state after applying action.</returns>
        TState Apply(TState previousState, TAction action);
    }
}
