/*
 * Author: Aliqi
 * E-mail: aliqi@hotmail.com
 * Created Time: 2012-07-22
 * Copyright: If you want to use this module, please retain this comment.
 * You can change any code of this file and add your name to the developers list,
 * but you cannot delete this comment or modify all content above.
 * Here's the Developers List:
 */

#if DEBUG

namespace DragonScale.Portable.Toolbox
{
    /// <summary>
    /// DebugLogger class using Debug.WriteLine.
    /// </summary>
    public sealed class DebugLogger : Logger
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DebugLogger"/> class.
        /// </summary>
        public DebugLogger() { Enabled = true; }

        /// <summary>
        /// Logs using the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="level">The default level is 10.</param>
        protected override void RaiseLog(string message, int level)
        {
            System.Diagnostics.Debug.WriteLine(Format(message, level));
        }
    }
}

#endif
