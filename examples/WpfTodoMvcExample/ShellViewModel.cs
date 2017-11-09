using Caliburn.Micro;
using System.Collections.Generic;
using System.Linq;
using Urunium.Redux;
using WpfTodoMvcExample.Actions;
using WpfTodoMvcExample.Fx;
using WpfTodoMvcExample.States;

namespace WpfTodoMvcExample
{
    /// <summary>
    /// Marker interface as suggested in various blog posts.
    /// </summary>
    public interface IShell
    {
    }

    /// <summary>
    /// Viewmodel class for ShellView.
    /// </summary>
    public class ShellViewModel : PropertyChangedBase, IShell
    {
        /// <summary>
        /// List of Todo items along with various functionalities.
        /// </summary>
        public TodoListAppViewModel TodoListApp { get; }

        private IStore<TodoList> _store;

        /// <summary>
        /// Create a new instance of ShellViewModel.
        /// </summary>
        /// <param name="store"></param>
        public ShellViewModel(IStore<TodoList> store)
        {
            _store = store;
            TodoListApp = Connect(_store, new TodoListAppViewModel());
        }

        private TodoListAppViewModel Connect(IStore<TodoList> store, TodoListAppViewModel connectible)
        {
            return store.Connect<TodoList, TodoListAppViewModel>(
                mapStateToProps: (state, props) =>
                {
                    props.CanClickShowCompleted = state.Filter != VisibilityFilter.ShowCompletedItems;
                    props.CanClickShowActive = state.Filter != VisibilityFilter.ShowActiveItems;
                    props.CanClickShowAll = state.Filter != VisibilityFilter.ShowAllItems;
                    props.HasTodoItems = state.Todos.Any();
                    props.ClearButtonIsVisible = state.Todos.Any(x => x.Value.Completed);
                    props.CheckToggleAll = !state.Todos.Any(x => x.Value.Completed == false);
                    props.ItemsLeft = state.Todos.Count(x => x.Value.Completed == false);
                    props.NewTodo = "";

                    var visibleTodos = GetFilteredTodos(state.Todos, state.EditingTodos, state.Filter);

                    props.Todos.Patch(visibleTodos);
                },
                mapDispatchToProps: (dispatcher, state, props) =>
                {
                    props.DispatchAdd = (newTodo) =>
                    {
                        int id = state.Todos.Values.Select(x => x.Id).DefaultIfEmpty(0).Max() + 1;
                        dispatcher.DispatchAsync(new AddTodo(new TodoItem(id, newTodo, false)));
                    };
                    props.DispatchClearCompleted = () => dispatcher.DispatchAsync(new RemoveCompleted());
                    props.DispatchCompleteAll = () =>
                    {
                        if (state.Todos.Any(x => !x.Value.Completed))
                        {
                            dispatcher.DispatchAsync(new CompleteAllTodos());
                        }
                    };
                    props.DispatchShowActive = () => dispatcher.DispatchAsync(new ChangeVisibilityFilter(VisibilityFilter.ShowActiveItems));
                    props.DispatchShowAll = () => dispatcher.DispatchAsync(new ChangeVisibilityFilter(VisibilityFilter.ShowAllItems));
                    props.DispatchShowCompleted = () => dispatcher.DispatchAsync(new ChangeVisibilityFilter(VisibilityFilter.ShowCompletedItems));
                    props.DispatchActivateAll = () =>
                    {
                        if (state.Todos.All(x => x.Value.Completed))
                        {
                            dispatcher.DispatchAsync(new MarkAllTodosAsActive());
                        }
                    };

                    foreach (var todoItemProps in props.Todos)
                    {
                        todoItemProps.DispatchShowTodoEditor = (todoItem) => dispatcher.DispatchAsync(new ShowTodoEditor(todoItem));
                        todoItemProps.DispatchHideTodoEditor = (id) => dispatcher.DispatchAsync(new HideTodoEditor(id));
                        todoItemProps.DispatchToggleComplete = (id) => dispatcher.DispatchAsync(new ToggleComplete(id));
                        todoItemProps.DispatchUpdateTodoText = (id, text) => dispatcher.DispatchAsync(new UpdateTodoText(id, text));
                        todoItemProps.DispatchRemoveTodo = (id) => dispatcher.DispatchAsync(new RemoveTodo(id));
                    }
                })(connectible);
        }

        private IEnumerable<TodoItemViewModel> GetFilteredTodos(IReadOnlyDictionary<int, TodoItem> todos, IReadOnlyDictionary<int, TodoItem> editingTodos, VisibilityFilter filter)
        {
            IEnumerable<TodoItem> visibleTodos = Enumerable.Empty<TodoItem>();

            if (filter == VisibilityFilter.ShowAllItems)
                visibleTodos = todos.Select(x => x.Value);
            if (filter == VisibilityFilter.ShowCompletedItems)
                visibleTodos = todos.Where(x => x.Value.Completed).Select(x => x.Value);
            if (filter == VisibilityFilter.ShowActiveItems)
                visibleTodos = todos.Where(x => !x.Value.Completed).Select(x => x.Value);

            return visibleTodos.Select(todo => new TodoItemViewModel()
            {
                Id = todo.Id,
                Text = todo.Text,
                TextEditor = todo.Text,
                IsCompleted = todo.Completed,
                TextEditorIsVisible = editingTodos.ContainsKey(todo.Id)
            });
        }
    }
}
