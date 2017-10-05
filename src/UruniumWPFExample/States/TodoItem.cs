namespace UruniumWPFExample.States
{
    /// <summary>
    /// A state object to store todo item
    /// </summary>
    public class TodoItem
    {
        /// <summary>
        /// Identifier of an todo item.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Text describing what needs to be done.
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// Flag marking if todo item is completed or still active.
        /// </summary>
        public bool Completed { get; }

        /// <summary>
        /// Create an instance of TodoItem
        /// </summary>
        /// <param name="id">Identifier of an todo item.</param>
        /// <param name="text">Text describing what needs to be done.</param>
        /// <param name="completed">Flag marking if todo item is completed or still active.</param>
        public TodoItem(int id, string text, bool completed)
        {
            Id = id;
            Text = text;
            Completed = completed;
        }
    }
}
