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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using DragonScale.Portable.Formatters.Core;

namespace DragonScale.Portable.Formatters
{

    /// <summary>
    /// ContentProvider abstract class.
    /// </summary>
    public abstract class ContentProvider
    {
        #region Fields
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IList<PropertyInfo> ignoredProperties = new List<PropertyInfo>();

        [EditorBrowsable(EditorBrowsableState.Never)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Dictionary<Type, List<PropertyInfo>> cacheProperties =
            new Dictionary<Type, List<PropertyInfo>>();

        [EditorBrowsable(EditorBrowsableState.Never)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IList<FieldInfo> ignoredFields =
            new List<FieldInfo>();

        [EditorBrowsable(EditorBrowsableState.Never)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Dictionary<Type, List<FieldInfo>> cacheFields =
            new Dictionary<Type, List<FieldInfo>>();

        [EditorBrowsable(EditorBrowsableState.Never)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Dictionary<Type, List<RenameMapping>> _keyRenameMapping = new Dictionary<Type, List<RenameMapping>>();
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the ignored properties.
        /// </summary>
        /// <value>
        /// The ignored properties.
        /// </value>
        public IList<PropertyInfo> IgnoredProperties { get { return ignoredProperties; } }

        /// <summary>
        /// Gets the ignored fields.
        /// </summary>
        /// <value>
        /// The ignored fields.
        /// </value>
        public IList<FieldInfo> IgnoredFields { get { return ignoredFields; } }

        /// <summary>
        /// Gets the settings.
        /// </summary>
        /// <value>
        /// The settings.
        /// </value>
        public Settings Settings { get; private set; }

        public Dictionary<Type, List<RenameMapping>> KeyRenameMapping { get { return _keyRenameMapping; } }

        #endregion

        #region Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="ContentProvider"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        protected ContentProvider(Settings settings)
        { Guard.ArgumentNotNull(settings, "settings"); Settings = settings; }
        #endregion

        #region Methods

        /// <summary>
        /// Gets the binding flags.
        /// </summary>
        /// <returns></returns>
        public abstract BindingFlags GetBindingFlags();

        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public PropertyInfo[] GetProperties(Type type)
        {
            // Search in cache
            List<PropertyInfo> propertyInfos = null;
            if (cacheProperties.TryGetValue(type, out propertyInfos))
                return propertyInfos.ToArray();
            // Creating infos
            var properties = RaiseGetProperties(type);
            var result = new List<PropertyInfo>();
            if (properties != null)
                foreach (var property in properties)
                    if (!IgnoreProperty(property))
                        result.Add(property);
            // adding result to Cache
            cacheProperties.Add(type, result);
            return result.ToArray();
        }

        /// <summary>
        /// Gets the fields.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public FieldInfo[] GetFields(Type type)
        {
            // Search in cache
            List<FieldInfo> infos = null;
            if (cacheFields.TryGetValue(type, out infos))
                return infos.ToArray();
            // Creating infos
            var fields = RaiseGetFields(type);
            var result = new List<FieldInfo>();
            if (fields != null)
                foreach (var field in fields)
                    if (!IgnoreField(field))
                        result.Add(field);
            // adding result to Cache
            cacheFields.Add(type, result);
            return result.ToArray();
        }

        /// <summary>
        /// Raises on the get properties process.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        protected abstract PropertyInfo[] RaiseGetProperties(Type type);

        /// <summary>
        /// Raises on the get the get fields process.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        protected abstract FieldInfo[] RaiseGetFields(Type type);

        /// <summary>
        /// Determines whether the property should be removed from serialization.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>
        /// true if the property:
        /// - is in the IgnoredProperties,
        /// - contains TransientAttribute,
        /// - does not have it's set or get accessor
        /// - is indexer
        /// </returns>
        protected virtual bool IgnoreProperty(PropertyInfo property)
        {
            if (IgnoredProperties.Contains(property))
                return true;
            if (ContainsIgnoredAttributes(property))
                return true;
            if (!property.CanRead || !property.CanWrite)
                return true;
            var indexParameters = property.GetIndexParameters();
            // Indexer
            if (indexParameters.Length > 0)
                return true;
            return false;
        }

        /// <summary>
        /// Determines whether the field should be removed from serialization.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns>
        ///  true if the field:
        /// - is in the IgnoredFields,
        /// - contains TransientAttribute,
        /// - is readonly
        /// - is const
        /// </returns>
        protected virtual bool IgnoreField(FieldInfo field)
        {
            if (IgnoredFields.Contains(field))
                return true;
            if (ContainsIgnoredAttributes(field))
                return true;
            if (field.IsLiteral || field.IsInitOnly)
                return true;
            return false;
        }

        /// <summary>
        /// Determines whether [contains ignored attributes] in [the specified member].
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns>
        ///   <c>true</c> if [contains ignored attributes] in [the specified member]; otherwise, <c>false</c>.
        /// </returns>
        protected bool ContainsIgnoredAttributes(MemberInfo member)
        {
            foreach (var attrType in Settings.IgnoredAttributes)
            {
                var attributes = member.GetCustomAttributes(attrType, false);
                if (attributes.Length > 0)
                    return true;
            }
            return false;
        }
        #endregion
    }
}
