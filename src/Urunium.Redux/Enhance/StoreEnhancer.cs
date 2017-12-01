using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Urunium.Redux.Enhance
{
    /// <summary>
    /// Gives ability to intercept and enhance store functionalities. This class works in Russian doll model,
    /// each enhancer wrapping an inner enhancer, until the inner most object is instance of <see cref="Store{TState}"/>
    /// itself. <a href="https://github.com/urunium/Urunium.Redux/blob/master/src/Urunium.Redux/Logic/LogicStoreExtension.cs#L33">LogicEnhancer</a> is an example of how enhancer can be used.
    /// </summary>
    /// <typeparam name="TState">Type of state.</typeparam>
    /// <example>
    /// <code>
    /// <![CDATA[
    /// // Store Enhancer, that gives filtered todo items based on VisibilityFilter
    /// // when a accessing State property.
    /// class FilterTodoItems : StoreEnhancer<Todo>
    /// {
    ///     public FilterTodoItems(IStore<Todo> store) : base(store)
    ///     {
    ///     }
    ///     
    ///     // override what gets returned when State property is accessed.
    ///     protected override Todo OnState(Func<Todo> forward)
    ///     {
    ///         var fullState = forward();
    ///         return new Todo(fullState.VisibilityFilter, FilterItems(fullState));
    ///     }
    ///     
    ///     private List<TodoItem> FilterItems(Todo state)
    ///     {
    ///         switch (state.VisibilityFilter)
    ///         {
    ///             case Filter.Completed:
    ///                 return state.Todos.Where(x => x.IsComplete).ToList();
    ///             case Filter.InProgress:
    ///                 return state.Todos.Where(x => !x.IsComplete).ToList();
    ///             default:
    ///                 return state.Todos;
    ///         }
    ///     }
    /// }
    /// 
    /// // Usage:
    /// IStore<Todo> store = new Store<Todo>(rootReducer, new Todo(Filter.ShowAll, new List<TodoItem>()))
    ///                         .EnhanceWith(typeof(FilterTodoItems));
    /// store.Dispatch(new AddTodo("Sleep"));
    /// store.Dispatch(new AddTodo("Eat"));
    /// store.Dispatch(new AddTodo("Drink"));
    /// store.Dispatch(new SetVisibilityFilter(Filter.Completed));
    /// // Since all todo Item is inprogress state.
    /// Assert.AreEqual(0, store.State.Todos.Count);
    /// store.Dispatch(new ToggleTodo(0));
    /// // Since one of the item is marked completed.
    /// Assert.AreEqual(1, store.State.Todos.Count);
    /// ]]>
    /// </code>
    /// </example>
    public abstract class StoreEnhancer<TState> : IStore<TState>
    {
        private EventHandler<EventArgs> _stateChangedHandlers = new EventHandler<EventArgs>((o, e) => { });

        /// <summary>
        /// Underlying Store object
        /// </summary>
        protected IStore<TState> Store { get; }

        /// <summary>
        /// Constructor of <see cref="StoreEnhancer{TState}"/> that enhances store object.
        /// </summary>
        /// <param name="store"><see cref="IStore{TState}"/> instance that needs to be enhanced.</param>
        public StoreEnhancer(IStore<TState> store)
        {
            Store = store;

            // Since StateChanged of original store cannot be changed,
            // It is advisable to not attach event handler directly to original store
            store.StateChanged += _store_StateChanged;
        }

        /// <summary>
        /// Enhanced State
        /// </summary>
        public TState State => OnState(() => Store.State);

        /// <summary>
        /// Get notified when state changes, with enhancements.
        /// </summary>
        public event EventHandler<EventArgs> StateChanged
        {
            add { _stateChangedHandlers += value; }
            remove { _stateChangedHandlers -= value; }
        }

        /// <summary>
        /// Dispatch action to reducers, with enhancements.
        /// </summary>
        /// <typeparam name="TAction"></typeparam>
        /// <param name="action"></param>
        public void Dispatch<TAction>(TAction action)
        {
            OnDispatch<TAction>(action, (actionParam) => Store.Dispatch(actionParam));
        }

        /// <summary>
        /// Locate a particular store enhancer, applied to current store.
        /// Note: 
        /// - Search is inwards, i.e while locating, traversal is done from 
        /// outer most enhacer to inner-most store.
        /// - This is implementation detail of IStore extension method : <seealso cref="StoreExtension.FindEnhancer{TEnhancer, TState}(IStore{TState})"/>
        /// </summary>
        /// <typeparam name="TEnhancer">Type of enhancer to find</typeparam>
        /// <returns>Enhancer instance if found, or null.</returns>
        public TEnhancer Find<TEnhancer>() where TEnhancer : StoreEnhancer<TState>
        {
            IStore<TState> store = this;
            while (store is StoreEnhancer<TState> enhancer)
            {
                if (store is TEnhancer)
                {
                    return store as TEnhancer;
                }
                store = enhancer.Store;
            }
            return null;
        }

        /// <summary>
        /// Override this method to handle how state is returned.
        /// </summary>
        /// <param name="forward">Forward the call to original state.</param>
        /// <returns>State with enhancements.</returns>
        protected virtual TState OnState(Func<TState> forward)
        {
            return forward();
        }

        /// <summary>
        /// Override this method to enhance dispatch.
        /// </summary>
        /// <typeparam name="TAction">Type of action</typeparam>
        /// <param name="action">Instance of action that needs to be applied by reducers.</param>
        /// <param name="forward">Forward call to original dispatch function.</param>
        protected virtual void OnDispatch<TAction>(TAction action, Action<TAction> forward)
        {
            forward(action);
        }

        /// <summary>
        /// Override this method to enhance StateChanged event handling.
        /// E.g. Logging.
        /// </summary>
        /// <param name="forward">Forward calls to actual event handlers.</param>
        protected virtual void OnStateChanged(EventHandler<EventArgs> forward)
        {
            forward(this, new EventArgs());
        }

        private void _store_StateChanged(object sender, EventArgs e)
        {
            OnStateChanged(_stateChangedHandlers);
        }
    }
}
