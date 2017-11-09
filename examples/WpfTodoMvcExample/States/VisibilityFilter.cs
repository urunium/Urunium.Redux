namespace WpfTodoMvcExample.States
{
    /// <summary>
    /// Filter which todo items needs to be showin in UI.
    /// </summary>
    public enum VisibilityFilter
    {
        /// <summary>
        /// Show all todo items.
        /// </summary>
        ShowAllItems,

        /// <summary>
        /// Show only completed todo items.
        /// </summary>
        ShowCompletedItems,

        /// <summary>
        /// Show only active items.
        /// </summary>
        ShowActiveItems
    }
}
