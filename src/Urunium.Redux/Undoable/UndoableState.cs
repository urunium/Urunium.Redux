using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Urunium.Redux.Undoable
{
    public class UndoableState<TState>
    {
        public IReadOnlyList<TState> Past { get; }
        public IReadOnlyList<TState> Future { get; }
        public TState Present { get; }

        public UndoableState(TState present, IReadOnlyList<TState> past, IReadOnlyList<TState> future)
        {
            Past = past ?? new List<TState>().AsReadOnly();
            Present = present;
            Future = future ?? new List<TState>().AsReadOnly();
        }

        public UndoableState(TState present)
            : this(present, null, null)
        {
        }
    }

    public class UndoableReducer<TState> : IReducer<UndoableState<TState>>
    {
        IReducer<TState> _innerReducer;
        int _keep = 10;
        public UndoableReducer(IReducer<TState> innerReducer, int keep = 10)
        {
            _innerReducer = innerReducer;
            _keep = keep;
        }

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
                        return new UndoableState<TState>(present, past.AsReadOnly(), future.AsReadOnly());
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
                        return new UndoableState<TState>(present, past.AsReadOnly(), future.AsReadOnly());
                    }
                    return previousState;
                default:
                    {
                        List<TState> past = previousState.Past.ToList();
                        List<TState> future = new List<TState>();
                        past.Add(previousState.Present);
                        present = _innerReducer.Apply(previousState.Present, action);
                        return new UndoableState<TState>(present, past.AsReadOnly(), future.AsReadOnly());
                    }
            }
        }
    }
}
