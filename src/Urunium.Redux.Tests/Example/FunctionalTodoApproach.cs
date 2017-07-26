using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Urunium.Redux.Compose;
using Urunium.Redux.Typed;

namespace Urunium.Redux.Tests.Example
{
    [TestFixture]
    class FunctionalTodoApproach
    {
        TodoMvcReducer rootReducer;
        Store<Todo> store;

        [SetUp]
        public void Setup()
        {
            rootReducer = new TodoMvcReducer();
            store = new Store<Todo>(rootReducer, new Todo(Filter.ShowAll, new List<TodoItem>()));
        }

        [Test]
        public void AddTodoItemTest()
        {
            store.Dispatch(new AddTodo("Sleep"));
            Assert.AreEqual(1, store.State.Todos.Count);
            Assert.AreEqual("Sleep", store.State.Todos[0].Text);
            Assert.AreEqual(false, store.State.Todos[0].IsComplete);
        }

        [Test]
        public void ToggleTodoItemTest()
        {
            store.Dispatch(new AddTodo("Sleep"));
            store.Dispatch(new ToggleTodo(0));
            Assert.AreEqual(1, store.State.Todos.Count);
            Assert.AreEqual("Sleep", store.State.Todos[0].Text);
            Assert.AreEqual(true, store.State.Todos[0].IsComplete);
        }

        [Test]
        public void SetVisibilityFilterTest()
        {
            store.Dispatch(new SetVisibilityFilter(Filter.Active));
            Assert.AreEqual(Filter.Active, store.State.VisibilityFilter);
        }

        class TodoMvcReducer : IReducer<Todo>
        {
            private List<IReducer<Todo>> _subReducers = new List<IReducer<Todo>>();

            public Todo Apply(Todo previousState, object action)
            {
                return new Todo(
                    visibilityFilter: VisibilityFilter(previousState.VisibilityFilter, action),
                    todos: TodoItems(previousState.Todos, action)
                    );
            }

            Filter VisibilityFilter(Filter previousState, object action)
            {
                if (action is SetVisibilityFilter setVisibility)
                {
                    return setVisibility.Filter;
                }
                return previousState;
            }
            
            List<TodoItem> TodoItems(List<TodoItem> previousState, object action)
            {
                if (action is AddTodo addTodo)
                    return AddTodoItem(previousState, addTodo);
                else if (action is ToggleTodo toggleTodo)
                    return ToggleTodoItem(previousState, toggleTodo);
                return previousState;
            }

            List<TodoItem> AddTodoItem(List<TodoItem> previousState, AddTodo action)
            {
                return previousState.Concat(new[] {
                            new TodoItem(action.Text, false)
                        }).ToList();
            }

            List<TodoItem> ToggleTodoItem(List<TodoItem> previousState, ToggleTodo action)
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
            Active,
            Inactive
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
    }
}
