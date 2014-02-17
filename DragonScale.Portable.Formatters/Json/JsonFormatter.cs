/***
 * Author: Aliqi
 * E-mail: aliqi@hotmail.com
 * Created Time: 2013-01-21
 * Copyright: If you want to use this module, please retain this comment.
 * You can change any code of this file and add your name to the developers list,
 * but you cannot delete this comment or modify all content above.
 * Here's the Developers List:
 */

using System;
using System.IO;
using System.Text;

namespace DragonScale.Portable.Formatters.Json
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class JsonFormatter : Formatter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonFormatter" /> class.
        /// </summary>
        public JsonFormatter() { }

        /// <summary>
        /// To the bytes.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public override byte[] ToBytes(object obj)
        {
            var json = obj.ToJson();
            return Encoding.UTF8.GetBytes(json);
        }

        /// <summary>
        /// To the object.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public override object ToObject(byte[] bytes, Type type)
        {
            var content = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
            return content.ToObject(type, ContentFormat.Json);
        }

        /// <summary>
        /// To the object.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public override object ToObject(Stream stream, Type type)
        {
            var bytes = new byte[stream.Length - stream.Position];
            stream.Read(bytes, (int)stream.Position, bytes.Length);
            return ToObject(bytes, type);
        }
    }
}
