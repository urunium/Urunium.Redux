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
object tree to represent whole of application's state instead of using multiple stores. In Urunium.Redux, store implements [IStore&lt;TState&gt;](http://uruniumredux.readthedocs.io/en/latest/Urunium.Redux/#interface-uruniumreduxistoretstate) interface,
where `TState` is the class representing state.

### State
State object holds the current state of application. The state mainly represents the current status of UI and also holds few data helping in business 
logic. In Urunium.Redux state can be any type: a class, struct or primitive datatype. There is just one rule that state must follow: **Immutability**.
A state object will never change it's properties. Every time we need to change something a new instance of state must be created, and full old state 
must be replaced by this new object.

### Reducers
Reducers are pure function whose name comes from "reduce()" method of functional programming (as in map/reduce).

### Actions
Actions are object telling reducer what needs to be done to current state. Reducer decides operation to be performed on state based on the type of 
action that is dispatched. Optionally actions may also contain payloads that reducer can used to make further decission. In Urunium.Redux action can
be any object including primitive type. In general it is good idea to make actions immutable too.
