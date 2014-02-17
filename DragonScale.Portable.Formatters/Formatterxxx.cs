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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

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
    /// Formatter static class.
    /// </summary>
    public static partial class Formatter
    {
        /// <summary>
        /// To the bytes.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static byte[] ToBytes(this object obj)
        {
            return null;
        }

        /// <summary>
        /// To the object.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static object ToObject(this byte[] bytes, Type type)
        {
            return null;
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
        /// To the XML.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static string ToXml(this object obj)
        {
            return null;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static string ToJson(this object obj, Settings settings = null)
        {
            return ToJsonEx(obj, settings);
        }

        /// <summary>
        /// To the object.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="type">The type.</param>
        /// <param name="format">The format.</param>
        /// <returns></returns>
        public static object ToObject(this string source, Type type, ContentFormat format = ContentFormat.Xml, Settings settings = null)
        {
            return null;
            
        }

        /// <summary>
        /// To the object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="format">The format.</param>
        /// <param name="settings">The settings.</param>
        /// <returns></returns>
        public static T ToObject<T>(this string source, ContentFormat format = ContentFormat.Xml, Settings settings = null)
        {
            var t = typeof(T);

            if (format == ContentFormat.Json)
            {
                var obj = ToObjectFormJson<T>(source, settings);
                if (obj is T)
                    return (T)obj;
                else
                    return default(T);
            }
            else
            {
                var obj = ToObject(source, typeof(T), format, settings);
                if (obj is T)
                    return (T)obj;
                else
                    return default(T);
            }
        }
    }
}
