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
using DragonScale.Portable.Formatters.Json;

namespace DragonScale.Portable.Formatters
{
    /// <summary>
    /// FormatterFactory static class.
    /// </summary>
    public static partial class FormatterFactory
    {
        //ToJsonEx //TODO;这个方法的用途。
        private static string ToJsonEx(object obj, Settings settings = null)
        {
            if (settings == null)
                settings = new JsonFormatterSettings();
            var json = JsonMapper.ToJson(obj, settings);
            if (Arrangement.None.Equals(settings.OutputArrangement))
            {
                return json;
            }
            else
            {
                return json;
                //return new JsonOutputFormatter().PrettyPrint(json);
            }
        }

        //ToObjectFormJson
        private static object ToObjectFormJson<T>(string source, Settings settings = null)
        {
            if (settings == null)
                settings = new JsonFormatterSettings();
            return JsonMapper.ToObject<T>(source, settings);
        }
        //ToObjectFormJson
        private static object ToObjectFormJson(string source, Type type, Settings settings = null)
        {
            if (settings == null)
                settings = new JsonFormatterSettings();
            return JsonMapper.ToObject(source, type, settings);
        }
    }

    #region Settings area
    /// <summary>
    /// Serialize MetaData with Ignore, SerializeBurst or SerializeTiny Mode
    /// </summary>
    public enum SerializeMetaData
    {
        /// <summary>
        /// Ignore Interface MetaData and do not serialize.
        /// </summary>
        Ignore,

        /// <summary>
        /// The serialize burst
        /// </summary>
        SerializeBurst,

        /// <summary>
        /// The serialize tiny
        /// </summary>
        SerializeTiny
    }


    /// <summary>
    /// Specifies reference loop handling options  .
    /// </summary>
    public enum ReferenceLoopHandling
    {
        /// <summary>
        /// Serialize loop references.
        /// </summary>
        Serialize,

        /// <summary>
        /// Ignore loop references and do not serialize.
        /// </summary>
        Ignore
    }

    /// <summary>
    /// Arrangement enum.
    /// </summary>
    [Flags]
    public enum Arrangement
    {
        /// <summary>
        /// Do nothing
        /// </summary>
        None,

        /// <summary>
        /// Beautify the output
        /// </summary>
        Beautify
    }
    #endregion
}