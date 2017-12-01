<a id="Urunium.Redux.IReducer`1"></a>
## interface Urunium.Redux.IReducer`1

Reducer takes in a state, applies an action and returns resultant state.
Action can be any object that is supported by reducer, there isn't any hard
and fast rule for action. Could even be primitive types.

**Examples**


```csharp
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


**Methods**

<a id="Urunium.Redux.IReducer`1.Apply(,System.Object)"></a>

* *TState* **Apply** *(TState previousState, Object action)*  




---

<a id="Urunium.Redux.IStore`1"></a>
## interface Urunium.Redux.IStore`1

Store is responsible for holding state. Dispatch action to reducer, 
and notify about change in state to subscribers. Default implementation 
for this interface is [Store\`1](#Urunium.Redux.Store`1) .

**Methods**

<a id="Urunium.Redux.IStore`1.Dispatch()"></a>

* *void* **Dispatch** *(TAction action)*  


**Events**

<a id="Urunium.Redux.IStore`1.StateChanged"></a>

* **StateChanged**  
  Can be subscribed to get notified about change in state.  



**Properties and Fields**

<a id="Urunium.Redux.IStore`1.State"></a>

* *TState* **State**  
  Application's State object which needs to be held by this store.  






---

<a id="Urunium.Redux.Store`1"></a>
## class Urunium.Redux.Store`1

Default implementation of [IStore\`1](#Urunium.Redux.IStore`1) .

**Examples**


```csharp
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
// Using Reducer in store.
var rootReducer = new Counter();
var initialState = 0;
var store = new Store<int>(rootReducer, initialState);
store.StateChanged += (sender, eventArgs) => 
{
    // update ui
    Console.WriteLine(store.State);
};
    
store.Dispatch(new IncrementAction());
// 1
store.Dispatch(new IncrementAction());
// 2
store.Dispatch(new DecrementAction());
// 1
```


**Constructors**

<a id="Urunium.Redux.Store`1..ctor"></a>

* **Store`1** *(Urunium.Redux.IReducer&lt;TState&gt; rootReducer, TState initialState)*  



**Methods**

<a id="Urunium.Redux.Store`1.Dispatch()"></a>

* *void* **Dispatch** *(TAction action)*  


**Events**

<a id="Urunium.Redux.Store`1.StateChanged"></a>

* **StateChanged**  
  Can be subscribed to get notified about change in state.  



**Properties and Fields**

<a id="Urunium.Redux.Store`1.State"></a>

* *TState* **State**  
  Get application's State object which needs to be held by this store.  






---

<a id="Urunium.Redux.Undoable.Redo"></a>
## class Urunium.Redux.Undoable.Redo

Action to "Redo" state, as in (Undo/redo).

**Constructors**

<a id="Urunium.Redux.Undoable.Redo..ctor"></a>

* **Redo** *()*  





---

<a id="Urunium.Redux.Undoable.Undo"></a>
## class Urunium.Redux.Undoable.Undo

Action to "Undo" state, as in (Undo/redo).

**Constructors**

<a id="Urunium.Redux.Undoable.Undo..ctor"></a>

* **Undo** *()*  





---

<a id="Urunium.Redux.Undoable.UndoableState`1"></a>
## class Urunium.Redux.Undoable.UndoableState`1

Datastructure to support undo/redo for state. Use this in conjunction with [UndoableReducer\`1](#Urunium.Redux.Undoable.UndoableReducer`1) to support [Undo](#Urunium.Redux.Undoable.Undo) / [Redo](#Urunium.Redux.Undoable.Redo) actions. 
The store instance must be [IStore\`1](#Urunium.Redux.IStore`1) of [UndoableState\`1](#Urunium.Redux.Undoable.UndoableState`1) where TState 
is the type of application's state.

**Constructors**

<a id="Urunium.Redux.Undoable.UndoableState`1..ctor"></a>

* **UndoableState`1** *(TState present, IReadOnlyList&lt;TState&gt; past, IReadOnlyList&lt;TState&gt; future)*  


<a id="Urunium.Redux.Undoable.UndoableState`1..ctor"></a>

* **UndoableState`1** *(TState present)*  



**Properties and Fields**

<a id="Urunium.Redux.Undoable.UndoableState`1.Past"></a>

* *IReadOnlyList&lt;TState&gt;* **Past**  
  List of past states  


<a id="Urunium.Redux.Undoable.UndoableState`1.Future"></a>

* *IReadOnlyList&lt;TState&gt;* **Future**  
  List of future states  


<a id="Urunium.Redux.Undoable.UndoableState`1.Present"></a>

* *TState* **Present**  
  Current state.  






---

<a id="Urunium.Redux.Undoable.UndoableReducer`1"></a>
## class Urunium.Redux.Undoable.UndoableReducer`1

Reducer to support undo/redo. This reducer is an enhancer for actual [IReducer\`1](#Urunium.Redux.IReducer`1) , 
which handles [Undo](#Urunium.Redux.Undoable.Undo) / [Redo](#Urunium.Redux.Undoable.Redo) actions and forwards all other action to underlying
reducer. Use this in conjunction with [UndoableState\`1](#Urunium.Redux.Undoable.UndoableState`1) to support [Undo](#Urunium.Redux.Undoable.Undo) / [Redo](#Urunium.Redux.Undoable.Redo) actions.

**Examples**


```csharp
// Root reducer.
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

// usage of UndoableState and UndoableReducer...
IStore<UndoableState<int>> store = new Store<UndoableState<int>>(
new UndoableReducer<int>(new Counter()),
new UndoableState<int>(0));
```


**Constructors**

<a id="Urunium.Redux.Undoable.UndoableReducer`1..ctor"></a>

* **UndoableReducer`1** *(Urunium.Redux.IReducer&lt;TState&gt; innerReducer, [int keep])*  



**Methods**

<a id="Urunium.Redux.Undoable.UndoableReducer`1.Apply(,System.Object)"></a>

* *Urunium.Redux.Undoable.UndoableState&lt;TState&gt;* **Apply** *(Urunium.Redux.Undoable.UndoableState&lt;TState&gt; previousState, Object action)*  




---

<a id="Urunium.Redux.Typed.IApply`2"></a>
## interface Urunium.Redux.Typed.IApply`2

Helper interface to be used with [TypedReducer\`1](#Urunium.Redux.Typed.TypedReducer`1) . Note that [TypedReducer\`1](#Urunium.Redux.Typed.TypedReducer`1) does work without implementing [IApply\`2](#Urunium.Redux.Typed.IApply`2) interface, but implementing this interface
helps in making implementation more testable, and in visual studio it also helps in generating template methods.

**Examples**


```csharp
public class Counter : Typed.TypedReducer<int>, IApply<int, Increment>, IApply<int, Decrement>;
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


**Methods**

<a id="Urunium.Redux.Typed.IApply`2.Apply(,)"></a>

* *TState* **Apply** *(TState previousState, TAction action)*  




---

<a id="Urunium.Redux.Typed.TypedReducer`1"></a>
## class Urunium.Redux.Typed.TypedReducer`1

A [TypedReducer\`1](#Urunium.Redux.Typed.TypedReducer`1) is essentially an [IReducer\`1](#Urunium.Redux.IReducer`1) whose Apply method forwards call to other Apply methods with correct signature.
The apply methods are strongly typed, hence provids ability to divide reducer functions by Action type.

**Examples**


```csharp
// Root reducer.
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


**Constructors**

<a id="Urunium.Redux.Typed.TypedReducer`1..ctor"></a>

* **TypedReducer`1** *()*  



**Methods**

<a id="Urunium.Redux.Typed.TypedReducer`1.Apply(,System.Object)"></a>

* *TState* **Apply** *(TState previousState, Object action)*  




---

<a id="Urunium.Redux.Logic.AnyAction"></a>
## class Urunium.Redux.Logic.AnyAction

Utility class for ability to handle any type of action. To be used in conjunction with [LogicBase\`2](#Urunium.Redux.Logic.LogicBase`2) class. To get the instance of action that was
actually dispatched use [ActualAction](#Urunium.Redux.Logic.AnyAction.ActualAction) property.

**Examples**


```csharp
interface IAction
{
    long TimeStamp { get; set; }
}

// An implementation of logic that can handle any action.
class AddTimeStamp<AppState> : LogicBase<AppState, AnyAction>
{
    public override Type CancelType => null; // Nothing can cancel this.

    public override uint Priority => 0; // No priority. Bigger number gets higer priority.

    protected override Task<AnyAction> OnTransfrom(Func<AppState> getState, AnyAction action, CancellationToken cancellationToken)
    {
        if (action.ActualAction is IAction actualAction)
        {
            actualAction.TimeStamp = DateTime.UtcNow.Ticks;
        }
        return base.OnTransfrom(getState, action, cancellationToken);
    }
}
```


**Properties and Fields**

<a id="Urunium.Redux.Logic.AnyAction.ActualAction"></a>

* *Object* **ActualAction**  
  Actual action that was dispatched.  






---

<a id="Urunium.Redux.Logic.IPriority"></a>
## interface Urunium.Redux.Logic.IPriority

Give priority to your Logic. In case two logic handles same action, but sequence in which these must be handled matters 
then set higher priority to logic that must be executed first.

**Properties and Fields**

<a id="Urunium.Redux.Logic.IPriority.Priority"></a>

* *uint* **Priority**  
  Priority of logic. Priority is calculated in descending order. i.e. Greater the number higher the priority  






---

<a id="Urunium.Redux.Logic.ICancelable"></a>
## interface Urunium.Redux.Logic.ICancelable

Make logic cancelable.

**Methods**

<a id="Urunium.Redux.Logic.ICancelable.Cancel()"></a>

* *void* **Cancel** *(TCancel cancelAction)*  


**Properties and Fields**

<a id="Urunium.Redux.Logic.ICancelable.CancelType"></a>

* *Type* **CancelType**  
  Type of action that when dispatched will cancel this process  






---

<a id="Urunium.Redux.Logic.ILogic`2"></a>
## interface Urunium.Redux.Logic.ILogic`2

Interface representing a `Logic`. A class must implement this interface, if it wants to get registered in store as a logic. 
`Logic` here typically means "business logic", a validation, transformation or  some kind of processing.

**Methods**

<a id="Urunium.Redux.Logic.ILogic`2.PreProcess(,)"></a>

* *System.Threading.Tasks.Task&lt;Urunium.Redux.Logic.PreProcessResult&gt;* **PreProcess** *(Urunium.Redux.IStore&lt;TState&gt; store, TAction action)*  

<a id="Urunium.Redux.Logic.ILogic`2.Process(,,Urunium.Redux.Logic.IMultiDispatcher)"></a>

* *System.Threading.Tasks.Task* **Process** *(Func&lt;TState&gt; getState, TAction action, Urunium.Redux.Logic.IMultiDispatcher dispatcher)*  


**Properties and Fields**

<a id="Urunium.Redux.Logic.ILogic`2.IsLongRunning"></a>

* *bool* **IsLongRunning**  
  Indicate that this is a long running process, so other logics must continue while this is running in background 
(i.e. don't wait for it to complete). Long running processes are given separate thread, through use of: [ystem.Threading.Tasks.TaskCreationOptions.LongRunning](#System.Threading.Tasks.TaskCreationOptions.LongRunning) . Implementor doesn't need to concern with running it in separate thread. 
Note:
- [PreProcess](#Urunium.Redux.Logic.ILogic`2.PreProcess(Urunium.Redux.IStore{`0},`1)) cannot be long running.
- Only [Process](#Urunium.Redux.Logic.ILogic`2.Process(System.Func{`0},`1,Urunium.Redux.Logic.IMultiDispatcher)) can be long running.  






---

<a id="Urunium.Redux.Logic.ILogicConfiguration`1"></a>
## interface Urunium.Redux.Logic.ILogicConfiguration`1

Configuration helper for logic extension, to add logic to store.

**Methods**

<a id="Urunium.Redux.Logic.ILogicConfiguration`1.AddLogics()"></a>

* *void* **AddLogics** *(Urunium.Redux.Logic.ILogic`2[TState,TAction][] logics)*  




---

<a id="Urunium.Redux.Logic.IMultiDispatcher"></a>
## interface Urunium.Redux.Logic.IMultiDispatcher

A scope under which `Dispatch` doesn't immediately fire state changed event. State is 
changed silently and one state changed event is fired when scope gets disposed.

**Methods**

<a id="Urunium.Redux.Logic.IMultiDispatcher.Dispatch()"></a>

* *void* **Dispatch** *(TAction action)*  

<a id="Urunium.Redux.Logic.IMultiDispatcher.DispatchImmediate()"></a>

* *void* **DispatchImmediate** *(TAction action)*  

<a id="Urunium.Redux.Logic.IMultiDispatcher.BeginScope"></a>

* *Urunium.Redux.Logic.IMultiDispatcher* **BeginScope** *()*  
  Begin a nested scope of multi-dispatcher.  




---

<a id="Urunium.Redux.Logic.LogicBase`2"></a>
## class Urunium.Redux.Logic.LogicBase`2

Abstract base class for logics. Implementing vanilla [ILogic\`2](#Urunium.Redux.Logic.ILogic`2) works but
leaves more to be desired. LogicBase splits Preprocess into Transformation and Validation.
Gives ability to replace any of Preprocess/Processing steps with custom implementation.

**Methods**

<a id="Urunium.Redux.Logic.LogicBase`2.Cancel()"></a>

* *void* **Cancel** *(TCancel cancelAction)*  


**Properties and Fields**

<a id="Urunium.Redux.Logic.LogicBase`2.CancelType"></a>

* *Type* **CancelType**  
  Type of action that when dispatched will cancel this process  


<a id="Urunium.Redux.Logic.LogicBase`2.Priority"></a>

* *uint* **Priority**  
  Priority of logic. Priority is calculated in descending order. i.e. Greater the number higher the priority  


<a id="Urunium.Redux.Logic.LogicBase`2.IsLongRunning"></a>

* *bool* **IsLongRunning**  
  Indicate that this is a long running process, so other logics must continue while this is running in background 
(i.e. don't wait for it to complete). Long running processes are given separate thread, through use of: [ystem.Threading.Tasks.TaskCreationOptions.LongRunning](#System.Threading.Tasks.TaskCreationOptions.LongRunning) . Implementor doesn't need to concern with running it in separate thread. 
Note:
- [ILogic\`2.PreProcess](#Urunium.Redux.Logic.ILogic`2.PreProcess(Urunium.Redux.IStore{`0},`1)) cannot be long running.
- Only [ILogic\`2.Process](#Urunium.Redux.Logic.ILogic`2.Process(System.Func{`0},`1,Urunium.Redux.Logic.IMultiDispatcher)) can be long running.  






---

<a id="Urunium.Redux.Logic.LogicStoreExtension"></a>
## class Urunium.Redux.Logic.LogicStoreExtension

Extension class to configure Logic in store.

**Static Methods**

<a id="Urunium.Redux.Logic.LogicStoreExtension.ConfigureLogic(,)"></a>

* *Urunium.Redux.IStore&lt;TState&gt;* **ConfigureLogic** *(Urunium.Redux.IStore&lt;TState&gt; originalStore, Action&lt;Urunium.Redux.Logic.ILogicConfiguration&lt;TState&gt;&gt; configurator)*  




---

<a id="Urunium.Redux.Logic.MultiDispatcher"></a>
## class Urunium.Redux.Logic.MultiDispatcher

Factory class to create [IMultiDispatcher](#Urunium.Redux.Logic.IMultiDispatcher) .

**Examples**


```csharp
Example of how multi-dispatcher generally will be used.

using(var dispatcher = MultiDispatcher.Create(store))
{
    dispatcher.DispatchImmediately("Long runing process has began");
    dispatcher.Dispatch("1");
    dispatcher.Dispatch("2");
    dispatcher.Dispatch("3");
}
```


**Static Methods**

<a id="Urunium.Redux.Logic.MultiDispatcher.Create()"></a>

* *Urunium.Redux.Logic.IMultiDispatcher* **Create** *(Urunium.Redux.IStore&lt;TState&gt; store)*  




---

<a id="Urunium.Redux.Logic.PoisonPill"></a>
## class Urunium.Redux.Logic.PoisonPill

All logic must handle this Action type, and cancel it's process when received.
Meaning all the logic currently running can be killed by dispatching PoisionPill at once.
This action is meant to be used in conjunction with Logic.

**Constructors**

<a id="Urunium.Redux.Logic.PoisonPill..ctor"></a>

* **PoisonPill** *(string reason, [bool kill])*  
  Instanciate new [PoisonPill](#Urunium.Redux.Logic.PoisonPill)   



**Properties and Fields**

<a id="Urunium.Redux.Logic.PoisonPill.Reason"></a>

* *string* **Reason**  
  Reason describing why this poison-pill was supplied.  


<a id="Urunium.Redux.Logic.PoisonPill.Kill"></a>

* *bool* **Kill**  
  Decides fatality of poison-pill. Setting [Kill](#Urunium.Redux.Logic.PoisonPill.Kill) to false will just cancel
all task in progress but not kill the system.  






---

<a id="Urunium.Redux.Logic.PreProcessResult"></a>
## class Urunium.Redux.Logic.PreProcessResult

Result of preprocessing logic.

**Constructors**

<a id="Urunium.Redux.Logic.PreProcessResult..ctor"></a>

* **PreProcessResult** *(bool continueToNext, Object action)*  
  Immutable Ctor.  



**Properties and Fields**

<a id="Urunium.Redux.Logic.PreProcessResult.ContinueToNextStep"></a>

* *bool* **ContinueToNextStep**  
  Whether or not continue to execute the chain of logic.  


<a id="Urunium.Redux.Logic.PreProcessResult.Action"></a>

* *Object* **Action**  
  Action that was dispatched  






---

<a id="Urunium.Redux.Logic.ValidationException"></a>
## class Urunium.Redux.Logic.ValidationException
*Extends System.Exception*

Exception representing something is invalid with dispatched action.

**Constructors**

<a id="Urunium.Redux.Logic.ValidationException..ctor"></a>

* **ValidationException** *(string message, IEnumerable&lt;Urunium.Redux.Logic.ValidationDetails&gt; validationDetails)*  


<a id="Urunium.Redux.Logic.ValidationException..ctor"></a>

* **ValidationException** *(string message)*  
  Create new instance of [ValidationException](#Urunium.Redux.Logic.ValidationException)   



**Properties and Fields**

<a id="Urunium.Redux.Logic.ValidationException.ValidationDetails"></a>

* *IEnumerable&lt;Urunium.Redux.Logic.ValidationDetails&gt;* **ValidationDetails**  
  Details explaining what are invalid.  






---

<a id="Urunium.Redux.Logic.ValidationDetails"></a>
## class Urunium.Redux.Logic.ValidationDetails

Details of validation result

**Constructors**

<a id="Urunium.Redux.Logic.ValidationDetails..ctor"></a>

* **ValidationDetails** *(string key, string message)*  
  Immutable Ctor  



**Properties and Fields**

<a id="Urunium.Redux.Logic.ValidationDetails.Key"></a>

* *string* **Key**  
  Associates a validation message with a key. 
Typically Key may be a name of UI (form) field, to which message needs to be associated. 
Recommended convention is to use "*" when validation doesn't associate with any particular field.  


<a id="Urunium.Redux.Logic.ValidationDetails.Message"></a>

* *string* **Message**  
  Explation of the reason for invalid.  






---

<a id="Urunium.Redux.Logic.ValidationResult"></a>
## class Urunium.Redux.Logic.ValidationResult

Validation Result

**Properties and Fields**

<a id="Urunium.Redux.Logic.ValidationResult.IsValid"></a>

* *bool* **IsValid**  
  Use to check if Action was valid or not.  


<a id="Urunium.Redux.Logic.ValidationResult.Error"></a>

* *Urunium.Redux.Logic.ValidationException* **Error**  
  Use to determine what are the reason Action is invalid.
Is set to null if IsValid is true.  




**Static Methods**

<a id="Urunium.Redux.Logic.ValidationResult.Success"></a>

* *Urunium.Redux.Logic.ValidationResult* **Success** *()*  
  If action being dispatched is valid.  

<a id="Urunium.Redux.Logic.ValidationResult.Failure(Urunium.Redux.Logic.ValidationException)"></a>

* *Urunium.Redux.Logic.ValidationResult* **Failure** *(Urunium.Redux.Logic.ValidationException error)*  
  If action being dispatched is invalid.  




---

<a id="Urunium.Redux.Enhance.StoreEnhancer`1"></a>
## class Urunium.Redux.Enhance.StoreEnhancer`1

Gives ability to intercept and enhance store functionalities. This class works in Russian doll model,
each enhancer wrapping an inner enhancer, until the inner most object is instance of [Store\`1](#Urunium.Redux.Store`1) itself.LogicEnhanceris an example of how enhancer can be used.

**Examples**


```csharp
// Store Enhancer, that gives filtered todo items based on VisibilityFilter
// when a accessing State property.
class FilterTodoItems : StoreEnhancer<Todo>
{
    public FilterTodoItems(IStore<Todo> store) : base(store)
    {
    }
    
    // override what gets returned when State property is accessed.
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

// Usage:
IStore<Todo> store = new Store<Todo>(rootReducer, new Todo(Filter.ShowAll, new List<TodoItem>()))
                        .EnhanceWith(typeof(FilterTodoItems));
store.Dispatch(new AddTodo("Sleep"));
store.Dispatch(new AddTodo("Eat"));
store.Dispatch(new AddTodo("Drink"));
store.Dispatch(new SetVisibilityFilter(Filter.Completed));
// Since all todo Item is inprogress state.
Assert.AreEqual(0, store.State.Todos.Count);
store.Dispatch(new ToggleTodo(0));
// Since one of the item is marked completed.
Assert.AreEqual(1, store.State.Todos.Count);
```


**Constructors**

<a id="Urunium.Redux.Enhance.StoreEnhancer`1..ctor"></a>

* **StoreEnhancer`1** *(Urunium.Redux.IStore&lt;TState&gt; store)*  



**Methods**

<a id="Urunium.Redux.Enhance.StoreEnhancer`1.Dispatch()"></a>

* *void* **Dispatch** *(TAction action)*  

<a id="Urunium.Redux.Enhance.StoreEnhancer`1.Find"></a>

* *TEnhancer* **Find** *()*  


**Events**

<a id="Urunium.Redux.Enhance.StoreEnhancer`1.StateChanged"></a>

* **StateChanged**  
  Get notified when state changes, with enhancements.  



**Properties and Fields**

<a id="Urunium.Redux.Enhance.StoreEnhancer`1.State"></a>

* *TState* **State**  
  Enhanced State  






---

<a id="Urunium.Redux.Enhance.StoreExtension"></a>
## class Urunium.Redux.Enhance.StoreExtension

Extension methods for [IStore\`1](#Urunium.Redux.IStore`1) to enhance store instances.

**Static Methods**

<a id="Urunium.Redux.Enhance.StoreExtension.EnhanceWith(,System.Type[])"></a>

* *Urunium.Redux.IStore&lt;TState&gt;* **EnhanceWith** *(Urunium.Redux.IStore&lt;TState&gt; originalStore, Type[] enhancerTypes)*  

<a id="Urunium.Redux.Enhance.StoreExtension.FindEnhancer()"></a>

* *TEnhancer* **FindEnhancer** *(Urunium.Redux.IStore&lt;TState&gt; originalStore)*  




---

<a id="Urunium.Redux.Compose.ISubTreeReducer`2"></a>
## interface Urunium.Redux.Compose.ISubTreeReducer`2

Reducer that works on a part (sub-tree) of application state.
Hence helps in dividing reducers into small reducers, processing independently
smaller part of the state tree.

**Properties and Fields**

<a id="Urunium.Redux.Compose.ISubTreeReducer`2.PropertySelector"></a>

* *System.Linq.Expressions.Expression&lt;Func&lt;TState, TPart&gt;&gt;* **PropertySelector**  
  Expression representing the property of TState class, that this subtree reducer deals with.  






---

<a id="Urunium.Redux.Compose.ReducerComposer`1"></a>
## class Urunium.Redux.Compose.ReducerComposer`1

Root reducer that will compose various reducers.

**Constructors**

<a id="Urunium.Redux.Compose.ReducerComposer`1..ctor"></a>

* **ReducerComposer`1** *()*  



**Methods**

<a id="Urunium.Redux.Compose.ReducerComposer`1.AddStateReducer()"></a>

* *Urunium.Redux.Compose.ReducerComposer&lt;TState&gt;* **AddStateReducer** *(Urunium.Redux.IReducer&lt;TState&gt; stateReducer)*  

<a id="Urunium.Redux.Compose.ReducerComposer`1.AddSubTreeReducer()"></a>

* *Urunium.Redux.Compose.ReducerComposer&lt;TState&gt;* **AddSubTreeReducer** *(Urunium.Redux.Compose.ISubTreeReducer&lt;TState, TPart&gt; subTreeReducer)*  

<a id="Urunium.Redux.Compose.ReducerComposer`1.Apply(,System.Object)"></a>

* *TState* **Apply** *(TState previousState, Object action)*  




---

<a id="Urunium.Redux.Compose.SubTreeToFullTreeAdapter`2"></a>
## class Urunium.Redux.Compose.SubTreeToFullTreeAdapter`2

Helps adapt a subtree reducer into full state tree reducer.

**Constructors**

<a id="Urunium.Redux.Compose.SubTreeToFullTreeAdapter`2..ctor"></a>

* **SubTreeToFullTreeAdapter`2** *(Urunium.Redux.Compose.ISubTreeReducer&lt;TState, TPart&gt; subTreeReducer)*  



**Methods**

<a id="Urunium.Redux.Compose.SubTreeToFullTreeAdapter`2.Apply(,System.Object)"></a>

* *TState* **Apply** *(TState previousState, Object action)*  




---

