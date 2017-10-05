using System.Collections.Generic;

namespace UruniumWPFExample.States
{
    /// <summary>
    /// State object store todo mvc state tree.
    /// </summary>
    public class TodoList
    {
        /// <summary>
        /// All available todo items.
        /// </summary>
        public IReadOnlyDictionary<int, TodoItem> Todos { get; }

        /// <summary>
        /// Todos that are being currently edited.
        /// </summary>
        public IReadOnlyDictionary<int, TodoItem> EditingTodos { get; }

        /// <summary>
        /// Filter which todo items should be visible.
        /// </summary>
        public VisibilityFilter Filter { get; }

        /// <summary>
        /// The initial state.
        /// </summary>
        public static TodoList InitialState => new TodoList();

        /// <summary>
        /// Create an instance of TodoList
        /// </summary>
        /// <param name="todos">All available todo items.</param>
        /// <param name="editingTodos">Todos that are being currently edited.</param>
        /// <param name="filter">Filter which todo items should be visible.</param>
        public TodoList(IReadOnlyDictionary<int, TodoItem> todos, IReadOnlyDictionary<int, TodoItem> editingTodos, VisibilityFilter filter)
        {
            Todos = todos;
            EditingTodos = editingTodos;
            Filter = filter;
        }

        /// <summary>
        /// Create an instance of TodoList, with default values.
        /// </summary>
        public TodoList()
            : this(new Dictionary<int, TodoItem>(), new Dictionary<int, TodoItem>(), VisibilityFilter.ShowAllItems)
        {

        }
    }
}
