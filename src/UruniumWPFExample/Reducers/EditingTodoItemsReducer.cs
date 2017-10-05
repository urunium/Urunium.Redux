using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using Urunium.Redux.Compose;
using Urunium.Redux.Typed;
using UruniumWPFExample.Actions;
using UruniumWPFExample.States;

namespace UruniumWPFExample.Reducers
{
    /// <summary>
    /// Reducer to handle modification of <see cref="TodoList.EditingTodos"/> property.
    /// </summary>
    public class EditingTodoItemsReducer : TypedReducer<IReadOnlyDictionary<int, TodoItem>>, ISubTreeReducer<TodoList, IReadOnlyDictionary<int, TodoItem>>
    {
        /// <summary>
        /// Property that will be modified by this reducer.
        /// </summary>
        public Expression<Func<TodoList, IReadOnlyDictionary<int, TodoItem>>> PropertySelector => x => x.EditingTodos;

        /// <summary>
        /// Reducer function to handle ShowTodoEditor action.
        /// </summary>
        /// <param name="previousState"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public IReadOnlyDictionary<int, TodoItem> Apply(IReadOnlyDictionary<int, TodoItem> previousState, ShowTodoEditor action)
        {
            var newTodo = action.Payload;
            var newState = previousState.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            if (!newState.ContainsKey(newTodo.Id))
            {
                newState.Add(newTodo.Id, newTodo);
            }
            return new ReadOnlyDictionary<int, TodoItem>(newState);
        }

        /// <summary>
        /// Reducer function to handle HideTodoEditor action.
        /// </summary>
        /// <param name="previousState"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public IReadOnlyDictionary<int, TodoItem> Apply(IReadOnlyDictionary<int, TodoItem> previousState, HideTodoEditor action)
        {
            var newState = previousState.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            if (newState.ContainsKey(action.Id))
            {
                newState.Remove(action.Id);
            }
            return new ReadOnlyDictionary<int, TodoItem>(newState);
        }
    }
}
