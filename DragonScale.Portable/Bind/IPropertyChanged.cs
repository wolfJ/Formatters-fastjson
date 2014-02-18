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
    /// IPropertyChanged interface.
    /// </summary>
    public interface IPropertyChanged
    {
        /// <summary>
        /// Occurs when [property changed].
        /// </summary>
        event EventHandler<PropertyChangedEventArgs> PropertyChanged;
    }
}
