using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Urunium.Redux.Logic
{
    /// <summary>
    /// Exception representing something is invalid with dispatched action.
    /// </summary>
    public class ValidationException : Exception
    {
        /// <summary>
        /// Create new instance of <see cref="ValidationException"/>
        /// </summary>
        /// <param name="message">Exception Message, typically a summary message implying something went wrong.</param>
        /// <param name="validationDetails">List of ValidationDetails explaining what went wrong.</param>
        public ValidationException(string message, IEnumerable<ValidationDetails> validationDetails)
            : base(message)
        {
            ValidationDetails = validationDetails;
        }

        /// <summary>
        /// Create new instance of <see cref="ValidationException"/>
        /// </summary>
        /// <param name="message">Exception Message, typically a summary message implying something went wrong.</param>
        public ValidationException(string message)
            : base(message)
        {
            ValidationDetails = Enumerable.Empty<ValidationDetails>();
        }

        /// <summary>
        /// Details explaining what are invalid.
        /// </summary>
        public IEnumerable<ValidationDetails> ValidationDetails { get; }
    }

    /// <summary>
    /// Details of validation result
    /// </summary>
    public class ValidationDetails
    {
        /// <summary>
        /// Associates a validation message with a key. 
        /// Typically Key may be a name of UI (form) field, to which message needs to be associated. 
        /// Recommended convention is to use "*" when validation doesn't associate with any particular field.
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Explation of the reason for invalid.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Immutable Ctor
        /// </summary>
        /// <param name="key"></param>
        /// <param name="message"></param>
        public ValidationDetails(string key, string message)
        {
            Key = key;
            Message = message;
        }
    }
}
