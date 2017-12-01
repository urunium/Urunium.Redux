using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Urunium.Redux.Undoable
{
    /// <summary>
    /// Datastructure to support undo/redo for state. Use this in conjunction with <see cref="UndoableReducer{TState}"/>
    /// to support <see cref="Undo"/>/<see cref="Redo"/> actions. 
    /// The store instance must be <see cref="IStore{TUndoableState}"/> of <see cref="UndoableState{TState}"/> where TState 
    /// is the type of application's state.
    /// </summary>
    /// <typeparam name="TState">Type of application's state.</typeparam>
    public class UndoableState<TState>
    {
        /// <summary>
        /// List of past states
        /// </summary>
        public IReadOnlyList<TState> Past { get; }
        /// <summary>
        /// List of future states
        /// </summary>
        public IReadOnlyList<TState> Future { get; }
        /// <summary>
        /// Current state.
        /// </summary>
        public TState Present { get; }

        /// <summary>
        /// Create an instance of <see cref="UndoableState{TState}"/>, with current state, past states and future states.
        /// </summary>
        /// <param name="present">Current state.</param>
        /// <param name="past">List of past states.</param>
        /// <param name="future">List of future states.</param>
        public UndoableState(TState present, IReadOnlyList<TState> past, IReadOnlyList<TState> future)
        {
            Past = past ?? new ReadOnlyCollection<TState>(new List<TState>());
            Present = present;
            Future = future ?? new ReadOnlyCollection<TState>(new List<TState>());
        }

        /// <summary>
        /// Create an instance of <see cref="UndoableState{TState}"/>, with just current state.
        /// </summary>
        /// <param name="present"></param>
        public UndoableState(TState present)
            : this(present, null, null)
        {
        }
    }

    /// <summary>
    /// Reducer to support undo/redo. This reducer is an enhancer for actual <see cref="IReducer{TState}"/>, 
    /// which handles <see cref="Undo"/>/<see cref="Redo"/> actions and forwards all other action to underlying
    /// reducer. Use this in conjunction with <see cref="UndoableState{TState}"/>
    /// to support <see cref="Undo"/>/<see cref="Redo"/> actions.
    /// </summary>
    /// <typeparam name="TState">Type of application's state.</typeparam>
    /// <example>
    /// <code>
    /// <![CDATA[
    /// // Root reducer.
    /// public class Counter : Typed.TypedReducer<int>
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
    /// 
    /// // usage of UndoableState and UndoableReducer...
    /// IStore<UndoableState<int>> store = new Store<UndoableState<int>>(
    /// new UndoableReducer<int>(new Counter()),
    /// new UndoableState<int>(0));
    /// ]]>
    /// </code>
    /// </example>
    public class UndoableReducer<TState> : IReducer<UndoableState<TState>>
    {
        IReducer<TState> _innerReducer;
        int _keep = 10;

        /// <summary>
        /// Create an instance of <see cref="UndoableReducer{TState}"/>.
        /// </summary>
        /// <param name="innerReducer">Reducer which needs to be enhanced with undo/redo ability.</param>
        /// <param name="keep">Number of historical states to be preserved while supporting undo/redo.</param>
        public UndoableReducer(IReducer<TState> innerReducer, int keep = 10)
        {
            _innerReducer = innerReducer;
            _keep = keep;
        }

        /// <summary>
        /// Reducer function to support undo/redo.
        /// </summary>
        /// <param name="previousState">Current state stored in <see cref="Store{TState}"/> object.</param>
        /// <param name="action">Action to be applied to state.</param>
        /// <returns>New state after applying action.</returns>
        public UndoableState<TState> Apply(UndoableState<TState> previousState, object action)
        {
            TState present;
            switch (action)
            {
                case Undo undoAction:
                    if (previousState.Past.Count > 0)
                    {
                        List<TState> past = previousState.Past.ToList();
                        List<TState> future = previousState.Future.ToList();
                        present = past.Last();
                        future.Add(previousState.Present);
                        past.RemoveAt(past.Count - 1);
                        return new UndoableState<TState>(present, new ReadOnlyCollection<TState>(past), new ReadOnlyCollection<TState>(future));
                    }
                    return previousState;
                case Redo redoAction:
                    if (previousState.Future.Count > 0)
                    {
                        List<TState> past = previousState.Past.ToList();
                        List<TState> future = previousState.Future.ToList();
                        present = future.Last();
                        past.Add(previousState.Present);
                        future.RemoveAt(future.Count - 1);
                        return new UndoableState<TState>(present, new ReadOnlyCollection<TState>(past), new ReadOnlyCollection<TState>(future));
                    }
                    return previousState;
                default:
                    {
                        List<TState> past = previousState.Past.ToList();
                        List<TState> future = new List<TState>();
                        past.Add(previousState.Present);
                        present = _innerReducer.Apply(previousState.Present, action);
                        return new UndoableState<TState>(present, new ReadOnlyCollection<TState>(past), new ReadOnlyCollection<TState>(future));
                    }
            }
        }
    }
}
