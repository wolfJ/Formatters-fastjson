/***
 * Author: Wolf
 * E-mail: wumingdlz@hotmail.com
 * Created Time: 2013-01-21
 * Copyright: If you want to use this module, please retain this comment.
 * You can change any code of this file and add your name to the developers list,
 * but you cannot delete this comment or modify all content above.
 * Here's the Developers List:
 */

using System;
using System.IO;
using System.Text;

namespace DragonScale.Portable.Formatters
{
    /// <summary>
    /// JsonFormatter class.
    /// </summary>
    public sealed class JsonFormatter : Formatter
    {
        private JsonFormatterSettings settings;

        /// <summary>
        /// Gets or sets the settings.
        /// </summary>
        /// <value>
        /// The settings.
        /// </value>
        public JsonFormatterSettings Settings { get { return settings; } set { settings = value; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonFormatter" /> class.
        /// </summary>
        public JsonFormatter() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonFormatter"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public JsonFormatter(JsonFormatterSettings settings) { this.settings = settings; }

        /// <summary>
        /// To the bytes.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public override byte[] ToBytes(object obj)
        {
            var json = obj.ToJson(settings);
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
            return content.ContentToObject(type, ContentFormat.Json, settings);
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
