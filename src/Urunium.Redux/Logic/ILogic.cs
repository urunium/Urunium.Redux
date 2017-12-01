using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Urunium.Redux.Logic
{
    /// <summary>
    /// Give priority to your Logic. In case two logic handles same action, but sequence in which these must be handled matters 
    /// then set higher priority to logic that must be executed first.
    /// </summary>
    public interface IPriority
    {
        /// <summary>
        /// Priority of logic. Priority is calculated in descending order. i.e. Greater the number higher the priority
        /// </summary>
        uint Priority { get; }
    }

    /// <summary>
    /// Make logic cancelable.
    /// </summary>
    public interface ICancelable
    {
        /// <summary>
        /// Type of action that when dispatched will cancel this process
        /// </summary>
        Type CancelType { get; }

        /// <summary>
        /// Request canceling
        /// </summary>
        /// <typeparam name="TCancel"></typeparam>
        /// <param name="cancelAction"></param>
        void Cancel<TCancel>(TCancel cancelAction);
    }

    /// <summary>
    /// Interface representing a `Logic`. A class must implement this interface, if it wants to get registered in store as a logic. 
    /// `Logic` here typically means "business logic", a validation, transformation or  some kind of processing.
    /// </summary>
    /// <typeparam name="TState">Type of State</typeparam>
    /// <typeparam name="TAction">
    /// Type of Action this logic wants to handle. 
    /// Supply <see cref="AnyAction"/> if this logic is interested in processing all actions dispatched to store. 
    /// E.g. to log all the action etc.
    /// </typeparam>
    public interface ILogic<TState, TAction> : IPriority, ICancelable
    {
        /// <summary>
        /// Indicate that this is a long running process, so other logics must continue while this is running in background 
        /// (i.e. don't wait for it to complete). Long running processes are given separate thread, through use of: 
        /// <see cref="TaskCreationOptions.LongRunning"/>. Implementor doesn't need to concern with running it in separate thread. 
        /// Note:
        /// - <see cref="PreProcess(IStore{TState}, TAction)"/> cannot be long running.
        /// - Only <see cref="Process(Func{TState}, TAction, IMultiDispatcher)"/> can be long running.
        /// </summary>
        bool IsLongRunning { get; }

        /// <summary>
        /// Preprocess an action before it is dispatched to store. E.g. Validate action, Transform action etc.
        /// In case multiple logic handles same action type, then preprocess of each logic is executed in order of priority
        /// before dispatching action.
        /// </summary>
        /// <param name="store">Store</param>
        /// <param name="action">Action that needs to be preprocessed.</param>
        /// <returns>
        /// An instance of <see cref="PreProcessResult"/>, indicating whether or not next logic in chain should be executed. 
        /// Note that, setting <see cref="PreProcessResult.ContinueToNextStep"/> to false will stop logic chaing right there. Even
        /// <see cref="Process(Func{TState}, TAction, IMultiDispatcher)"/> is also not executed 
        /// </returns>
        Task<PreProcessResult> PreProcess(IStore<TState> store, TAction action);

        /// <summary>
        /// Processing of dispatched action. This gets executed after <see cref="PreProcess(IStore{TState}, TAction)"/> and,
        /// <see cref="IStore{TState}.Dispatch{TAction}(TAction)"/>. Typically pre-processing is to handle scenarios before
        /// dispatching action, and process is for handling after dispatching.
        /// </summary>
        /// <param name="getState">Function to get current state.</param>
        /// <param name="action">Action that needs to be processed.</param>
        /// <param name="dispatcher">Multi dispatcher.</param>
        /// <returns>async Task (instead of async void.)</returns>
        Task Process(Func<TState> getState, TAction action, IMultiDispatcher dispatcher);
    }

    /// <summary>
    /// Configuration helper for logic extension, to add logic to store.
    /// </summary>
    /// <typeparam name="TState">Type of application's state</typeparam>
    public interface ILogicConfiguration<TState>
    {
        /// <summary>
        /// Add a business logic that will listen to particular action beign dispatched to store.
        /// </summary>
        /// <typeparam name="TAction">
        /// Type of action that business logic is interested in processing. use <see cref="AnyAction"/> if business logic needs
        /// to process all the action being dispatched.
        /// </typeparam>
        /// <param name="logics">
        /// Array of logics; processing particular action type. Multiple calls are needed to handle different action types.
        /// </param>
        void AddLogics<TAction>(params ILogic<TState, TAction>[] logics);
    }
}
