namespace WpfTodoMvcExample.Actions
{
    /// <summary>
    /// Action to toggle complete flag of particular todo item.
    /// </summary>
    public class ToggleComplete
    {
        /// <summary>
        /// Id of the todo item whose complete flag needs to be toggled.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Create new instance of ToggleComplete action.
        /// </summary>
        /// <param name="id">Id of the todo item whose complete flag needs to be toggled.</param>
        public ToggleComplete(int id)
        {
            Id = id;
        }
    }
}
