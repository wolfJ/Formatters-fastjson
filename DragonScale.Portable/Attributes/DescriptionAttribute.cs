/***
 * Author: Aliqi
 * E-mail: aliqi@hotmail.com
 * Created Time: 2012-11-13
 * Copyright: If you want to use this module, please retain this comment.
 * You can change any code of this file and add your name to the developers list,
 * but you cannot delete this comment or modify all content above.
 * Here's the Developers List:
 */

using System;

namespace DragonScale.Portable
{
    /// <summary>
    /// Description attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
    public sealed class DescriptionAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the detail.
        /// </summary>
        /// <value>
        /// The detail.
        /// </value>
        public string Detail { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DescriptionAttribute" /> class.
        /// </summary>
        /// <param name="detail">The detail.</param>
        public DescriptionAttribute(string detail)
        {
            Detail = detail;
        }
    }
}
