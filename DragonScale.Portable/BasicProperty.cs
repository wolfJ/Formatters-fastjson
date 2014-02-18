/***
 * Author: Aliqi
 * E-mail: aliqi@hotmail.com
 * Created Time: 2013-06-14
 * Copyright: If you want to use this module, please retain this comment.
 * You can change any code of this file and add your name to the developers list,
 * but you cannot delete this comment or modify all content above.
 * Here's the Developers List:
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace DragonScale.Portable
{
    /// <summary>
    /// BasicProperty class.
    /// </summary>
    public sealed class BasicProperty
    {
        #region Fields
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal readonly static Dictionary<int, BasicProperty> properties =
            new Dictionary<int, BasicProperty>();

        [EditorBrowsable(EditorBrowsableState.Never)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal readonly int Hash;

        [EditorBrowsable(EditorBrowsableState.Never)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal readonly bool IsPropertyValueType;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the default value.
        /// </summary>
        /// <value>
        /// The default value.
        /// </value>
        public object DefaultValue { get; private set; }

        /// <summary>
        /// Gets the type of the owner.
        /// </summary>
        /// <value>
        /// The type of the owner.
        /// </value>
        public Type OwnerType { get; private set; }

        /// <summary>
        /// Gets the type of the property.
        /// </summary>
        /// <value>
        /// The type of the property.
        /// </value>
        public Type PropertyType { get; private set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; private set; }
        #endregion

        #region Ctor
        [EditorBrowsable(EditorBrowsableState.Never)]
        private BasicProperty(int hash, bool isValueType)
        {
            Hash = hash;
            IsPropertyValueType = isValueType;
        }
        #endregion

        #region	Methods
        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return Hash;
        }

        /// <summary>
        /// Registers the specified basic property.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="ownerType">Type of the owner.</param>
        /// <param name="propertyType">Type of the property.</param>
        /// <returns></returns>
        public static BasicProperty Register(string name, Type ownerType, Type propertyType)
        {
            return Register(name, ownerType, propertyType, null);
        }

        /// <summary>
        /// Registers the specified basic property.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="ownerType">Type of the owner.</param>
        /// <param name="propertyType">Type of the property.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public static BasicProperty Register(string name, Type ownerType, Type propertyType,
            object defaultValue)
        {
            Guard.ArgumentNotNullOrTrimedEmptyString(name, "name");
            Guard.ArgumentNotNull(ownerType, "ownerType");
            Guard.ArgumentNotNull(propertyType, "propertyType");
            if (defaultValue != null && !propertyType.IsAssignableFrom(defaultValue.GetType()))
                throw new InvalidOperationException("Property type not match the type of default value.");
            var hash = name.GetHashCode() ^ ownerType.GetHashCode() ^ propertyType.GetHashCode();
            BasicProperty prop = null;
            if (properties.TryGetValue(hash, out prop))
                throw new InvalidOperationException("A kind of BasicProperty(s) can not register for multiple times."
                    + Environment.NewLine
                    + "This property has been register by " + ownerType);
            prop = new BasicProperty(hash, propertyType.IsValueType)
            {
                OwnerType = ownerType,
                PropertyType = propertyType,
                Name = name,
                DefaultValue = defaultValue
            };
            if (prop.IsPropertyValueType && defaultValue == null)
                prop.DefaultValue = Activator.CreateInstance(propertyType);
            properties.Add(hash, prop);
            return prop;
        }
        #endregion
    }
}
