namespace UruniumWPFExample.Actions
{
    /// <summary>
    /// Action to update todo item's description text.
    /// </summary>
    public class UpdateTodoText
    {
        /// <summary>
        /// Id of todo item whose description text needs to be updated.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// The new text for todo item.
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// Create new instance of UpdateTodoText action.
        /// </summary>
        /// <param name="id">Id of todo item whose description text needs to be updated.</param>
        /// <param name="text">The new text for todo item.</param>
        public UpdateTodoText(int id, string text)
        {
            Id = id;
            Text = text;
        }
    }
}
