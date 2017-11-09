using WpfTodoMvcExample.States;

namespace WpfTodoMvcExample.Actions
{
    /// <summary>
    /// Action to add new Todo item.
    /// </summary>
    public class AddTodo
    {
        /// <summary>
        /// The TodoItem to be added.
        /// </summary>
        public TodoItem Payload { get; }

        /// <summary>
        /// Create new instance of AddTodo action.
        /// </summary>
        /// <param name="todoItem">The new todo item to be added.</param>
        public AddTodo(TodoItem todoItem)
        {
            Payload = todoItem;
        }

        /// <summary>
        /// Create a new instance of AddTodo action.
        /// </summary>
        /// <param name="id">Id of the new todo item to be added.</param>
        /// <param name="text">Text of the new todo item to be added.</param>
        /// <param name="completed">Completed property of the new todo item to be added.</param>
        public AddTodo(int id, string text, bool completed)
            : this(new TodoItem(id, text, completed))
        {
        }
    }
}
