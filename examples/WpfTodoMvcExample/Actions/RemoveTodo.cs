namespace WpfTodoMvcExample.Actions
{
    /// <summary>
    /// Action to Remove a particular todo item.
    /// </summary>
    public class RemoveTodo
    {
        /// <summary>
        /// Id of todo item that needs to be removed.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Create new instance of RemoveTodo action.
        /// </summary>
        /// <param name="id">Id of todo item that needs to be removed.</param>
        public RemoveTodo(int id)
        {
            Id = id;
        }
    }
}
