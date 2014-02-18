/*
 * Author: Aliqi
 * E-mail: aliqi@hotmail.com
 * Created Time: 2012-07-22
 * Copyright: If you want to use this module, please retain this comment.
 * You can change any code of this file and add your name to the developers list,
 * but you cannot delete this comment or modify all content above.
 * Here's the Developers List:
 */

using System.Text;

namespace DragonScale.Portable
{
    /// <summary>
    /// Logger abstract class.
    /// </summary>
    public abstract class Logger
    {
        #region Fields
        /// <summary>
        /// The log string
        /// </summary>
        protected const string LOG = "Log";

        /// <summary>
        /// The Verbose string
        /// </summary>
        protected const string VERBOSE = "Verbose";

        /// <summary>
        /// The Debug string
        /// </summary>
        protected const string DEBUG = "Debug";

        /// <summary>
        /// The Info string
        /// </summary>
        protected const string INFO = "Info";

        /// <summary>
        /// The Warn string
        /// </summary>
        protected const string WARN = "Warn";

        /// <summary>
        /// The Error string
        /// </summary>
        protected const string ERROR = "Error";

        /// <summary>
        /// The Fatal string
        /// </summary>
        protected const string FATAL = "Fatal";

        /// <summary>
        /// The Core string
        /// </summary>
        protected const string CORE = "Core";

        /// <summary>
        /// The verbose level is 1
        /// </summary>
        public const int VerboseLevel = 1;

        /// <summary>
        /// The debug level is 2
        /// </summary>
        public const int DebugLevel = 2;

        /// <summary>
        /// The info level is 3
        /// </summary>
        public const int InfoLevel = 3;

        /// <summary>
        /// The warn level is 4
        /// </summary>
        public const int WarnLevel = 4;

        /// <summary>
        /// The error level is 5
        /// </summary>
        public const int ErrorLevel = 5;

        /// <summary>
        /// The fatal level is 6
        /// </summary>
        public const int FatalLevel = 6;

        /// <summary>
        /// The core level is 0
        /// </summary>
        public const int CoreLevel = 0;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether [info enabled].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [info enabled]; otherwise, <c>false</c>.
        /// </value>
        public bool InfoEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [fatal enabled].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [fatal enabled]; otherwise, <c>false</c>.
        /// </value>
        public bool FatalEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [error enabled].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [error enabled]; otherwise, <c>false</c>.
        /// </value>
        public bool ErrorEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [warn enabled].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [warn enabled]; otherwise, <c>false</c>.
        /// </value>
        public bool WarnEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [debug enabled].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [debug enabled]; otherwise, <c>false</c>.
        /// </value>
        public bool DebugEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [core enabled].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [core enabled]; otherwise, <c>false</c>.
        /// </value>
        public bool CoreEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [verbose enabled].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [verbose enabled]; otherwise, <c>false</c>.
        /// </value>
        public bool VerboseEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [log enabled].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [log enabled]; otherwise, <c>false</c>.
        /// </value>
        public bool LogEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [all log options enabled].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [all log options enabled]; otherwise, <c>false</c>.
        /// </value>
        public bool Enabled
        {
            get
            {
                return InfoEnabled && FatalEnabled && ErrorEnabled && LogEnabled &&
                    WarnEnabled && DebugEnabled && CoreEnabled && VerboseEnabled;
            }
            set
            {
                InfoEnabled = FatalEnabled = ErrorEnabled = VerboseEnabled = LogEnabled =
                    WarnEnabled = DebugEnabled = CoreEnabled = value;
            }
        }
        #endregion

        #region Virtual
        /// <summary>
        /// Verboses using the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="level">The 1 level.</param>
        protected virtual void RaiseVerbose(string message, int level) { RaiseLog(message, level); }

        /// <summary>
        /// Debugs using the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="level">The 2 level.</param>
        protected virtual void RaiseDebug(string message, int level) { RaiseLog(message, level); }

        /// <summary>
        /// Infoes using the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="level">The 3 level.</param>
        protected virtual void RaiseInfo(string message, int level) { RaiseLog(message, level); }

        /// <summary>
        /// Warns using the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="level">The 4 level.</param>
        protected virtual void RaiseWarn(string message, int level) { RaiseLog(message, level); }

        /// <summary>
        /// Errors using the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="level">The 5 level.</param>
        protected virtual void RaiseError(string message, int level) { RaiseLog(message, level); }

        /// <summary>
        /// Fatals using the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="level">The 6 level.</param>
        protected virtual void RaiseFatal(string message, int level) { RaiseLog(message, level); }

        /// <summary>
        /// Cores using the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="level">The 0 level.</param>
        protected virtual void RaiseCore(string message, int level) { RaiseLog(message, level); }

        /// <summary>
        /// Formats the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="level">The level.</param>
        /// <returns></returns>
        protected virtual string Format(string message, int level)
        {
            string sender = null;
            switch (level)
            {
                case VerboseLevel: sender = VERBOSE; break;
                case DebugLevel: sender = DEBUG; break;
                case InfoLevel: sender = INFO; break;
                case WarnLevel: sender = WARN; break;
                case ErrorLevel: sender = ERROR; break;
                case FatalLevel: sender = FATAL; break;
                case CoreLevel: sender = CORE; break;
                default:
                    sender = LOG;
                    break;
            }
            return new StringBuilder("[").Append(sender).Append("][Level: ")
                .Append(level).Append("] ").Append(message).ToString();
        }
        #endregion

        /// <summary>
        /// Logs using the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="level">The default level is 10.</param>
        protected abstract void RaiseLog(string message, int level);

        #region Methods
        /// <summary>
        /// Flushes everything.
        /// </summary>
        public virtual void Flush() { }

        /// <summary>
        /// Logs the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="level">The level.</param>
        public void Log(string message, int level = 10)
        {
            if (LogEnabled)
                RaiseLog(message, level);
        }

        /// <summary>
        /// Verboses the specified message. 1 Level.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Verbose(string message)
        {
            if (VerboseEnabled)
                RaiseVerbose(message, VerboseLevel);
        }

        /// <summary>
        /// Debugs using the specified message. 2 Level.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Debug(string message)
        {
            if (DebugEnabled)
                RaiseDebug(message, DebugLevel);
        }

        /// <summary>
        /// Infoes using the specified message. 3 Level.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Info(string message)
        {
            if (InfoEnabled)
                RaiseInfo(message, InfoLevel);
        }

        /// <summary>
        /// Warns using the specified message. 4 Level.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Warn(string message)
        {
            if (WarnEnabled)
                RaiseWarn(message, WarnLevel);
        }

        /// <summary>
        /// Errors using the specified message. 5 Level.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Error(string message)
        {
            if (ErrorEnabled)
                RaiseError(message, ErrorLevel);
        }

        /// <summary>
        /// Fatals using the specified message. 6 Level.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Fatal(string message)
        {
            if (FatalEnabled)
                RaiseFatal(message, FatalLevel);
        }

        /// <summary>
        /// Cores the specified message. 0 Level.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Core(string message)
        {
            if (CoreEnabled)
                RaiseCore(message, CoreLevel);
        }
        #endregion
    }
}
