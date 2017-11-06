using System;
using System.Threading;
using System.Threading.Tasks;
using Urunium.Redux.Logic;
using UruniumWPFExample.Actions;
using UruniumWPFExample.States;

namespace UruniumWPFExample.Logics
{
    /// <summary>
    /// Logic to handle dispatch of `UpdateTodoText` action.
    /// </summary>
    public class UpdateTodoTextHandler : LogicBase<TodoList, UpdateTodoText>
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
        /// Validate the UpdateTodoText action being dispatched.
        /// </summary>
        /// <param name="getState">Function to get the latest state.</param>
        /// <param name="action">Instance of action that is being dispatched.</param>
        /// <param name="dispatcher">Multi-Dispatcher</param>
        /// <param name="cancellationToken">Cancelation token that can be used to cancel validation step.</param>
        /// <returns>Awaitable ValidationResult</returns>
        protected override Task<ValidationResult> OnValidate(Func<TodoList> getState, UpdateTodoText action, IMultiDispatcher dispatcher, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(action.Text))
            {
                return Task.FromResult(ValidationResult.Failure(new ValidationException("Cannot update")));
            }
            return Task.FromResult(ValidationResult.Success());
        }

        /// <summary>
        /// Further processing after `UpdateTodoText` has beed dispatched.
        /// </summary>
        /// <param name="getState">Function to get the latest state.</param>
        /// <param name="action">Instance of action that is being dispatched.</param>
        /// <param name="dispatcher">Multi-Dispatcher</param>
        /// <param name="cancellationToken">Cancelation token that can be used to cancel validation step.</param>
        /// <returns>Awaitable task.</returns>
        protected override Task OnProcess(Func<TodoList> getState, UpdateTodoText action, IMultiDispatcher dispatcher, CancellationToken cancellationToken)
        {
            dispatcher.Dispatch(new HideTodoEditor(action.Id));
            return Task.CompletedTask;
        }
    }
}
