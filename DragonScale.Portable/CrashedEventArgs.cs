/*
 * Author: Aliqi
 * E-mail: aliqi@hotmail.com
 * Created Time: 2012-09-24
 * Copyright: If you want to use this module, please retain this comment.
 * You can change any code of this file and add your name to the developers list,
 * but you cannot delete this comment or modify all content above.
 * Here's the Developers List:
 */

using System;

namespace DragonScale.Portable
{
    /// <summary>
    /// CrashedEventArgs class.
    /// </summary>
    public class CrashedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the exception.
        /// </summary>
        /// <value>
        /// The exception.
        /// </value>
        public Exception Exception { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="CrashedEventArgs"/> is handled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if handled; otherwise, <c>false</c>.
        /// </value>
        public bool Handled { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CrashedEventArgs"/> class.
        /// </summary>
        /// <param name="e">The e.</param>
        public CrashedEventArgs(Exception e)
        {
            Exception = e;
        }
    }
}