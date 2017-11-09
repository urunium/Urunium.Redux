namespace WpfTodoMvcExample.Actions
{
    /// <summary>
    /// Action to hide the editor (textbox) for currently editing todo item.
    /// </summary>
    public class HideTodoEditor
    {
        /// <summary>
        /// Id of the todo item that is being edited.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Create new instance of HideTodoEditor action.
        /// </summary>
        /// <param name="id">Id of the todo item that is being edited.</param>
        public HideTodoEditor(int id)
        {
            Id = id;
        }
    }
}
