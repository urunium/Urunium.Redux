using System;
using UruniumWPFExample.Fx;
using UruniumWPFExample.States;

namespace UruniumWPFExample
{
    /// <summary>
    /// Viewmodel for TodoItemView. A concrete implementation of <see cref="Connectable"/>.
    /// </summary>
    public class TodoItemViewModel : Connectable
    {
        /// <summary>
        /// Dispatch HideTodoEditor action.
        /// </summary>
        public Action<int> DispatchHideTodoEditor { get; set; }

        /// <summary>
        /// Dispatch RemoveTodo action.
        /// </summary>
        public Action<int> DispatchRemoveTodo { get; set; }

        /// <summary>
        /// Dispatch ShowTodoEditor action.
        /// </summary>
        public Action<TodoItem> DispatchShowTodoEditor { get; set; }

        /// <summary>
        /// Dispatch ToggleComplete action.
        /// </summary>
        public Action<int> DispatchToggleComplete { get; set; }

        /// <summary>
        /// Dispatch UpdateTodoText action.
        /// </summary>
        public Action<int, string> DispatchUpdateTodoText { get; set; }

        /// <summary>
        /// Identifier for todo item.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Flag marking whether or not todo item is completed.
        /// </summary>
        public bool IsCompleted { get; set; }

        /// <summary>
        /// Current description text of todo item.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Holds text while editing todo item decription.
        /// </summary>
        public string TextEditor { get; set; }

        /// <summary>
        /// Flag marking whether or not textbox be shown for editing text description of todo item.
        /// </summary>
        public bool TextEditorIsVisible { get; set; }

        /// <summary>
        /// Store whether `Esc` key was pressed while editing todo item.
        /// </summary>
        private bool _isEscaped = false;

        /// <summary>
        /// Handle click even on remove todo item button.
        /// </summary>
        public void OnRemoveTodoClick() => DispatchRemoveTodo?.Invoke(Id);

        /// <summary>
        /// Handle Enter key press event on `TextEditor` textbox.
        /// </summary>
        public void OnTextEditorEnterKeyPress() => DispatchUpdateTodoText?.Invoke(Id, TextEditor);

        /// <summary>
        /// Handle Escape key press event on `TextEditor` textbox.
        /// </summary>
        public void OnTextEditorEscapeKeyPress() => ResetAndDispatchHideTodoEditor();

        /// <summary>
        /// Handle when `TextEditor` textbox, looses focus.
        /// </summary>
        public void OnTextEditorLostFocus() => DispatchUpdateTodoTextIfNotEscaped();

        /// <summary>
        /// Handle both checked/unchecked event on todo item's checkbox.
        /// </summary>
        public void OnTodoItemCheckBoxCheckedOrUnchecked() => DispatchToggleComplete?.Invoke(Id);

        /// <summary>
        /// Handle double click event on Todo item.
        /// </summary>
        public void OnTodoItemDblClick() => DispatchShowTodoEditor?.Invoke(new TodoItem(Id, Text, IsCompleted));

        /// <summary>
        /// Dispatch only if focus lost wasn't caused due to pressing `Esc` key.
        /// </summary>
        private void DispatchUpdateTodoTextIfNotEscaped()
        {
            if (!_isEscaped)
            {
                DispatchUpdateTodoText?.Invoke(Id, TextEditor);
            }
            _isEscaped = false;
        }

        private void ResetAndDispatchHideTodoEditor()
        {
            _isEscaped = true;
            TextEditor = Text;
            DispatchHideTodoEditor?.Invoke(Id);
        }
    }
}
