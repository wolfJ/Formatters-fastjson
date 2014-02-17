/***
 * Author: Aliqi
 * E-mail: aliqi@hotmail.com
 * Created Time: 2012-10-01
 * Copyright: If you want to use this module, please retain this comment.
 * You can change any code of this file and add your name to the developers list,
 * but you cannot delete this comment or modify all content above.
 * Here's the Developers List:
 */

using DragonScale.Portable.Formatters.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using DragonScale.Portable.Formatters.Json;
using System.Reflection;

namespace DragonScale.Portable.Formatters
{
    public delegate List<Assembly> LoadTypeFromAssemblies();
    /// <summary>
    /// Settings abstract class.
    /// </summary>
    public abstract class Settings
    {

        #region Fields

        [EditorBrowsable(EditorBrowsableState.Never)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IList<Type> _attributesToIgnore;

        [EditorBrowsable(EditorBrowsableState.Never)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ReferenceLoopHandling _referenceLoopHandling;

        [EditorBrowsable(EditorBrowsableState.Never)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ContentProvider _contentProvider;

        [EditorBrowsable(EditorBrowsableState.Never)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private JsonMetaData _jsonMetaData;

        [EditorBrowsable(EditorBrowsableState.Never)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int _max_nesting_depth = 100;

        public event LoadTypeFromAssemblies LoadTypeFromAssemblies;
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the max nesting depth.
        /// </summary>
        /// <value>
        /// The max nesting depth.
        /// </value>
        public int MaxNestingDepth
        {
            get { return _max_nesting_depth; }
            set { _max_nesting_depth = value; }
        }

        /// <summary>
        /// The serialization of a JSON, can make the metadata of the type can also be serialized
        /// </summary>
        public SerializeMetaData SerializeMetaData { get; set; }

        /// <summary>
        /// Gets or sets the json output formater enum.
        /// </summary>
        /// <value>
        /// The json output formater enum.
        /// </value>
        public JsonOutputFormatterEnum JsonOutputFormaterEnum { get; set; }

        /// <summary>
        ///   All float numbers and date/time values are stored as text according to the Culture. Default is CultureInfo.InvariantCulture.
        ///   This setting is overridden if you set SimpleValueConverter
        /// </summary>
        public CultureInfo Culture { get; set; }

        /// <summary>
        ///   Describes format in which the xml file is stored. Default is UTF-8.
        ///   This setting is overridden if you set XmlWriterSettings
        /// </summary>
        public Encoding Encoding { get; set; }


        /// <summary>
        /// Gets the json meta data.
        /// </summary>
        /// <value>
        /// The json meta data.
        /// </value>
        internal JsonMetaData JsonMetaData
        {
            get
            {
                if (_jsonMetaData == null && SerializeMetaData == Formatters.SerializeMetaData.SerializeTiny)
                    _jsonMetaData = new JsonMetaData();
                return _jsonMetaData;
            }
        }

        /// <summary>
        /// ContentProvider
        /// </summary>
        public ContentProvider ContentProvider
        {
            get
            {
                //default Content provider is portable.
                if (_contentProvider == null) _contentProvider = new DefaultContentProvider(this);
                return _contentProvider;
            }
            set
            {
                _contentProvider = value;
            }
        }

        /// <summary>
        /// All Properties marked with one of the contained attribute-types will be ignored on save.
        /// As default, this list contains only TransientAttribute.
        /// For performance reasons it would be better to clear this list if this attribute 
        /// is not used in serialized classes.
        /// </summary>
        public IList<Type> IgnoredAttributes
        {
            get
            {
                if (_attributesToIgnore == null) _attributesToIgnore = new List<Type>();
                return _attributesToIgnore;
            }
            set { _attributesToIgnore = value; }
        }

        /// <summary>
        /// Reference Loop Handling
        /// </summary>
        public ReferenceLoopHandling ReferenceLoopHandling
        {
            get
            {
                //if (_ReferenceLoopHandling == null) _ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                return _referenceLoopHandling;
            }
            set
            {
                _referenceLoopHandling = value;
            }
        }

        /// <summary>
        /// Gets or sets the ignore null member info.
        /// </summary>
        /// <value>
        /// The ignore null member info.
        /// </value>
        public IgnoreNullMemberInfo IgnoreNullMemberInfo { get; set; }


        #region 暂时未实现
        ///// <summary>
        /////   Converts Type to string and vice versa. Default is an instance of TypeNameConverter which serializes Types as "type name, assembly name"
        /////   If you want to serialize your objects as fully qualified assembly name, you should set this setting with an instance of TypeNameConverter
        /////   with overloaded constructor.
        ///// </summary>
        //public ITypeConverter TypeNameConverter { get; set; }

        ///// <summary>
        /////   Version=x.x.x.x will be inserted to the type name
        ///// </summary>
        //public bool IncludeAssemblyVersionInTypeName { get; set; }

        ///// <summary>
        /////   Culture=.... will be inserted to the type name
        ///// </summary>
        //public bool IncludeCultureInTypeName { get; set; }

        ///// <summary>
        /////   PublicKeyToken=.... will be inserted to the type name
        ///// </summary>
        //public bool IncludePublicKeyTokenInTypeName { get; set; }
        #endregion


        #endregion

        #region Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="Settings"/> class.
        /// </summary>
        protected Settings()
        {
            IgnoredAttributes.Add(typeof(TransientAttribute));
            Encoding = Encoding.UTF8;
            Culture = CultureInfo.InvariantCulture;
            //IncludeAssemblyVersionInTypeName = true;
            //IncludeCultureInTypeName = true;
            //IncludePublicKeyTokenInTypeName = true;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Adds the ignore member, include fields and propertys.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="memberName">Name of the member.</param>
        public void AddIgnoreMember(Type type, string memberName)
        {
            var field = type.GetField(memberName, this.ContentProvider.GetBindingFlags());
            if (field == null)
            {
                _contentProvider.IgnoredFields.Add(field);
            }

            var prop = type.GetProperty(memberName, this.ContentProvider.GetBindingFlags());
            if (prop == null)
            {
                _contentProvider.IgnoredProperties.Add(prop);
            }
        }

        /// <summary>
        /// Gets the serialized metadata json string.
        /// </summary>
        /// <returns>metadata json</returns>
        public string GetSerializeMetaDataModeJson()
        {
            if (_jsonMetaData == null)
                return null;
            var set = JsonFormatterSettings.Default;
            return _jsonMetaData.ToJson(set);
        }

        /// <summary>
        /// Sets the serialize meta data mode json.
        /// </summary>
        /// <param name="metadataJson">The metadata json.</param>
        /// <exception cref="JsonException">SerializeMetaDataModeJson can not be empty or null!</exception>
        public void SetSerializeMetaDataModeJson(string metadataJson)
        {
            if (string.IsNullOrEmpty(metadataJson))
                throw new JsonException("SerializeMetaDataModeJson can not be null or empty!");
            var dict = metadataJson.ToObject<Dictionary<string, string>>(ContentFormat.Json);
            if (dict == null)
                throw new JsonException("SerializeMetaDataModeJson can not be convert JsonMetaData!");
            _jsonMetaData = new JsonMetaData(dict);
        }

        /// <summary>
        /// Replaces the meta data mode.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="replaceType">Type of the replace.</param>
        /// <returns>if JsonMetaData do not contains key , then return false; else return true.</returns>
        public bool ReplaceMetaDataMode(string key, Type replaceType)
        {
            if (JsonMetaData.ContainsKey(key))
            {
                JsonMetaData[key] = replaceType.AssemblyQualifiedName;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void AddKeyRenameMappingObjToJson(Type type, string memberName, string renameMemberName)
        {
            if (type == null || string.IsNullOrEmpty(memberName) || string.IsNullOrEmpty(renameMemberName))
                throw new JsonException("parameters can not be null or empty!");
            if (memberName.Equals(renameMemberName)) return;
            if (!validateJsonKeyWorld(renameMemberName))
                throw new JsonException(string.Format("renameMemberName :{0} is Formatters`s key world!", renameMemberName));

            if (!_contentProvider.KeyRenameMapping.ContainsKey(type))
            {
                _contentProvider.KeyRenameMapping.Add(type, new List<RenameMapping>());
            }
            _contentProvider.KeyRenameMapping[type].Add(new RenameMapping(memberName, renameMemberName));
        }

        public void AddKeyRenameMappingJsonToObj(Type type, string memberName, string jsonMemberName)
        {
            if (type == null || string.IsNullOrEmpty(memberName) || string.IsNullOrEmpty(jsonMemberName))
                throw new JsonException("parameters can not be null or empty!");
            if (memberName.Equals(jsonMemberName)) return;
            if (!_contentProvider.KeyRenameMapping.ContainsKey(type))
            {
                _contentProvider.KeyRenameMapping.Add(type, new List<RenameMapping>());
            }
            _contentProvider.KeyRenameMapping[type].Add(new RenameMapping(memberName, jsonMemberName));
        }
        #endregion

        #region Private Methods

        private bool validateJsonKeyWorld(string renameMemberName)
        {
            if (renameMemberName.StartsWith("$"))
                return false;
            return true;
        }

        List<System.Reflection.Assembly> asses = null;
        internal Type DoLoadTypeFromAssemblies(string typeQualifiedName)
        {
            GenericTypeModel typeModel = new GenericTypeModel(typeQualifiedName);
            Type retType = null;
            if (asses == null && LoadTypeFromAssemblies != null)
            {
                asses = LoadTypeFromAssemblies();
            }
            retType = typeModel.GetModelType(asses);
            return retType;
        }
        #endregion
    }

    /// <summary>
    /// BinaryFormatterSettings class.
    /// </summary>
    public class BinaryFormatterSettings : Settings { }

    /// <summary>
    /// JsonFormatterSettings class.
    /// </summary>
    public class JsonFormatterSettings : Settings
    {
        /// <summary>
        /// Gets the default.
        /// </summary>
        /// <value>
        /// The default.
        /// </value>
        public static JsonFormatterSettings Default
        {
            get
            {
                var settings = new JsonFormatterSettings();
                settings.ContentProvider = new DefaultContentProvider(settings);
                return settings;
            }
        }
    }

    /// <summary>
    /// XmlFormatterSettings class.
    /// </summary>
    public class XmlFormatterSettings : Settings { }
}