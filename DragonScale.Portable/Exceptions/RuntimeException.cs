/*
 * Author: Aliqi
 * E-mail: aliqi@hotmail.com
 * Created Time: 2011-3-21
 * Copyright: If you want to use this module, please retain this comment.
 * You can change any code of this file and add your name to the developers list,
 * but you cannot delete this comment or modify all content above.
 * Here's the Developers List:
 */

using System;

namespace DragonScale.Portable.Exceptions
{
    /// <summary>
    /// Runtime exception.
    /// </summary>
    public class RuntimeException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeException"/> class.
        /// </summary>
        public RuntimeException() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public RuntimeException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public RuntimeException(string message, Exception innerException) : base(message, innerException) { }
    }
}
