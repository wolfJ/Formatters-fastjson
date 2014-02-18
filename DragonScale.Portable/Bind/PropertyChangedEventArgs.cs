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

namespace DragonScale.Portable
{
    /// <summary>
    /// PropertyChangedEventArgs class.
    /// </summary>
    public sealed class PropertyChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <value>
        /// The name of the property.
        /// </value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the value of the property.
        /// </summary>
        /// <value>
        /// The value of the property.
        /// </value>
        public object Value { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyChangedEventArgs" /> class.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="propertyValue">Value of the property.</param>
        public PropertyChangedEventArgs(string propertyName, object propertyValue)
        {
            Guard.ArgumentNotNullOrEmptyString(propertyName, "propertyName");
            Name = propertyName;
            Value = propertyValue;
        }
    }
}
