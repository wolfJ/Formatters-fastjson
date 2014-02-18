/***
 * Author: Wolf
 * E-mail: wumingdlz@hotmail.com
 * Created Time: 2012-11-13
 * Copyright: If you want to use this module, please retain this comment.
 * You can change any code of this file and add your name to the developers list,
 * but you cannot delete this comment or modify all content above.
 * Here's the Developers List:
 */

using DragonScale.Portable.Formatters.Json;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;

namespace DragonScale.Portable.Formatters
{
    /// <summary>
    /// ContentFormat enum.
    /// </summary>
    [Flags]
    public enum ContentFormat : byte
    {
        /// <summary>
        /// The XML format.
        /// </summary>
        Xml,
        /// <summary>
        /// The json format.
        /// </summary>
        Json
    }

    /// <summary>
    /// Formatters static class.
    /// </summary>
    public static partial class Extensions
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        private static Formatter formatter;

        /// <summary>
        /// Gets or sets the default formatter. Set null to get a JsonFormatter as default.
        /// </summary>
        /// <value>
        /// The default formatter.
        /// </value>
        public static Formatter Default
        {
            get
            {
                if (formatter == null)
                    formatter = new JsonFormatter();
                return formatter;
            }
            set { formatter = value; }
        }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public static Logger Logger { get; set; }

        #region Methods
        #region Basic
        /// <summary>
        /// To the bytes.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static byte[] ToBytes(this object obj)
        {
            return Default.ToBytes(obj);
        }

        /// <summary>
        /// To the object.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static object ToObject(this byte[] bytes, Type type)
        {
            return Default.ToObject(bytes, type);
        }

        /// <summary>
        /// To the object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bytes">The bytes.</param>
        /// <returns></returns>
        public static T ToObject<T>(this byte[] bytes)
        {
            var obj = ToObject(bytes, typeof(T));
            if (obj is T)
                return (T)obj;
            else
                return default(T);
        }

        /// <summary>
        /// To the object.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static object ToObject(this Stream stream, Type type)
        {
            return Default.ToObject(stream, type);
        }

        /// <summary>
        /// To the object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        public static T ToObject<T>(this Stream stream)
        {
            var obj = ToObject(stream, typeof(T));
            if (obj is T)
                return (T)obj;
            else
                return default(T);
        }
        #endregion

        #region Extension
        /// <summary>
        /// To XML format content.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static string ToXml(this object obj)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// To json format content.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static string ToJson(this object obj, Settings settings = null)
        {
            var json = JsonMapper.ToJson(obj, settings);
            if (settings == null || Arrangement.None == settings.OutputArrangement)
                return json;
            else
                return new JsonArrangement().Beautify(json);
        }

        /// <summary>
        /// From string to object. Default content format is Json.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="type">The type.</param>
        /// <param name="format">The format.</param>
        /// <param name="settings">The settings.</param>
        /// <returns></returns>
        public static object ContentToObject(this string source, Type type,
            ContentFormat format = ContentFormat.Json, Settings settings = null)
        {
            if (format == ContentFormat.Json)
                return JsonMapper.ToObject(type, source, settings);
            else
                throw new NotImplementedException();
        }

        /// <summary>
        /// From string to object. Default content format is Json.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="format">The format.</param>
        /// <param name="settings">The settings.</param>
        /// <returns></returns>
        public static T ContentToObject<T>(this string source,
            ContentFormat format = ContentFormat.Json, Settings settings = null)
        {
            var t = typeof(T);
            if (format == ContentFormat.Json)
            {
                var obj = ContentToObject(source, typeof(T), format, settings);
                if (obj is T)
                    return (T)obj;
                else
                    return default(T);
            }
            else
            {
                var obj = ContentToObject(source, typeof(T), format);
                if (obj is T)
                    return (T)obj;
                else
                    return default(T);
            }
        }
        public static T ToObject<T>(this string source,
           ContentFormat format = ContentFormat.Json, Settings settings = null)
        {
            var t = typeof(T);
            if (format == ContentFormat.Json)
            {
                var obj = ContentToObject(source, typeof(T), format, settings);
                if (obj is T)
                    return (T)obj;
                else
                    return default(T);
            }
            else
            {
                var obj = ContentToObject(source, typeof(T), format);
                if (obj is T)
                    return (T)obj;
                else
                    return default(T);
            }
        }
        #endregion
        #endregion
    }
}
