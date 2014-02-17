/***
 * Author: Aliqi
 * E-mail: aliqi@hotmail.com
 * Created Time: 2012-10-01
 * Copyright: If you want to use this module, please retain this comment.
 * You can change any code of this file and add your name to the developers list,
 * but you cannot delete this comment or modify all content above.
 * Here's the Developers List:
 */

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;

namespace DragonScale.Portable.Formatters
{
    /// <summary>
    /// DefaultContentProvider class.
    /// </summary>
    public sealed class DefaultContentProvider : ContentProvider
    {
        #region Fields
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static readonly BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
        #endregion

        #region Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultContentProvider"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public DefaultContentProvider(Settings settings) : base(settings) { }
        #endregion

        #region Methods
        /// <summary>
        /// Raises on the get properties process. Default get action will return which:
        ///   - are public
        ///   - are not static (instance properties)
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        protected override PropertyInfo[] RaiseGetProperties(Type type)
        {
            return type.GetProperties(bindingFlags);
        }

        /// <summary>
        /// Raises on the get the get fields process. Default get action will return which:
        /// - are public
        /// - are not static (instance fields)
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        protected override FieldInfo[] RaiseGetFields(Type type)
        {
            return type.GetFields(bindingFlags);
        }
        #endregion

        public override BindingFlags GetBindingFlags()
        {
            return bindingFlags;
        }
    }
}
