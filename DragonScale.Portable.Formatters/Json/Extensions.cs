/***
 * Author: Aliqi
 * E-mail: aliqi@hotmail.com
 * Created Time: 2012-10-01
 * Copyright: If you want to use this module, please retain this comment.
 * You can change any code of this file and add your name to the developers list,
 * but you cannot delete this comment or modify all content above.
 * Here's the Developers List:
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using DragonScale.Portable.Formatters.Json;

namespace DragonScale.Portable.Formatters
{
    /// <summary>
    /// FormatterFactory static class.
    /// </summary>
    public static partial class FormatterFactory
    {
        //ToJsonEx
        private static string ToJsonEx(object obj, Settings settings = null)
        {
            if (settings == null)
                settings = new JsonFormatterSettings();
            var json = JsonMapper.ToJson(obj, settings);
            if (JsonOutputFormatterEnum.NoFormatterOutput.Equals(settings.JsonOutputFormaterEnum))
            {
                return json;
            }
            else
            {
                return new JsonOutputFormatter().PrettyPrint(json);
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
    /// Ignore Default MemberInfo
    /// </summary>
    public enum IgnoreNullMemberInfo
    {
        /// <summary>
        /// The serialize
        /// </summary>
        Serialize,

        /// <summary>
        /// The ignore
        /// </summary>
        Ignore
    }



    /// <summary>
    /// JsonOutputFormater Enum
    /// </summary>
    public enum JsonOutputFormatterEnum
    {

        /// <summary>
        /// The formatter output
        /// </summary>
        FormatterOutput,


        /// <summary>
        /// The no formatter output
        /// </summary>
        NoFormatterOutput
    }
    #endregion
}