using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Urunium.Redux.Compose;

namespace Urunium.Redux.Tests.Example
{
    [TestFixture]
    class DivideByProperties
    {
        TodoMvcReducer rootReducer;
        Store<Todo> store;

        [SetUp]
        public void Setup()
        {
            rootReducer = new TodoMvcReducer().AddSubtreeReducer(new TodosReducer())
                                              .AddSubtreeReducer(new VisibilityFilterReducer());
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

        ///<summary>
        ///Undivided reducer
        ///class TodoMvcReducer : IReducer&lt;Todo&gt;
        ///{
        ///    public Todo Apply(Todo previousState, object action)
        ///    {
        ///        switch (action)
        ///        {
        ///            case AddTodo add:
        ///                return new Todo(previousState.VisibilityFilter, previousState.Todos.Concat(new[] {
        ///                    new TodoItem(add.Text, false)
        ///                }).ToList());
        ///            case SetVisibilityFilter setFilter:
        ///                return new Todo(setFilter.Filter, previousState.Todos);
        ///            case ToggleTodo toggleTodo:
        ///                return new Todo(previousState.VisibilityFilter, previousState.Todos.Select((todoItem, i) =>
        ///                {
        ///                    if (toggleTodo.Index == i)
        ///                    {
        ///                        return new TodoItem(todoItem.Text, !todoItem.IsComplete);
        ///                    }
        ///                    return todoItem;
        ///                }).ToList());
        ///        }
        ///        return previousState;
        ///    }
        ///}
        ///</summary>
        class TodoMvcReducer : IReducer<Todo>
        {
            private List<IReducer<Todo>> _subReducers = new List<IReducer<Todo>>();

            public TodoMvcReducer AddSubtreeReducer<TPart>(ISubTreeReducer<Todo, TPart> subTreeReducer)
            {
                _subReducers.Add(new SubTreeToFullTreeAdapter<Todo, TPart>(subTreeReducer));
                return this;
            }

            public Todo Apply(Todo previousState, object action)
            {
                foreach (var subReducer in _subReducers)
                {
                    previousState = subReducer.Apply(previousState, action);
                }
                return previousState;
            }
        }

        class TodosReducer : ISubTreeReducer<Todo, List<TodoItem>>
        {
            public Expression<Func<Todo, List<TodoItem>>> PropertySelector => state => state.Todos;

            public List<TodoItem> Apply(List<TodoItem> previousState, object action)
            {
                switch (action)
                {
                    case AddTodo add:
                        return previousState.Concat(new[] {
                            new TodoItem(add.Text, false)
                        }).ToList();
                    case ToggleTodo toggleTodo:
                        return previousState.Select((todoItem, i) =>
                        {
                            if (toggleTodo.Index == i)
                            {
                                return new TodoItem(todoItem.Text, !todoItem.IsComplete);
                            }
                            return todoItem;
                        }).ToList();
                }

                return previousState;
            }
        }

        class VisibilityFilterReducer : ISubTreeReducer<Todo, Filter>
        {
            public Expression<Func<Todo, Filter>> PropertySelector => state => state.VisibilityFilter;

            public Filter Apply(Filter previousState, object action)
            {
                if (action is SetVisibilityFilter setFilter)
                {
                    return setFilter.Filter;
                }
                return previousState;
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
