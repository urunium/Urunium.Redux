using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Urunium.Redux.Compose;
using Urunium.Redux.Typed;
using Urunium.Redux.Enhance;

namespace Urunium.Redux.Tests.Example
{
    [TestFixture]
    class TodoEnhancerExample
    {
        IReducer<Todo> rootReducer;
        IStore<Todo> store;

        [SetUp]
        public void Setup()
        {
            // Enhance the Todo store to only return todo Items that should be visibile in the View.
            rootReducer = new ReducerComposer<Todo>().AddSubTreeReducer(new TodosReducer())
                                              .AddSubTreeReducer(new VisibilityFilterReducer());
            store = new Store<Todo>(rootReducer, new Todo(Filter.ShowAll, new List<TodoItem>()))
                            .EnhanceWith(typeof(FilterTodoItems));
        }

        [Test]
        public void FilterCompletedTodoItems()
        {
            store.Dispatch(new AddTodo("Sleep"));
            store.Dispatch(new AddTodo("Eat"));
            store.Dispatch(new AddTodo("Drink"));
            store.Dispatch(new SetVisibilityFilter(Filter.Completed));
            // Since all todo Item is inprogress state.
            Assert.AreEqual(0, store.State.Todos.Count);

            store.Dispatch(new ToggleTodo(0));
            // Since one of the item is marked completed.
            Assert.AreEqual(1, store.State.Todos.Count);
        }

        [Test]
        public void FilterInProgressTodoItems()
        {
            store.Dispatch(new AddTodo("Sleep"));
            store.Dispatch(new AddTodo("Eat"));
            store.Dispatch(new AddTodo("Drink"));
            store.Dispatch(new SetVisibilityFilter(Filter.InProgress));
            // Since all todo Item is inprogress state.
            Assert.AreEqual(3, store.State.Todos.Count);

            store.Dispatch(new ToggleTodo(0));
            // Since one of the item is marked completed.
            Assert.AreEqual(2, store.State.Todos.Count);
        }

        class TodosReducer : TypedReducer<List<TodoItem>>,
            IApply<List<TodoItem>, AddTodo>,
            IApply<List<TodoItem>, ToggleTodo>,
            ISubTreeReducer<Todo, List<TodoItem>>
        {
            public Expression<Func<Todo, List<TodoItem>>> PropertySelector => state => state.Todos;

            public List<TodoItem> Apply(List<TodoItem> previousState, AddTodo action)
            {
                return previousState.Concat(new[] {
                            new TodoItem(action.Text, false)
                        }).ToList();
            }

            public List<TodoItem> Apply(List<TodoItem> previousState, ToggleTodo action)
            {
                return previousState.Select((todoItem, i) =>
                {
                    if (action.Index == i)
                    {
                        return new TodoItem(todoItem.Text, !todoItem.IsComplete);
                    }
                    return todoItem;
                }).ToList();
            }
        }

        class VisibilityFilterReducer : TypedReducer<Filter>,
            IApply<Filter, SetVisibilityFilter>,
            ISubTreeReducer<Todo, Filter>
        {
            public Expression<Func<Todo, Filter>> PropertySelector => state => state.VisibilityFilter;

            public Filter Apply(Filter previousState, SetVisibilityFilter action)
            {
                return action.Filter;
            }
        }

        // State:
        class Todo
        {
            public Filter VisibilityFilter { get; }
            public List<TodoItem> Todos { get; }

            public Todo(Filter visibilityFilter, List<TodoItem> todos)
            {
                VisibilityFilter = visibilityFilter;
                Todos = todos;
            }
        }

        class TodoItem
        {
            public string Text { get; }
            public bool IsComplete { get; }
            public TodoItem(string text, bool isComplete)
            {
                Text = text;
                IsComplete = isComplete;
            }
        }

        enum Filter
        {
            ShowAll,
            InProgress,
            Completed
        }

        // Actions:
        class SetVisibilityFilter
        {
            public Filter Filter { get; }
            public SetVisibilityFilter(Filter filter)
            {
                Filter = filter;
            }
        }

        class AddTodo
        {
            public string Text { get; }
            public AddTodo(string text)
            {
                Text = text;
            }
        }

        class ToggleTodo
        {
            public int Index { get; }
            public ToggleTodo(int index)
            {
                Index = index;
            }
        }

        // Store Enhancer:
        class FilterTodoItems : StoreEnhancer<Todo>
        {
            public FilterTodoItems(IStore<Todo> store) : base(store)
            {
            }

            protected override Todo OnState(Func<Todo> forward)
            {
                var fullState = forward();
                return new Todo(fullState.VisibilityFilter, FilterItems(fullState));
            }

            private List<TodoItem> FilterItems(Todo state)
            {
                switch (state.VisibilityFilter)
                {
                    case Filter.Completed:
                        return state.Todos.Where(x => x.IsComplete).ToList();
                    case Filter.InProgress:
                        return state.Todos.Where(x => !x.IsComplete).ToList();
                    default:
                        return state.Todos;
                }
            }
        }
    }
}
