# Introducing Urunium.Redux an implimentation of Redux in `c#`
**[Urunium.Redux](https://github.com/urunium/Urunium.Redux)** is an implementation of [redux](https://github.com/reactjs/redux) in c#. 
It is a [netstandard](https://docs.microsoft.com/en-us/dotnet/standard/net-standard)-[1.0](https://github.com/dotnet/standard/blob/master/docs/versions/netstandard1.0.md) library, 
hence can be used in multiple platforms with dotnet-core, mono etc. *Though only dotnet-core is tested.*

## Redux
Redux is a predictable state container that first appeared in [JavaScript land](http://redux.js.org/). Urunium.Redux is an opinionated Redux implementation for .net core 
written in c#. This implementation is more geared towards being c# OOP oriented instead of functional oriented, while still adhering to Redux core 
principles. As such the programing models and APIs used would be more familier to a c# developer.

At it's core redux is an implementation of Flux model which advocates one way data flow, to make application state highly predictable. Following gif 
does summarize how data flows in redux:

<div style="text-align:center">
    <img src="https://raw.githubusercontent.com/urunium/Urunium.Redux/master/resources/redux.gif" alt="How Redux Works" title="How Redux Works" width="500" />
</div>

*Image credit: [simple-redux](https://bumbu.github.io/simple-redux/)*

Although there is a small issue with this image. Redux as a concept doesn't have "Dispatcher", redux has "Dispatch" api exposed through Store object.

**Following are the parts of Redux:**

### Store
Store in redux is a global singleton that holds the latest state of the application. Unlike original flux implementation redux proposes use of single
object tree to represent whole of application's state instead of using multiple stores. In Urunium.Redux, [store](http://uruniumredux.readthedocs.io/en/latest/Urunium.Redux/#class-uruniumreduxstoretstate) implements [IStore&lt;TState&gt;](http://uruniumredux.readthedocs.io/en/latest/Urunium.Redux/#interface-uruniumreduxistoretstate) interface,
where `TState` is the class representing state.

**Usage**

```c#
var rootReducer = new ReducerComposer();
var initialState = new AppState();
var store = new Store<AppState>(rootReducer, initialState);
```

### State
State object holds the current state of application. The state mainly represents the current status of UI and also holds few data helping in business 
logic. In Urunium.Redux state can be any type: a class, struct or primitive datatype. There is just one rule that state must follow: **Immutability**.
A state object will never change it's properties. Every time we need to change something a new instance of state must be created, and full old state 
must be replaced by this new object.

### Reducers
Reducers are pure function whose name comes from "reduce()" method of functional programming (as in map/reduce). In Urunium.Redux reducer is a class
that implements [IReducer&lt;TState&gt;](http://uruniumredux.readthedocs.io/en/latest/Urunium.Redux/#interface-uruniumreduxireducertstate) interface.
It has only one method named `Apply()`. The signature of `Apply` method is as follows:

```c#
//IReducer<TState>.Apply:
TState Apply (TState previousState, Object action)
```
Here Apply must be a "pure" function, i.e. it should always return same result for same input parameters, any number of time it is invoked.

Sample:

```c#
class IncrementAction { }
class DecrementAction { }
class Counter : IReducer<int>
{
    public int Apply(int previousState, object action)
    {
        switch (action)
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
```

#### Root Reducer
An application using Urunium.Redux can divide it's reducer into multiple classes to achieve higher maintainability. But all these reducers must 
ultimately be composed into as single reducer. The root reducer works by broadcasting any action it receives to all the registered sub-reducers.
Which ever sub-reducer can handle action will handle it, or return `previousState` as-is if it cannot handle the action.

Sample:

```c#
// A quick example for implementing own root-reducer composed of sub-reducers.

class IncrementAction { }
class DecrementAction { }
class Counter : IReducer<int>
{
	private List<IReducer<int>> _subReducers = new List<IReducer<int>> { new IncrementReducer(), new DecrementReducer() };

    public int Apply(int previousState, object action)
    {
		// Broadcast
		foreach(var reducer in _subReducers)
		{
			previousState = reducer.Apply(previousState, action);
		}

		return previousState;
    }
}
class IncrementReducer : IReducer<int>
{
    public int Apply(int previousState, object action)
    {
        if (action is IncrementAction)
        {
			return previousState + 1;
        }
        // Unsupported actions should return previousState unchanged.
        return previousState;
    }
}
class DecrementReducer : IReducer<int>
{
    public int Apply(int previousState, object action)
    {
        if (action is DecrementAction)
        {
			return previousState - 1;
        }
        // Unsupported actions should return previousState unchanged.
        return previousState;
    }
}
```

#### ReducerComposer
[ReducerComposer&lt;TState&gt;](http://uruniumredux.readthedocs.io/en/latest/Urunium.Redux/#class-uruniumreduxcomposereducercomposertstate) in 
Urunium.Redux is a special class meant to be used as the "Root Reducer". It helps in segregating our reducers by action or property 
(of application state). To divide reducers by action use:
`Urunium.Redux.Compose.ReducerComposer<TState> AddStateReducer (Urunium.Redux.IReducer<TState> stateReducer)` method. And to divide reducers
by application state's property use:
`Urunium.Redux.Compose.ReducerComposer<TState> AddSubTreeReducer <TPart>(Urunium.Redux.Compose.ISubTreeReducer<TState, TPart> subTreeReducer)`
method.

#### TypedReducer
[TypedReducer](http://uruniumredux.readthedocs.io/en/latest/Urunium.Redux/#class-uruniumreduxtypedtypedreducertstate) in Urunium.Redux makes it
easy to write strongly typed reducers. Instead of dealing with `object` actions, we can create reducer that deals with specific type. It is somewhat
like dividing reducer by action, but instead dividing into classes, we divided into functions.

Sample:

```c#
public class Counter : Typed.TypedReducer<int>
{
   public int Apply(int previousState, Increment action)
   {
       return previousState + 1;
   }

   public int Apply(int previousState, Decrement action)
   {
       return previousState - 1;
   }
}
```
We can also **optionally** implement [IApply](http://uruniumredux.readthedocs.io/en/latest/Urunium.Redux/#interface-uruniumreduxtypediapplytstate-taction) 
interfaces in typed reducers e.g.

```c#
public class Counter : Typed.TypedReducer<int>, IApply<int, Increment>, IApply<int, Decrement>
{
   public int Apply(int previousState, Increment action)
   {
       return previousState + 1;
   }

   public int Apply(int previousState, Decrement action)
   {
       return previousState - 1;
   }
}
```

#### Sub-tree reducer
A sub-tree reducer is any reducer that implements [ISubTreeReducer](http://uruniumredux.readthedocs.io/en/latest/Urunium.Redux/#interface-uruniumreduxcomposeisubtreereducertstate-tpart) interface.
These reducers works with a property of application's state instead of working with full state. [ReducerComposer&lt;TState&gt;](http://uruniumredux.readthedocs.io/en/latest/Urunium.Redux/#class-uruniumreduxcomposereducercomposertstate)
can be used to compose multiple sub-tree reducers into single root-reducer. A special helper class [SubTreeToFullTreeAdapter](http://uruniumredux.readthedocs.io/en/latest/Urunium.Redux/#class-uruniumreduxcomposesubtreetofulltreeadaptertstate-tpart)
is internally used by `ReducerComposer` to translate the effect of sub-tree reducer into the application's main state. If any one wants to write
custom reducer-composer this may be helpful.

Sample:

Following sample shows usage of sub-tree reducer, typed reducer and reducer-composer.

```c#
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

// Reducers:
class TodosReducer : TypedReducer<List<TodoItem>>, ISubTreeReducer<Todo, List<TodoItem>>
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

class VisibilityFilterReducer : TypedReducer<Filter>, ISubTreeReducer<Todo, Filter>
{
    public Expression<Func<Todo, Filter>> PropertySelector => state => state.VisibilityFilter;

    public Filter Apply(Filter previousState, SetVisibilityFilter action)
    {
        return action.Filter;
    }
}

// Usage:
var rootReducer = new ReducerComposer<Todo>().AddSubTreeReducer(new TodosReducer()).AddSubTreeReducer(new VisibilityFilterReducer());
var initialState = new Todo(Filter.All, new List<TodoItem>());
var store = new Store<Todo>(rootReducer, initialState);
```

### Actions
Actions are object telling reducer what needs to be done to current state. Reducer decides operation to be performed on state based on the type of 
action that is dispatched. Optionally actions may also contain payloads that reducer can used to make further decission. In Urunium.Redux action can
be any object including primitive type. In general it is good idea to make actions immutable too.
