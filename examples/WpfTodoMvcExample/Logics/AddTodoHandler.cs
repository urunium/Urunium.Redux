using System;
using System.Threading;
using System.Threading.Tasks;
using Urunium.Redux.Logic;
using WpfTodoMvcExample.Actions;
using WpfTodoMvcExample.States;

namespace WpfTodoMvcExample.Logics
{
    /// <summary>
    /// Logic to handle dispatch of `AddTodo` action.
    /// </summary>
    public class AddTodoHandler : LogicBase<TodoList, AddTodo>
    {
        /// <summary>
        /// Action type that can be dispatched to cancel this logic handler.
        /// null => This handler cannot be canceled.
        /// </summary>
        public override Type CancelType => null;

        /// <summary>
        /// Priority of this handler.
        /// </summary>
        public override uint Priority => 1;

        /// <summary>
        /// Validate the AddTodo action being dispatched.
        /// </summary>
        /// <param name="getState">Function to get the latest state.</param>
        /// <param name="action">Instance of action that is being dispatched.</param>
        /// <param name="dispatcher">Multi-Dispatcher</param>
        /// <param name="cancellationToken">Cancelation token that can be used to cancel validation step.</param>
        /// <returns>Awaitable ValidationResult</returns>
        protected override Task<ValidationResult> OnValidate(Func<TodoList> getState, AddTodo action, IMultiDispatcher dispatcher, CancellationToken cancellationToken)
        {
            if (action.Payload == null)
            {
                return Task.FromResult(ValidationResult.Failure(new ValidationException("No todo to add")));
            }
            else if (string.IsNullOrEmpty(action.Payload.Text))
            {
                return Task.FromResult(ValidationResult.Failure(new ValidationException("Cannot add todo with empty text")));
            }
            return Task.FromResult(ValidationResult.Success());
        }
    }
}
