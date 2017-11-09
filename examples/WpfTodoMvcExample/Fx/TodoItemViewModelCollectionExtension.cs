using System.Collections.Generic;
using System.Linq;
using TodoItemViewModelCollection = Caliburn.Micro.BindableCollection<WpfTodoMvcExample.TodoItemViewModel>;

namespace WpfTodoMvcExample.Fx
{
    /// <summary>
    /// Extension methods for BindableCollection of TodoItemViewModel.
    /// </summary>
    public static class TodoItemViewModelCollectionExtension
    {
        /// <summary>
        /// Diff/patch algorithm for BindableCollection of TodoItemViewModel.
        /// </summary>
        /// <param name="bindableCollection">Original bindable collection.</param>
        /// <param name="patch">Patches that needs to be applied.</param>
        public static void Patch(this TodoItemViewModelCollection bindableCollection, IEnumerable<TodoItemViewModel> patch)
        {
            var oldItems = bindableCollection.ToDictionary(x => x.Id);
            var changes = patch.ToDictionary(x => x.Id);

            List<TodoItemViewModel> additions = new List<TodoItemViewModel>();
            foreach (var change in patch)
            {
                // if item is in change but not in old collection.
                if (!oldItems.TryGetValue(change.Id, out TodoItemViewModel updatedItem))
                {
                    additions.Add(change);
                }
                else
                {
                    if (updatedItem != null && change != null)
                    {
                        updatedItem.IsCompleted = change.IsCompleted;
                        updatedItem.Text = change.Text;
                        updatedItem.TextEditor = change.TextEditor;
                        updatedItem.TextEditorIsVisible = change.TextEditorIsVisible;
                    }
                }
            }

            // If item is in old collection but not in changes.
            bindableCollection.RemoveRange(from oldItem in oldItems where !changes.ContainsKey(oldItem.Key) select oldItem.Value);
            bindableCollection.AddRange(additions);
        }
    }
}
