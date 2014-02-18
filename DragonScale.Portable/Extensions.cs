/*
 * Author: Aliqi
 * E-mail: aliqi@hotmail.com
 * Created Time: 2012-07-22
 * Copyright: If you want to use this module, please retain this comment.
 * You can change any code of this file and add your name to the developers list,
 * but you cannot delete this comment or modify all content above.
 * Here's the Developers List:
 */

using System;
using System.Reflection;

namespace DragonScale.Portable
{
    /// <summary>
    /// Extensions static class.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Check whether the source starts with the value.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static bool StartsWith(this byte[] source, byte[] value)
        {
            if (value.Length <= source.Length)
            {
                for (int i = 0; i < value.Length; i++)
                    if (source[i] != value[i])
                        return false;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Check whether the source ends with the value.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static bool EndsWith(this byte[] source, byte[] value)
        {
            if (value.Length <= source.Length)
            {
                var source_index = source.Length - 1;
                for (int i = value.Length - 1; i >= 0; i--)
                {
                    if (value[i] != source[source_index])
                        return false;
                    source_index--;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Takes the specified range of data from source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="length">The length.</param>
        /// <returns></returns>
        public static byte[] Take(this byte[] source, int offset, int length)
        {
            var retval = new byte[length];
            Buffer.BlockCopy(source, offset, retval, 0, length);
            return retval;
        }

        /// <summary>
        /// Determines whether the specified type has interface.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="interfaceType">Type of the interface.</param>
        /// <returns>
        ///   <c>true</c> if the specified type has interface; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasInterface(this Type type, Type interfaceType)
        {
            var interfaces = type.GetInterfaces();
            foreach (var item in interfaces)
                if (item == interfaceType)
                    return true;
            return false;
        }

        /// <summary>
        /// Determines whether [has default constructor] [the specified type].
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///   <c>true</c> if [has default constructor] [the specified type]; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasDefaultConstructor(this Type type)
        {
            var ctors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            foreach (var item in ctors)
                if (item.GetParameters().Length == 0)
                    return true;
            return false;
        }

        /// <summary>
        /// Determines whether [the specified string] [is null or trimed empty].
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if [the specified string value] [is null or trimed empty]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullOrTrimedEmpty(string value)
        {
            if (value == null)
                return true;
            return value.Trim() == string.Empty;
        }
    }
}
