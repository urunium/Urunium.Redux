using WpfTodoMvcExample.States;

namespace WpfTodoMvcExample.Actions
{
    /// <summary>
    /// Action to show editor (textbox) for a particular todo item.
    /// Helps in updating the todo item's description text.
    /// </summary>
    public class ShowTodoEditor
    {
        /// <summary>
        /// The todo item for which editor needs to be shown.
        /// </summary>
        public TodoItem Payload { get; }

        /// <summary>
        /// Create new instance of ShowTodoEditor action.
        /// </summary>
        /// <param name="payload">The todo item for which editor needs to be shown.</param>
        public ShowTodoEditor(TodoItem payload)
        {
            Payload = payload;
        }
    }
}
