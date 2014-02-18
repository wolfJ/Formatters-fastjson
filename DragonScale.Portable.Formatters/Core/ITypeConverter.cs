/***
 * Author: Wolf
 * E-mail: wumingdlz@hotmail.com
 * Created Time: 2012-10-01
 * Copyright: If you want to use this module, please retain this comment.
 * You can change any code of this file and add your name to the developers list,
 * but you cannot delete this comment or modify all content above.
 * Here's the Developers List:
 */

using System;

namespace DragonScale.Portable.Formatters.Core
{
    /// <summary>
    ///   Converts Type to its string representation and vice versa. The default instance used in the Framework is TypeNameConverter
    /// </summary>
    public interface ITypeConverter
    {
        /// <summary>
        ///   Gives back Type as text.
        /// </summary>
        /// <param name = "type"></param>
        /// <returns>string.Empty if the type is null</returns>
        string ToName(Type type);

        /// <summary>
        ///   Gives back Type from the text.
        /// </summary>
        /// <param name = "typeName"></param>
        /// <returns></returns>
        Type ToType(string typeName);
    }
}