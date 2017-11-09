using System;
using System.Linq.Expressions;
using Urunium.Redux.Compose;
using Urunium.Redux.Typed;
using WpfTodoMvcExample.Actions;
using WpfTodoMvcExample.States;

namespace WpfTodoMvcExample.Reducers
{
    /// <summary>
    /// Reducer to handle modification of <see cref="TodoList.Filter"/> property.
    /// </summary>
    public class VisibilityFilterReducer : TypedReducer<VisibilityFilter>, ISubTreeReducer<TodoList, VisibilityFilter>
    {
        /// <summary>
        /// Property that will be modified by this reducer.
        /// </summary>
        public Expression<Func<TodoList, VisibilityFilter>> PropertySelector => x => x.Filter;

        /// <summary>
        /// Reducer function to handle ChangeVisibilityFilter action.
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public VisibilityFilter Apply(VisibilityFilter filter, ChangeVisibilityFilter action)
        {
            return action.Payload;
        }
    }
}
