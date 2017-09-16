using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Urunium.Redux.Logic
{
    /// <summary>
    /// Validation Result
    /// </summary>
    public class ValidationResult
    {
        /// <summary>
        /// Use to check if Action was valid or not.
        /// </summary>
        public bool IsValid { get; }

        /// <summary>
        /// Use to determine what are the reason Action is invalid.
        /// Is set to null if IsValid is true.
        /// </summary>
        public ValidationException Error { get; }

        /// <summary>
        /// Use static factory method to create ValidationResult
        /// - <see cref="Success"/>
        /// - <see cref="Failure(ValidationException)"/>
        /// </summary>
        private ValidationResult()
        {
            IsValid = true;
        }

        /// <summary>
        /// Use static factory method to create ValidationResult
        /// - <see cref="Success"/>
        /// - <see cref="Failure(ValidationException)"/>
        /// </summary>
        private ValidationResult(ValidationException error)
        {
            IsValid = false;
            Error = error;
        }

        /// <summary>
        /// If action being dispatched is valid.
        /// </summary>
        /// <returns></returns>
        public static ValidationResult Success()
        {
            return new ValidationResult();
        }

        /// <summary>
        /// If action being dispatched is invalid.
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        public static ValidationResult Failure(ValidationException error)
        {
            return new ValidationResult(error);
        }
    }
}
