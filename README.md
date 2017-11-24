# Urunium.Redux
[![Build Status](https://travis-ci.org/urunium/Urunium.Redux.svg?branch=master)](https://travis-ci.org/urunium/Urunium.Redux) [![NuGet version](https://badge.fury.io/nu/Urunium.Redux.svg)](https://badge.fury.io/nu/Urunium.Redux)

Redux is a predictable state container that first appeared in [JavaScript land](http://redux.js.org/). `Urunium.Redux` is an opinionated Redux implementation for .net core written in c#. This implementation is more geared towards being c# OOP oriented instead of functional oriented, while still adhering to Redux core principles. As such the programing models and APIs used would be more familier to a c# developer.

### The Gist
Redux is an implementation of Flux model which advocates one way data flow, to make application state highly predictable. Redux constitute of 4 basic components, namely:
1. State : An object representing state of the application. The state stores mostly UI state, but can also contain few business logic related stuffs and other helpers/calculated state.
2. Action : An object representing action that can be applied to state which changes the state.
3. Reducer : These are pure functions (i.e. no side effects!) that applies a give action to current state and returns next state. In case of `Urunium.Redux` reducer is a class that implements `IReducer` interface, but this class just serves as the container for the reducer function, and shouldn't contain any state of it's own.
4. Store : Is the glue that holds all other components of Redux together. It holds the latest state, routes actions to reducers and notifies changes in state.

If you are not aware about basic redux concepts then it is highly recommended to visit the [original documentation](https://redux.js.org/#the-gist).

<div style="text-align:center">
    <img src="./resources/redux.gif" alt="How Redux Works" title="How Redux Works" width="500" />
</div>

## Satus
Core redux implementation is feature complete. I'll be adding few bells and whistles to it for sure; but it is mostly usable.

## Getting Started
To get started with urunium.redux install it from nuget.

```
Install-Package urunium.redux
```
Once the urunium.redux library is installed, now you are ready to use it.

The counter shown in the original example can be written as:

```c#
class IncrementAction {}
class DecrementAction {}

class Counter : IReducer<int>
{
	public int Apply(int previousState, object action)
	{
		switch(action)
		{
			case IncrementAction inc:
				return previousState + 1;
			case DecrementAction dec:
				return previousState - 1;
		}
        // Unsupported actions should return previousState unchanged.
		return previousState;
	}
}

var store = new Store<int>(new Counter(), 0);

store.StateChanged += (sender, eventArgs) => 
{
	// update ui
};

store.Dispatch(new IncrementAction());
// 1
store.Dispatch(new IncrementAction());
// 2
store.Dispatch(new DecrementAction());
// 1
```

Let's try with a bit more complex state tree example. **First** create classes for our application's state.

```c#
class Todo
{
    public Filter VisibilityFilter { get; }
    public ImmutableList<TodoItem> Todos { get; }

    public Todo(Filter visibilityFilter, ImmutableList<TodoItem> todos)
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
```

Here the class `Todo` is our state tree, and remaining classes are there to constitute the part of the state tree. The point to be noted here is: *these classes are immutable*. 
Each of these state classes provide constructor that populates the properties, and all the properties are readonly.

**The second step** is to create classes for all the supported **actions** that can be applied to the state tree.

```c#
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
```
Notice that these classes are immutable too!

**Third** create reducers. In case of urunium.redux, we have a class named `ReducerComposer` which we can use for root-reducer, so we don't have to write root reducer. We will be writing property reducers and composing them into root reducer:

```c#
// reducer for Todos property
class TodosReducer : ISubTreeReducer<Todo, ImmutableList<TodoItem>>
{
    public Expression<Func<Todo, ImmutableList<TodoItem>>> PropertySelector => state => state.Todos;

    public ImmutableList<TodoItem> Apply(ImmutableList<TodoItem> previousState, object action)
    {
        switch (action)
        {
            case AddTodo add:
                return previousState.Add(new TodoItem(add.Text, false));
            case ToggleTodo toggleTodo:
                return previousState.Select((todoItem, i) =>
                {
                    if (toggleTodo.Index == i)
                    {
                        return new TodoItem(todoItem.Text, !todoItem.IsComplete);
                    }
                    return todoItem;
                }).ToImmutableList();
        }

        return previousState;
    }
}

// reducer for VisibilityFilter property
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
```
Now, to compose these property reducer classes into root-reducer:

```c#
var rootReducer = new ReducerComposer<Todo>();
rootReducer.AddSubtreeReducer(new TodosReducer()).AddSubtreeReducer(new VisibilityFilterReducer());
```
After we have a root reducer, we need to create a **store**. As follows:

```c#
var initialState = new Todo(Filter.All, ImmutableList<TodoItem>.Create(new TodoItem[0]));
var store = new Store<Todo>(rootReducer, initialState);
```

**Finally** we are done with organizing our code. Now we can begin dispatching actions against the store to get our reducers to work, e.g.:

```c#
store.Dispatch(new AddTodo("Sleep"));
store.Dispatch(new AddTodo("Eat"));
```
The change in state can be subscribed through `StateChanged` event of store object.

```c#
store.StateChanged += (sender, eventArgs) => {
	// handle changes in state here...
};
```

### Where to go from here?

[An example](https://github.com/urunium/Urunium.Redux/tree/master/examples/WpfTodoMvcExample) is availabe in the repository showing usage in TodoMVC app written in WPF. Includes all basic usage of redux along with redux-logic.

## Prior Art
Other alternatives are also available:
- https://github.com/GuillaumeSalles/redux.NET
- https://github.com/pshomov/reducto

### Why new implementation?
Well, it's an learning opportunity for me. This implementation is more object-oriented and idiomatic C#, while existing implementations tends to be more functional. Reducto even tries stay close to javascript redux in terms of API. It is also more opinionated in the sense that it includes built in plugins similar to [redux-logic](https://github.com/jeffbski/redux-logic), [redux-undo](https://github.com/omnidan/redux-undo) etc.

## Documentation
Basic usage has been provided above. Few insight to implementation details is also given in the wiki page.
Please visit the wiki https://github.com/urunium/Urunium.Redux/wiki.

API documentation comming soon!

## License
Urunium.Redux is provided under very flexible [MIT license](https://github.com/urunium/Urunium.Redux/blob/master/LICENSE).

## For contributors:
Anyone is most welcome to contribute to this project with your fixes and ideas. Please send a PR and I'll happly accept it ;) or post issues if you find some bug, have questions or think some feature would be cool to use.

### Getting started
Clone this repository from 

```
https://github.com/urunium/Urunium.Redux.git
```
And start poking the code!

### Prerequisite
- Visual studio 2017 (VS 2015 won't work!)

### Building
Build project/solution from visual studio. Or in command line enter following command `dotnet build`.

## Testing
Execute test in visual studio from "Test Explorer" window. Or in command line enter following command `dotnet test Urunium.Redux.Tests`
