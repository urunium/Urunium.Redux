<a id="Urunium.Redux.IReducer`1"></a>
## interface Urunium.Redux.IReducer&lt;TState&gt;

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

<a id="Urunium.Redux.IReducer`1.Apply(`0,System.Object)"></a>
  * *TState* **Apply** *(TState previousState, Object action)*  
       Apply an action to old state and return a new state.
  
    **Returns:** New state
  
    **Parameters:**
     * *TState* **previousState**  
       Existing state

     * *Object* **action**  
       Action that needs to be applied





---

<a id="Urunium.Redux.IStore`1"></a>
## interface Urunium.Redux.IStore&lt;TState&gt;

Store is responsible for holding state. Dispatch action to reducer, 
and notify about change in state to subscribers. Default implementation 
for this interface is [Urunium.Redux.Store&lt;TState&gt;](#Urunium.Redux.Store`1) .

**Methods**

<a id="Urunium.Redux.IStore`1.Dispatch``1(``0)"></a>
  * *void* **Dispatch** *&lt;TAction&gt;(TAction action)*  
       Dispatch action to reducers which will then apply the actions.
Also, notifies about state change by firing StageChanged event.
  
    **Parameters:**
     * *TAction* **action**  
       Instance of `Action` that needs to be applied to current state of Store. 
Applying an action may transform a state into new state.



**Events**

 <a id="Urunium.Redux.IStore`1.StateChanged"></a>

 * *EventHandler&lt;EventArgs&gt;* **StateChanged**  
  Can be subscribed to get notified about change in state.  



**Properties and Fields**

 <a id="Urunium.Redux.IStore`1.State"></a>

 * *TState* **State**  
  Application's State object which needs to be held by this store.  






---

<a id="Urunium.Redux.Store`1"></a>
## class Urunium.Redux.Store&lt;TState&gt;

Default implementation of [Urunium.Redux.IStore&lt;TState&gt;](#Urunium.Redux.IStore`1) . A store is generally a singleton global
object that holds entire state of the application. Store can be used to route actions to reducers,
to change the current state to new desired state using [Dispatch&lt;TAction&gt;(TAction action)](#Urunium.Redux.Store`1.Dispatch``1(``0)) method, 
the change in state can be listened through [StateChanged](#Urunium.Redux.Store`1.StateChanged) event, and the
current state can be accessed through [State](#Urunium.Redux.Store`1.State) property. Note that only
way to modify state is through dispatching an action through store.

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

 * **Urunium.Redux.Store&lt;TState&gt;** *(Urunium.Redux.IReducer&lt;TState&gt; rootReducer, TState initialState)*  
   Store should take in the root reducer, and initial state.
  
    **Parameters:**
     * *IReducer&lt;TState&gt;* **rootReducer**  
       The root reducer is responsible for distributing all the incomming actions to correct reducer.

     * *TState* **initialState**  
       The initial state of application when store is constructed for first time.

  


**Methods**

<a id="Urunium.Redux.Store`1.Dispatch``1(``0)"></a>
  * *void* **Dispatch** *&lt;TAction&gt;(TAction action)*  
       Dispatch action to reducers which will then apply the actions.
Also, notifies about state change by firing StageChanged event.
  
    **Parameters:**
     * *TAction* **action**  
       Instance of `Action` that needs to be applied to current state of Store. 
Applying an action may transform a state into new state.



**Events**

 <a id="Urunium.Redux.Store`1.StateChanged"></a>

 * *EventHandler&lt;EventArgs&gt;* **StateChanged**  
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





---

<a id="Urunium.Redux.Undoable.Undo"></a>
## class Urunium.Redux.Undoable.Undo

Action to "Undo" state, as in (Undo/redo).

**Constructors**





---

<a id="Urunium.Redux.Undoable.UndoableState`1"></a>
## class Urunium.Redux.Undoable.UndoableState&lt;TState&gt;

Datastructure to support undo/redo for state. Use this in conjunction with [Urunium.Redux.Undoable.UndoableReducer&lt;TState&gt;](#Urunium.Redux.Undoable.UndoableReducer`1) to support [Urunium.Redux.Undoable.Undo](#Urunium.Redux.Undoable.Undo) / [Urunium.Redux.Undoable.Redo](#Urunium.Redux.Undoable.Redo) actions. 
The store instance must be [Urunium.Redux.IStore&lt;TState&gt;](#Urunium.Redux.IStore`1) of [Urunium.Redux.Undoable.UndoableState&lt;TState&gt;](#Urunium.Redux.Undoable.UndoableState`1) where TState 
is the type of application's state.

**Constructors**

 <a id="Urunium.Redux.Undoable.UndoableState`1..ctor"></a>

 * **Urunium.Redux.Undoable.UndoableState&lt;TState&gt;** *(TState present, System.Collections.Generic.IReadOnlyList&lt;TState&gt; past, System.Collections.Generic.IReadOnlyList&lt;TState&gt; future)*  
   Create an instance of [Urunium.Redux.Undoable.UndoableState&lt;TState&gt;](#Urunium.Redux.Undoable.UndoableState`1) , with current state, past states and future states.
  
    **Parameters:**
     * *TState* **present**  
       Current state.

     * *IReadOnlyList&lt;TState&gt;* **past**  
       List of past states.

     * *IReadOnlyList&lt;TState&gt;* **future**  
       List of future states.

  

 <a id="Urunium.Redux.Undoable.UndoableState`1..ctor"></a>

 * **Urunium.Redux.Undoable.UndoableState&lt;TState&gt;** *(TState present)*  
   Create an instance of [Urunium.Redux.Undoable.UndoableState&lt;TState&gt;](#Urunium.Redux.Undoable.UndoableState`1) , with just current state.
  
    **Parameters:**
     * *TState* **present**  
       

  


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
## class Urunium.Redux.Undoable.UndoableReducer&lt;TState&gt;

Reducer to support undo/redo. This reducer is an enhancer for actual [Urunium.Redux.IReducer&lt;TState&gt;](#Urunium.Redux.IReducer`1) , 
which handles [Urunium.Redux.Undoable.Undo](#Urunium.Redux.Undoable.Undo) / [Urunium.Redux.Undoable.Redo](#Urunium.Redux.Undoable.Redo) actions and forwards all other action to underlying
reducer. Use this in conjunction with [Urunium.Redux.Undoable.UndoableState&lt;TState&gt;](#Urunium.Redux.Undoable.UndoableState`1) to support [Urunium.Redux.Undoable.Undo](#Urunium.Redux.Undoable.Undo) / [Urunium.Redux.Undoable.Redo](#Urunium.Redux.Undoable.Redo) actions.

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

 * **Urunium.Redux.Undoable.UndoableReducer&lt;TState&gt;** *(Urunium.Redux.IReducer&lt;TState&gt; innerReducer, [int keep])*  
   Create an instance of [Urunium.Redux.Undoable.UndoableReducer&lt;TState&gt;](#Urunium.Redux.Undoable.UndoableReducer`1) .
  
    **Parameters:**
     * *IReducer&lt;TState&gt;* **innerReducer**  
       Reducer which needs to be enhanced with undo/redo ability.

     * *int* **keep** *(optional, default: 10)*  
       Number of historical states to be preserved while supporting undo/redo.

  


**Methods**

<a id="Urunium.Redux.Undoable.UndoableReducer`1.Apply(Urunium.Redux.Undoable.UndoableState{`0},System.Object)"></a>
  * *UndoableState&lt;TState&gt;* **Apply** *(Urunium.Redux.Undoable.UndoableState&lt;TState&gt; previousState, Object action)*  
       Reducer function to support undo/redo.
  
    **Returns:** New state after applying action.
  
    **Parameters:**
     * *UndoableState&lt;TState&gt;* **previousState**  
       Current state stored in [Urunium.Redux.Store&lt;TState&gt;](#Urunium.Redux.Store`1) object.

     * *Object* **action**  
       Action to be applied to state.





---

<a id="Urunium.Redux.Typed.IApply`2"></a>
## interface Urunium.Redux.Typed.IApply&lt;TState, TAction&gt;

Helper interface to be used with [Urunium.Redux.Typed.TypedReducer&lt;TState&gt;](#Urunium.Redux.Typed.TypedReducer`1) . Note that [Urunium.Redux.Typed.TypedReducer&lt;TState&gt;](#Urunium.Redux.Typed.TypedReducer`1) does work without implementing [Urunium.Redux.Typed.IApply&lt;TState, TAction&gt;](#Urunium.Redux.Typed.IApply`2) interface, but implementing this interface
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

<a id="Urunium.Redux.Typed.IApply`2.Apply(`0,`1)"></a>
  * *TState* **Apply** *(TState previousState, TAction action)*  
       Reducer function that applies specific type of TAction to given TState.
  
    **Returns:** New state after applying action.
  
    **Parameters:**
     * *TState* **previousState**  
       The state currently stored in Store

     * *TAction* **action**  
       Action to be applied to state





---

<a id="Urunium.Redux.Typed.TypedReducer`1"></a>
## class Urunium.Redux.Typed.TypedReducer&lt;TState&gt;

A [Urunium.Redux.Typed.TypedReducer&lt;TState&gt;](#Urunium.Redux.Typed.TypedReducer`1) is essentially an [Urunium.Redux.IReducer&lt;TState&gt;](#Urunium.Redux.IReducer`1) whose Apply method forwards call to other Apply methods with correct signature.
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

 * **Urunium.Redux.Typed.TypedReducer&lt;TState&gt;** *()*  
   Instantiate new [Urunium.Redux.Typed.TypedReducer&lt;TState&gt;](#Urunium.Redux.Typed.TypedReducer`1) 
  


**Methods**

<a id="Urunium.Redux.Typed.TypedReducer`1.Apply(`0,System.Object)"></a>
  * *TState* **Apply** *(TState previousState, Object action)*  
       Base Apply method from [Urunium.Redux.IReducer&lt;TState&gt;](#Urunium.Redux.IReducer`1) 
  
    **Parameters:**
     * *TState* **previousState**  
       

     * *Object* **action**  
       





---

<a id="Urunium.Redux.Logic.AnyAction"></a>
## class Urunium.Redux.Logic.AnyAction

Utility class for ability to handle any type of action. To be used in conjunction with [Urunium.Redux.Logic.LogicBase&lt;TState, TAction&gt;](#Urunium.Redux.Logic.LogicBase`2) class. To get the instance of action that was
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

<a id="Urunium.Redux.Logic.ICancelable.Cancel``1(``0)"></a>
  * *void* **Cancel** *&lt;TCancel&gt;(TCancel cancelAction)*  
       Request canceling
  
    **Parameters:**
     * *TCancel* **cancelAction**  
       



**Properties and Fields**

 <a id="Urunium.Redux.Logic.ICancelable.CancelType"></a>

 * *Type* **CancelType**  
  Type of action that when dispatched will cancel this process  






---

<a id="Urunium.Redux.Logic.ILogic`2"></a>
## interface Urunium.Redux.Logic.ILogic&lt;TState, TAction&gt;

Interface representing a `Logic`. A class must implement this interface, if it wants to get registered in store as a logic. 
`Logic` here typically means "business logic", a validation, transformation or  some kind of processing.

**Methods**

<a id="Urunium.Redux.Logic.ILogic`2.PreProcess(Urunium.Redux.IStore{`0},`1)"></a>
  * *System.Threading.Tasks.Task&lt;Urunium.Redux.Logic.PreProcessResult&gt;* **PreProcess** *(Urunium.Redux.IStore&lt;TState&gt; store, TAction action)*  
       Preprocess an action before it is dispatched to store. E.g. Validate action, Transform action etc.
In case multiple logic handles same action type, then preprocess of each logic is executed in order of priority
before dispatching action.
  
    **Returns:** An instance of [Urunium.Redux.Logic.PreProcessResult](#Urunium.Redux.Logic.PreProcessResult) , indicating whether or not next logic in chain should be executed. 
Note that, setting [PreProcessResult.ContinueToNextStep](#Urunium.Redux.Logic.PreProcessResult.ContinueToNextStep) to false will stop logic chaing right there. Even [Process(System.Func&lt;TState&gt; getState, TAction action, Urunium.Redux.Logic.IMultiDispatcher dispatcher)](#Urunium.Redux.Logic.ILogic`2.Process(System.Func{`0},`1,Urunium.Redux.Logic.IMultiDispatcher)) is also not executed
  
    **Parameters:**
     * *IStore&lt;TState&gt;* **store**  
       Store

     * *TAction* **action**  
       Action that needs to be preprocessed.


<a id="Urunium.Redux.Logic.ILogic`2.Process(System.Func{`0},`1,Urunium.Redux.Logic.IMultiDispatcher)"></a>
  * *System.Threading.Tasks.Task* **Process** *(System.Func&lt;TState&gt; getState, TAction action, Urunium.Redux.Logic.IMultiDispatcher dispatcher)*  
       Processing of dispatched action. This gets executed after [PreProcess(Urunium.Redux.IStore&lt;TState&gt; store, TAction action)](#Urunium.Redux.Logic.ILogic`2.PreProcess(Urunium.Redux.IStore{`0},`1)) and, [Dispatch&lt;TAction&gt;(TAction action)](#Urunium.Redux.IStore`1.Dispatch``1(``0)) . Typically pre-processing is to handle scenarios before
dispatching action, and process is for handling after dispatching.
  
    **Returns:** async Task (instead of async void.)
  
    **Parameters:**
     * *Func&lt;TState&gt;* **getState**  
       Function to get current state.

     * *TAction* **action**  
       Action that needs to be processed.

     * *Urunium.Redux.Logic.IMultiDispatcher* **dispatcher**  
       Multi dispatcher.



**Properties and Fields**

 <a id="Urunium.Redux.Logic.ILogic`2.IsLongRunning"></a>

 * *bool* **IsLongRunning**  
  Indicate that this is a long running process, so other logics must continue while this is running in background 
(i.e. don't wait for it to complete). Long running processes are given separate thread, through use of: [System.Threading.Tasks.TaskCreationOptions.LongRunning](#System.Threading.Tasks.TaskCreationOptions.LongRunning) . Implementor doesn't need to concern with running it in separate thread. 
Note:
- [PreProcess(Urunium.Redux.IStore&lt;TState&gt; store, TAction action)](#Urunium.Redux.Logic.ILogic`2.PreProcess(Urunium.Redux.IStore{`0},`1)) cannot be long running.
- Only [Process(System.Func&lt;TState&gt; getState, TAction action, Urunium.Redux.Logic.IMultiDispatcher dispatcher)](#Urunium.Redux.Logic.ILogic`2.Process(System.Func{`0},`1,Urunium.Redux.Logic.IMultiDispatcher)) can be long running.  






---

<a id="Urunium.Redux.Logic.ILogicConfiguration`1"></a>
## interface Urunium.Redux.Logic.ILogicConfiguration&lt;TState&gt;

Configuration helper for logic extension, to add logic to store.

**Methods**

<a id="Urunium.Redux.Logic.ILogicConfiguration`1.AddLogics``1(Urunium.Redux.Logic.ILogic{`0,``0}[])"></a>
  * *void* **AddLogics** *&lt;TAction&gt;(Urunium.Redux.Logic.ILogic&lt;TState, TAction&gt;[] logics)*  
       Add a business logic that will listen to particular action beign dispatched to store.
  
    **Parameters:**
     * *ILogic&lt;TState, TAction&gt;[]* **logics**  
       Array of logics; processing particular action type. Multiple calls are needed to handle different action types.





---

<a id="Urunium.Redux.Logic.IMultiDispatcher"></a>
## interface Urunium.Redux.Logic.IMultiDispatcher

A scope under which `Dispatch` doesn't immediately fire state changed event. State is 
changed silently and one state changed event is fired when scope gets disposed.

**Methods**

<a id="Urunium.Redux.Logic.IMultiDispatcher.Dispatch``1(``0)"></a>
  * *void* **Dispatch** *&lt;TAction&gt;(TAction action)*  
       Dispatch action to store, without firing StateChange event. All state changes are done
silently, and single StateChange event is fired at the end, when scope gets disposed.
  
    **Parameters:**
     * *TAction* **action**  
       Action object to dispatch


<a id="Urunium.Redux.Logic.IMultiDispatcher.DispatchImmediate``1(``0)"></a>
  * *void* **DispatchImmediate** *&lt;TAction&gt;(TAction action)*  
       Dispatch and cause to fire StateChange immediately. Even when under multi-dispatcher
scope, there may be some changes that makes sense to be reflected in UI immediately.
Typically DispatchImmediately is intended to dispatch in-progress actions before actual
dispatches begin.
Warning:
- Dispatching immediately in middle of dispatch sequence may cause UI to render partally correct state.
  
    **Parameters:**
     * *TAction* **action**  
       Action object to dispatch


<a id="Urunium.Redux.Logic.IMultiDispatcher.BeginScope"></a>
  * *Urunium.Redux.Logic.IMultiDispatcher* **BeginScope** *()*  
       Begin a nested scope of multi-dispatcher.




---

<a id="Urunium.Redux.Logic.LogicBase`2"></a>
## class Urunium.Redux.Logic.LogicBase&lt;TState, TAction&gt;

Abstract base class for logics. Implementing vanilla [Urunium.Redux.Logic.ILogic&lt;TState, TAction&gt;](#Urunium.Redux.Logic.ILogic`2) works but
leaves more to be desired. LogicBase splits Preprocess into Transformation and Validation.
Gives ability to replace any of Preprocess/Processing steps with custom implementation.

**Methods**

<a id="Urunium.Redux.Logic.LogicBase`2.Cancel``1(``0)"></a>
  * *void* **Cancel** *&lt;TCancel&gt;(TCancel cancelAction)*  
       Cancel a process
  
    **Parameters:**
     * *TCancel* **cancelAction**  
       Action used to cancel this process.



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
(i.e. don't wait for it to complete). Long running processes are given separate thread, through use of: [System.Threading.Tasks.TaskCreationOptions.LongRunning](#System.Threading.Tasks.TaskCreationOptions.LongRunning) . Implementor doesn't need to concern with running it in separate thread. 
Note:
- [PreProcess(Urunium.Redux.IStore&lt;TState&gt; store, TAction action)](#Urunium.Redux.Logic.ILogic`2.PreProcess(Urunium.Redux.IStore{`0},`1)) cannot be long running.
- Only [Process(System.Func&lt;TState&gt; getState, TAction action, Urunium.Redux.Logic.IMultiDispatcher dispatcher)](#Urunium.Redux.Logic.ILogic`2.Process(System.Func{`0},`1,Urunium.Redux.Logic.IMultiDispatcher)) can be long running.  






---

<a id="Urunium.Redux.Logic.LogicStoreExtension"></a>
## class Urunium.Redux.Logic.LogicStoreExtension

Extension class to configure Logic in store.

**Static Methods**

<a id="Urunium.Redux.Logic.LogicStoreExtension.ConfigureLogic``1(Urunium.Redux.IStore{``0},System.Action{Urunium.Redux.Logic.ILogicConfiguration{``0}})"></a>
  * *IStore&lt;TState&gt;* **ConfigureLogic** *&lt;TState&gt;(Urunium.Redux.IStore&lt;TState&gt; originalStore, System.Action&lt;Urunium.Redux.Logic.ILogicConfiguration&lt;TState&gt;&gt; configurator)*  
       Enhance your store to handle business logics.
  
    **Returns:** Enhanced store, that now can handle business logics.
  
    **Parameters:**
     * *IStore&lt;TState&gt;* **originalStore**  
       Original store that will be enhanced with ability to handle business logic.

     * *Action&lt;ILogicConfiguration&lt;TState&gt;&gt;* **configurator**  
       Instance of [Urunium.Redux.Logic.ILogicConfiguration&lt;TState&gt;](#Urunium.Redux.Logic.ILogicConfiguration`1) .





---

<a id="Urunium.Redux.Logic.MultiDispatcher"></a>
## class Urunium.Redux.Logic.MultiDispatcher

Factory class to create [Urunium.Redux.Logic.IMultiDispatcher](#Urunium.Redux.Logic.IMultiDispatcher) .

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

<a id="Urunium.Redux.Logic.MultiDispatcher.Create``1(Urunium.Redux.IStore{``0})"></a>
  * *Urunium.Redux.Logic.IMultiDispatcher* **Create** *&lt;TState&gt;(Urunium.Redux.IStore&lt;TState&gt; store)*  
       Create an instance of IMultiDispatcher
  
    **Returns:** Instance of [Urunium.Redux.Logic.IMultiDispatcher](#Urunium.Redux.Logic.IMultiDispatcher) 
  
    **Parameters:**
     * *IStore&lt;TState&gt;* **store**  
       Instance of store for which multi-dispatcher needs to be created.





---

<a id="Urunium.Redux.Logic.PoisonPill"></a>
## class Urunium.Redux.Logic.PoisonPill

This action is meant to be used in conjunction with Logic, for a system which needs a way
to gracefully cancel everything things that are in progress.
All logic must handle this Action type, and cancel it's process when received.
Meaning all the logic currently running can be killed by dispatching PoisionPill at once.

**Constructors**

 <a id="Urunium.Redux.Logic.PoisonPill..ctor"></a>

 * **Urunium.Redux.Logic.PoisonPill** *(string reason, [bool kill])*  
   Instanciate new [Urunium.Redux.Logic.PoisonPill](#Urunium.Redux.Logic.PoisonPill) 
  
    **Parameters:**
     * *string* **reason**  
       Reason describing why this poison-pill was supplied.

     * *bool* **kill** *(optional, default: True)*  
       Decides fatality of poison-pill. Setting [Kill](#Urunium.Redux.Logic.PoisonPill.Kill) to false will just cancel all task in progress but not kill the system

  


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

 * **Urunium.Redux.Logic.PreProcessResult** *(bool continueToNext, Object action)*  
   Immutable constructor to instanciate new [Urunium.Redux.Logic.PreProcessResult](#Urunium.Redux.Logic.PreProcessResult) .
  
    **Parameters:**
     * *bool* **continueToNext**  
       

     * *Object* **action**  
       

  


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
*Extends Exception*

Exception representing something is invalid with dispatched action.

**Constructors**


 <a id="Urunium.Redux.Logic.ValidationException..ctor"></a>

 * **Urunium.Redux.Logic.ValidationException** *(string message)*  
   Create new instance of [Urunium.Redux.Logic.ValidationException](#Urunium.Redux.Logic.ValidationException) 
  
    **Parameters:**
     * *string* **message**  
       Exception Message, typically a summary message implying something went wrong.

  


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

 * **Urunium.Redux.Logic.ValidationDetails** *(string key, string message)*  
   Immutable Ctor
  
    **Parameters:**
     * *string* **key**  
       

     * *string* **message**  
       

  


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
  
    **Parameters:**
     * *Urunium.Redux.Logic.ValidationException* **error**  
       





---

<a id="Urunium.Redux.Enhance.StoreEnhancer`1"></a>
## class Urunium.Redux.Enhance.StoreEnhancer&lt;TState&gt;

Gives ability to intercept and enhance store functionalities. This class works in Russian doll model,
each enhancer wrapping an inner enhancer, until the inner most object is instance of [Urunium.Redux.Store&lt;TState&gt;](#Urunium.Redux.Store`1) itself.LogicEnhanceris an example of how enhancer can be used.

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

 * **Urunium.Redux.Enhance.StoreEnhancer&lt;TState&gt;** *(Urunium.Redux.IStore&lt;TState&gt; store)*  
   Constructor of [Urunium.Redux.Enhance.StoreEnhancer&lt;TState&gt;](#Urunium.Redux.Enhance.StoreEnhancer`1) that enhances store object.
  
    **Parameters:**
     * *IStore&lt;TState&gt;* **store**  
        [Urunium.Redux.IStore&lt;TState&gt;](#Urunium.Redux.IStore`1) instance that needs to be enhanced.

  


**Methods**

<a id="Urunium.Redux.Enhance.StoreEnhancer`1.Dispatch``1(``0)"></a>
  * *void* **Dispatch** *&lt;TAction&gt;(TAction action)*  
       Dispatch action to reducers, with enhancements.
  
    **Parameters:**
     * *TAction* **action**  
       


<a id="Urunium.Redux.Enhance.StoreEnhancer`1.Find``1"></a>
  * *TEnhancer* **Find** *&lt;TEnhancer&gt;()*  
       Locate a particular store enhancer, applied to current store.
Note: 
- Search is inwards, i.e while locating, traversal is done from 
outer most enhacer to inner-most store.
- This is implementation detail of IStore extension method :
  
    **Returns:** Enhancer instance if found, or null.


**Events**

 <a id="Urunium.Redux.Enhance.StoreEnhancer`1.StateChanged"></a>

 * *EventHandler&lt;EventArgs&gt;* **StateChanged**  
  Get notified when state changes, with enhancements.  



**Properties and Fields**

 <a id="Urunium.Redux.Enhance.StoreEnhancer`1.State"></a>

 * *TState* **State**  
  Enhanced State  






---

<a id="Urunium.Redux.Enhance.StoreExtension"></a>
## class Urunium.Redux.Enhance.StoreExtension

Extension methods for [Urunium.Redux.IStore&lt;TState&gt;](#Urunium.Redux.IStore`1) to enhance store instances.

**Static Methods**

<a id="Urunium.Redux.Enhance.StoreExtension.EnhanceWith``1(Urunium.Redux.IStore{``0},System.Type[])"></a>
  * *IStore&lt;TState&gt;* **EnhanceWith** *&lt;TState&gt;(Urunium.Redux.IStore&lt;TState&gt; originalStore, Type[] enhancerTypes)*  
       Applies enhancers to given store.
  
    **Returns:** instance of IStore after applying all enhancers.
  
    **Parameters:**
     * *IStore&lt;TState&gt;* **originalStore**  
       IStore instance

     * *Type[]* **enhancerTypes**  
       All store enhancers must inherit from


<a id="Urunium.Redux.Enhance.StoreExtension.FindEnhancer``2(Urunium.Redux.IStore{``1})"></a>
  * *TEnhancer* **FindEnhancer** *&lt;TEnhancer, TState&gt;(Urunium.Redux.IStore&lt;TState&gt; originalStore)*  
       Locate a particular store enhancer applied to current store.
Note: Search is inwards, i.e while locating, traversal is done from 
outer most enhacer to inner-most IStore.
  
    **Returns:** Enhancer instance if found, or null.
  
    **Parameters:**
     * *IStore&lt;TState&gt;* **originalStore**  
       Instance of IStore from which Enhancer is to be located.





---

<a id="Urunium.Redux.Compose.ISubTreeReducer`2"></a>
## interface Urunium.Redux.Compose.ISubTreeReducer&lt;TState, TPart&gt;

Reducer that works on a part (sub-tree) of application state.
Hence helps in dividing reducers into small reducers, processing independently
smaller part of the state tree.

**Properties and Fields**

 <a id="Urunium.Redux.Compose.ISubTreeReducer`2.PropertySelector"></a>

 * *Expression&lt;Func&lt;TState, TPart&gt;&gt;* **PropertySelector**  
  Expression representing the property of TState class, that this subtree reducer deals with.  






---

<a id="Urunium.Redux.Compose.ReducerComposer`1"></a>
## class Urunium.Redux.Compose.ReducerComposer&lt;TState&gt;

Root reducer that will compose various reducers.

**Constructors**



**Methods**

<a id="Urunium.Redux.Compose.ReducerComposer`1.AddStateReducer(Urunium.Redux.IReducer{`0})"></a>
  * *Urunium.Redux.Compose.ReducerComposer&lt;TState&gt;* **AddStateReducer** *(Urunium.Redux.IReducer&lt;TState&gt; stateReducer)*  
       Add a reducer.
  
    **Returns:** ReducerComposer for fluent-api.
  
    **Parameters:**
     * *IReducer&lt;TState&gt;* **stateReducer**  
       


<a id="Urunium.Redux.Compose.ReducerComposer`1.AddSubTreeReducer``1(Urunium.Redux.Compose.ISubTreeReducer{`0,``0})"></a>
  * *Urunium.Redux.Compose.ReducerComposer&lt;TState&gt;* **AddSubTreeReducer** *&lt;TPart&gt;(Urunium.Redux.Compose.ISubTreeReducer&lt;TState, TPart&gt; subTreeReducer)*  
       Add a reducer that works in part/property (sub-tree) of application's state.
  
    **Returns:** ReducerComposer for fluent-api.
  
    **Parameters:**
     * *ISubTreeReducer&lt;TState, TPart&gt;* **subTreeReducer**  
       Instance of ISubTreeReducer


<a id="Urunium.Redux.Compose.ReducerComposer`1.Apply(`0,System.Object)"></a>
  * *TState* **Apply** *(TState previousState, Object action)*  
       Apply action to state using registered reducers.
  
    **Returns:** Resulting state after applying action.
  
    **Parameters:**
     * *TState* **previousState**  
       State that needs to be transformed

     * *Object* **action**  
       Action that needs to be applied to state.





---

<a id="Urunium.Redux.Compose.SubTreeToFullTreeAdapter`2"></a>
## class Urunium.Redux.Compose.SubTreeToFullTreeAdapter&lt;TState, TPart&gt;

Helps adapt a subtree reducer into full state tree reducer.

**Constructors**

 <a id="Urunium.Redux.Compose.SubTreeToFullTreeAdapter`2..ctor"></a>

 * **Urunium.Redux.Compose.SubTreeToFullTreeAdapter&lt;TState, TPart&gt;** *(Urunium.Redux.Compose.ISubTreeReducer&lt;TState, TPart&gt; subTreeReducer)*  
   Constructor for SubTreeToFullTreeAdapter
  
    **Parameters:**
     * *ISubTreeReducer&lt;TState, TPart&gt;* **subTreeReducer**  
       Instance of subtree reducer that needs to adapt.

  


**Methods**

<a id="Urunium.Redux.Compose.SubTreeToFullTreeAdapter`2.Apply(`0,System.Object)"></a>
  * *TState* **Apply** *(TState previousState, Object action)*  
       Apply subtree reducer and adapt to application state
  
    **Returns:** New state of property
  
    **Parameters:**
     * *TState* **previousState**  
       Old state of property

     * *Object* **action**  
       Action to be applied





---

