using System;
using UruniumWPFExample.Fx;
using TodoItemViewModelCollection = Caliburn.Micro.BindableCollection<UruniumWPFExample.TodoItemViewModel>;

namespace UruniumWPFExample
{
    /// <summary>
    /// Viewmodel for TodoListAppView. A concrete implementation of <see cref="Connectable"/>.
    /// </summary>
    public class TodoListAppViewModel : Connectable
    {
        /// <summary>
        /// Determines whether or not to activate 'Active' link button.
        /// </summary>
        public bool CanClickShowActive { get; set; }

        /// <summary>
        /// Determines whether or not to activate 'All' link button.
        /// </summary>
        public bool CanClickShowAll { get; set; }

        /// <summary>
        /// Determines whether or not to activate 'Completed' link button.
        /// </summary>
        public bool CanClickShowCompleted { get; set; }

        /// <summary>
        /// Determines whether or not to check mark toggle all checkbox.
        /// </summary>
        public bool CheckToggleAll { get; set; }

        /// <summary>
        /// Determines whether or not to show 'Clear completed' link button.
        /// </summary>
        public bool ClearButtonIsVisible { get; set; }

        /// <summary>
        /// Dispatch ActivateAll action.
        /// </summary>
        public Action DispatchActivateAll { get; set; }

        /// <summary>
        /// Dispatch AddTodo action.
        /// </summary>
        public Action<string> DispatchAdd { get; set; }

        /// <summary>
        /// Dispatch ClearCompleted action.
        /// </summary>
        public Action DispatchClearCompleted { get; set; }

        /// <summary>
        /// Dispatch CompleteAll action.
        /// </summary>
        public Action DispatchCompleteAll { get; set; }

        /// <summary>
        /// Dispatch ShowActive action.
        /// </summary>
        public Action DispatchShowActive { get; set; }

        /// <summary>
        /// Dispatch ShowAll action.
        /// </summary>
        public Action DispatchShowAll { get; set; }

        /// <summary>
        /// Dispatch ShowCompleted action.
        /// </summary>
        public Action DispatchShowCompleted { get; set; }

        /// <summary>
        /// Determines whether or not there are any todo items, that needs to be shown in UI.
        /// Depends upon the number of todo items, and visibility filter.
        /// </summary>
        public bool HasTodoItems { get; set; }

        /// <summary>
        /// Number of active items, i.e. items that aren't marked as completed.
        /// </summary>
        public int ItemsLeft { get; set; }

        /// <summary>
        /// Description text of todo item that needs to be added.
        /// </summary>
        public string NewTodo { get; set; }

        /// <summary>
        /// List of todo items.
        /// </summary>
        public TodoItemViewModelCollection Todos { get; } = new TodoItemViewModelCollection();

        /// <summary>
        /// List of todo items, sorted by Id.
        /// </summary>
        public System.ComponentModel.ICollectionView SortedTodos { get; }

        /// <summary>
        /// Handle click on `Clear completed` button.
        /// </summary>
        public void OnClearCompletedClick() => DispatchClearCompleted?.Invoke();

        /// <summary>
        /// Handle Enter key event on NewTodo textbox.
        /// </summary>
        public void OnNewTodoEnterKeyPress() => DispatchAdd?.Invoke(NewTodo);

        /// <summary>
        /// Handle click on `Active` button.
        /// </summary>
        public void OnShowActiveClick() => DispatchShowActive?.Invoke();

        /// <summary>
        /// Handle click on 'All' button.
        /// </summary>
        public void OnShowAllClick() => DispatchShowAll?.Invoke();

        /// <summary>
        /// Handle click on 'Completed' button.
        /// </summary>
        public void OnShowCompletedClick() => DispatchShowCompleted?.Invoke();

        /// <summary>
        /// Handle check on toggle all check box (top most checkbox).
        /// </summary>
        public void OnToggleAllChecked() => DispatchCompleteAll?.Invoke();

        /// <summary>
        /// Handle uncheck of toggle all check box.
        /// </summary>
        public void OnToggleAllUnchecked() => DispatchActivateAll?.Invoke();

        public TodoListAppViewModel()
        {
            SortedTodos = System.Windows.Data.CollectionViewSource.GetDefaultView(Todos);
            SortedTodos.SortDescriptions.Add(new System.ComponentModel.SortDescription("Id", System.ComponentModel.ListSortDirection.Ascending));
        }
    }
}
