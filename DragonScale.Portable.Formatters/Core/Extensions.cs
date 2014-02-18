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
using System.ComponentModel;

namespace DragonScale.Portable.Formatters
{
    /// <summary>
    /// Extensions static class.
    /// </summary>
    public static partial class Extensions
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        private static bool isType(Type type)
        {
            return type == typeof(Type) || type.IsSubclassOf(typeof(Type));
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal static void ErrorLog(string msg)
        {
            if (Logger != null)
                Logger.Error(msg);
        }
    }
}