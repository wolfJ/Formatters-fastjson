using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DragonScale.Portable.Formatters;
using System.Reflection;
using System.ComponentModel;
using System.Diagnostics;

namespace DragonScale.Silverlight.Formatters
{
    /// <summary>
    /// Full Content Provider
    /// </summary>
    public class FullContentProvider : ContentProvider
    {
        #region Fields
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static readonly BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static;
        #endregion


        #region Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultContentProvider"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public FullContentProvider(Settings settings) : base(settings) { }
        #endregion

        /// <summary>
        /// Raises on the get properties process.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        protected override System.Reflection.PropertyInfo[] RaiseGetProperties(Type type)
        {
            return type.GetProperties(bindingFlags | BindingFlags.GetProperty);
        }

        /// <summary>
        /// Raises on the get the get fields process.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        protected override System.Reflection.FieldInfo[] RaiseGetFields(Type type)
        {
            return type.GetFields(bindingFlags | BindingFlags.GetField);
        }


        public override BindingFlags GetBindingFlags()
        {
            return bindingFlags;
        }
    }

}
