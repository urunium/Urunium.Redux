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
    /// Reducer to handle modification of <see cref="TodoList.Todos"/> property.
    /// </summary>
    public class TodoItemsReducer : TypedReducer<IReadOnlyDictionary<int, TodoItem>>, ISubTreeReducer<TodoList, IReadOnlyDictionary<int, TodoItem>>
    {
        /// <summary>
        /// Property that will be modified by this reducer.
        /// </summary>
        public Expression<Func<TodoList, IReadOnlyDictionary<int, TodoItem>>> PropertySelector => x => x.Todos;

        /// <summary>
        /// Reducer function to handle AddTodo action.
        /// </summary>
        /// <param name="previousState"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public IReadOnlyDictionary<int, TodoItem> Apply(IReadOnlyDictionary<int, TodoItem> previousState, AddTodo action)
        {
            var newTodo = action.Payload;
            var newState = previousState.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            newState.Add(newTodo.Id, newTodo);
            return new ReadOnlyDictionary<int, TodoItem>(newState);
        }

        /// <summary>
        /// Reducer function to handle ToggleComplete action.
        /// </summary>
        /// <param name="previousState"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public IReadOnlyDictionary<int, TodoItem> Apply(IReadOnlyDictionary<int, TodoItem> previousState, ToggleComplete action)
        {
            var oldTodo = previousState[action.Id];
            var newTodo = new TodoItem(action.Id, oldTodo.Text, !oldTodo.Completed);
            var newState = previousState.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            newState[newTodo.Id] = newTodo;
            return new ReadOnlyDictionary<int, TodoItem>(newState);
        }

        /// <summary>
        /// Reducer function to handle UpdateTodoText action.
        /// </summary>
        /// <param name="previousState"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public IReadOnlyDictionary<int, TodoItem> Apply(IReadOnlyDictionary<int, TodoItem> previousState, UpdateTodoText action)
        {
            var oldTodo = previousState[action.Id];
            var newTodo = new TodoItem(action.Id, action.Text, oldTodo.Completed);
            var newState = previousState.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            newState[newTodo.Id] = newTodo;
            return new ReadOnlyDictionary<int, TodoItem>(newState);
        }

        /// <summary>
        /// Reducer function to handle RemoveTodo action.
        /// </summary>
        /// <param name="previousState"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public IReadOnlyDictionary<int, TodoItem> Apply(IReadOnlyDictionary<int, TodoItem> previousState, RemoveTodo action)
        {
            var newState = previousState.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            if (newState.ContainsKey(action.Id))
            {
                newState.Remove(action.Id);
            }
            return new ReadOnlyDictionary<int, TodoItem>(newState);
        }

        /// <summary>
        /// Reducer function to handle RemoveCompleted action.
        /// </summary>
        /// <param name="previousState"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public IReadOnlyDictionary<int, TodoItem> Apply(IReadOnlyDictionary<int, TodoItem> previousState, RemoveCompleted action)
        {
            var newState = previousState.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            var completedIds = newState.Where(x => x.Value.Completed).Select(x => x.Key).ToList();
            foreach (var id in completedIds)
            {
                newState.Remove(id);
            }

            return new ReadOnlyDictionary<int, TodoItem>(newState);
        }

        /// <summary>
        /// Reducer function to handle CompleteAllTodos action.
        /// </summary>
        /// <param name="previousState"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public IReadOnlyDictionary<int, TodoItem> Apply(IReadOnlyDictionary<int, TodoItem> previousState, CompleteAllTodos action)
        {
            var newState = previousState.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            var activeTodoIds = newState.Where(x => !x.Value.Completed).Select(x => x.Key).ToList();
            foreach (var id in activeTodoIds)
            {
                var oldTodo = previousState[id];
                var newTodo = new TodoItem(id, oldTodo.Text, true);
                newState[id] = newTodo;
            }

            return new ReadOnlyDictionary<int, TodoItem>(newState);
        }

        /// <summary>
        /// Reducer function to handle MarkAllTodosAsActive action.
        /// </summary>
        /// <param name="previousState"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public IReadOnlyDictionary<int, TodoItem> Apply(IReadOnlyDictionary<int, TodoItem> previousState, MarkAllTodosAsActive action)
        {
            var newState = previousState.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            var inactiveTodos = newState.Where(x => x.Value.Completed).Select(x => x.Key).ToList();
            foreach (var id in inactiveTodos)
            {
                var oldTodo = previousState[id];
                var newTodo = new TodoItem(id, oldTodo.Text, false);
                newState[id] = newTodo;
            }

            return new ReadOnlyDictionary<int, TodoItem>(newState);
        }
    }
}
