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
using System.IO;

namespace DragonScale.Portable
{
    /// <summary>
    /// Formatter abstract class.
    /// </summary>
    public abstract class Formatter
    {
        /// <summary>
        /// To the bytes.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public abstract byte[] ToBytes(object obj);

        /// <summary>
        /// To the object.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public abstract object ToObject(byte[] bytes, Type type);

        /// <summary>
        /// To the object.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public abstract object ToObject(Stream stream, Type type);
    }
}
