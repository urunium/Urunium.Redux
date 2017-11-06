using UruniumWPFExample.States;

namespace UruniumWPFExample.Actions
{
    /// <summary>
    /// Action to change visibility filter. Visibility filter determines,
    /// which todo items should be shown in UI.
    /// </summary>
    public class ChangeVisibilityFilter
    {
        /// <summary>
        /// Visibility fiter to apply.
        /// </summary>
        public VisibilityFilter Payload { get; }

        /// <summary>
        /// Create new instance of ChangeVisibilityFilter action.
        /// </summary>
        /// <param name="visibilityFilter">Visibility fiter to apply.</param>
        public ChangeVisibilityFilter(VisibilityFilter visibilityFilter)
        {
            Payload = visibilityFilter;
        }
    }
}
