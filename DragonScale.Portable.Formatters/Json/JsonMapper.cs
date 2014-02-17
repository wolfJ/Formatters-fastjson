using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Globalization;
using System.Collections;
using System.IO;

namespace DragonScale.Portable.Formatters.Json
{
    internal struct PropertyMetadata
    {
        public MemberInfo Info;
        public bool IsField;
        public Type Type;
    }

    internal struct ArrayMetadata
    {
        private Type element_type;
        private bool is_array;
        private bool is_list;


        public Type ElementType
        {
            get
            {
                if (element_type == null)
                    return typeof(JsonData);

                return element_type;
            }

            set { element_type = value; }
        }

        public bool IsArray
        {
            get { return is_array; }
            set { is_array = value; }
        }

        public bool IsList
        {
            get { return is_list; }
            set { is_list = value; }
        }
    }

    internal struct ObjectMetadata
    {
        private Type element_type;
        private bool is_dictionary;

        private IDictionary<string, PropertyMetadata> properties;


        public Type ElementType
        {
            get
            {
                if (element_type == null)
                    return typeof(JsonData);

                return element_type;
            }

            set { element_type = value; }
        }

        public bool IsDictionary
        {
            get { return is_dictionary; }
            set { is_dictionary = value; }
        }

        public IDictionary<string, PropertyMetadata> Properties
        {
            get { return properties; }
            set { properties = value; }
        }
    }

    internal delegate void ExporterFunc(object obj, JsonWriter writer);
    /// <summary>
    /// ExporterFunc
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj">The obj.</param>
    /// <param name="writer">The writer.</param>
    public delegate void ExporterFunc<T>(T obj, JsonWriter writer);
    internal delegate object ImporterFunc(object input);
    /// <summary>
    /// ImporterFunc
    /// </summary>
    /// <typeparam name="TJson">The type of the json.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="input">The input.</param>
    /// <returns></returns>
    public delegate TValue ImporterFunc<TJson, TValue>(TJson input);

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public delegate JsonWrapper WrapperFactory();


    class JsonMapper
    {

        #region Fields
        private const string OBJECT_ID = "$F_ID";
        private const string OBJECT_SUB_TYPE = "$F_TYPE";


        private static int max_nesting_depth;
        private static IFormatProvider datetime_format;

        private static IDictionary<Type, ExporterFunc> base_exporters_table;
        private static IDictionary<Type, ExporterFunc> custom_exporters_table;
        private static IDictionary<Type, IDictionary<Type, ImporterFunc>> base_importers_table;
        private static IDictionary<Type, IDictionary<Type, ImporterFunc>> custom_importers_table;

        private static IDictionary<Type, ArrayMetadata> array_metadata;
        private static readonly object array_metadata_lock = new Object();

        private static IDictionary<Type, IDictionary<Type, MethodInfo>> conv_ops;
        private static readonly object conv_ops_lock = new Object();

        private static IDictionary<Type, ObjectMetadata> object_metadata;
        private static readonly object object_metadata_lock = new Object();

        private static IDictionary<Type, IList<PropertyMetadata>> type_properties;
        private static readonly object type_properties_lock = new Object();

        private static JsonWriter static_writer;
        private static readonly object static_writer_lock = new Object();

        private static Settings settings;
        #endregion

        #region Constructors
        static JsonMapper()
        {
            //max_nesting_depth = 100;

            array_metadata = new Dictionary<Type, ArrayMetadata>();
            conv_ops = new Dictionary<Type, IDictionary<Type, MethodInfo>>();
            object_metadata = new Dictionary<Type, ObjectMetadata>();
            type_properties = new Dictionary<Type,
                            IList<PropertyMetadata>>();

            static_writer = new JsonWriter();

            datetime_format = DateTimeFormatInfo.InvariantInfo;

            base_exporters_table = new Dictionary<Type, ExporterFunc>();
            custom_exporters_table = new Dictionary<Type, ExporterFunc>();

            base_importers_table = new Dictionary<Type, IDictionary<Type, ImporterFunc>>();
            custom_importers_table = new Dictionary<Type, IDictionary<Type, ImporterFunc>>();

            RegisterBaseExporters();
            RegisterBaseImporters();
            RegisterCustomImporters();
        }
        #endregion

        #region Private Methods

        private static void AddArrayMetadata(Type type)
        {
            if (array_metadata.ContainsKey(type))
                return;

            ArrayMetadata data = new ArrayMetadata();

            data.IsArray = type.IsArray;

            if (typeof(System.Collections.ICollection).IsAssignableFrom(type) ||
                typeof(ICollection<>).IsAssignableFrom(type))
            {
                data.IsList = true;
            }

            foreach (PropertyInfo p_info in type.GetProperties())
            {
                if (p_info.Name != "Item")
                    continue;

                ParameterInfo[] parameters = p_info.GetIndexParameters();

                if (parameters.Length != 1)
                    continue;

                if (parameters[0].ParameterType == typeof(int))
                    data.ElementType = p_info.PropertyType;
            }

            lock (array_metadata_lock)
            {
                try
                {
                    array_metadata.Add(type, data);
                }
                catch (ArgumentException)
                {
                    return;
                }
            }
        }

        private static void AddObjectMetadata(Type type, object pro)
        {
            if (object_metadata.ContainsKey(type))
                return;

            ObjectMetadata data = new ObjectMetadata();
            var types = type.GetInterfaces();
            if (types != null)
            {
                foreach (var item in types)
                {
                    if (item.FullName.Contains("System.Collections.IDictionary"))
                        data.IsDictionary = true; break;
                }
            }
            if ((typeof(IDictionary)).IsAssignableFrom(type) ||
                (typeof(System.Collections.Generic.IDictionary<,>)).IsAssignableFrom(type))
                data.IsDictionary = true;

            data.Properties = new Dictionary<string, PropertyMetadata>();

            FlattenHierarchyProxy fp = new FlattenHierarchyProxy(pro, settings);
            var items = fp.Items;

            foreach (var item in items)
            {
                if (item.IsField)
                {
                    PropertyMetadata p_data = new PropertyMetadata();
                    p_data.Info = item.Info;
                    p_data.IsField = item.IsField;
                    p_data.Type = item.Type;
                    data.Properties.Add(item.Name, p_data);
                }
                else
                {
                    var p_info = ((PropertyInfo)item.Info);
                    if (item.Name == "Item")
                    {

                        ParameterInfo[] parameters = p_info.GetIndexParameters();

                        if (parameters.Length != 1)
                            continue;

                        if (parameters[0].ParameterType == typeof(string))
                            data.ElementType = p_info.PropertyType;

                        continue;
                    }

                    PropertyMetadata p_data = new PropertyMetadata();
                    p_data.Info = p_info;
                    p_data.Type = p_info.PropertyType;

                    data.Properties.Add(p_info.Name, p_data);
                }
            }

            #region delete
            //var typex = type;
            //List<string> nameList = new List<string>();
            //while (typex != (typeof(Object)))
            //{
            //    var fields = settings.ContentProvider.GetFields(type);
            //    foreach (var f_info in fields)
            //    {
            //        if (!nameList.Contains(f_info.Name))
            //        {
            //            nameList.Add(f_info.Name);


            //            PropertyMetadata p_data = new PropertyMetadata();
            //            p_data.Info = f_info;
            //            p_data.IsField = true;
            //            p_data.Type = f_info.FieldType;

            //            data.Properties.Add(f_info.Name, p_data);
            //        }
            //    }

            //    var properties = settings.ContentProvider.GetProperties(type);
            //    foreach (var p_info in properties)
            //    {
            //        if (!nameList.Contains(p_info.Name))
            //        {
            //            nameList.Add(p_info.Name);
            //            if (p_info.Name == "Item")
            //            {
            //                ParameterInfo[] parameters = p_info.GetIndexParameters();

            //                if (parameters.Length != 1)
            //                    continue;

            //                if (parameters[0].ParameterType == typeof(string))
            //                    data.ElementType = p_info.PropertyType;

            //                continue;
            //            }

            //            PropertyMetadata p_data = new PropertyMetadata();
            //            p_data.Info = p_info;
            //            p_data.Type = p_info.PropertyType;

            //            data.Properties.Add(p_info.Name, p_data);
            //        }
            //    }
            //    typex = typex.BaseType;
            //}
            #endregion

            lock (object_metadata_lock)
            {
                try
                {
                    object_metadata.Add(type, data);
                }
                catch (ArgumentException)
                {
                    return;
                }
            }
        }

        private static void AddTypeProperties(Type type, object pro)
        {
            if (type_properties.ContainsKey(type))
                return;

            FlattenHierarchyProxy fp = new FlattenHierarchyProxy(pro, settings);
            var items = fp.Items;

            IList<PropertyMetadata> props = new List<PropertyMetadata>();
            foreach (var item in items)
            {
                PropertyMetadata p_data = new PropertyMetadata();
                p_data.Info = item.Info;
                p_data.IsField = item.IsField;
                props.Add(p_data);
            }

            lock (type_properties_lock)
            {
                try
                {
                    type_properties.Add(type, props);
                }
                catch (ArgumentException)
                {
                    return;
                }
            }
        }

        private static MethodInfo GetConvOp(Type t1, Type t2)
        {
            lock (conv_ops_lock)
            {
                if (!conv_ops.ContainsKey(t1))
                    conv_ops.Add(t1, new Dictionary<Type, MethodInfo>());
            }

            if (conv_ops[t1].ContainsKey(t2))
                return conv_ops[t1][t2];

            MethodInfo op = t1.GetMethod("op_Implicit", new Type[] { t2 });

            lock (conv_ops_lock)
            {
                try
                {
                    conv_ops[t1].Add(t2, op);
                }
                catch (ArgumentException)
                {
                    return conv_ops[t1][t2];
                }
            }

            return op;
        }

        private static object ReadValue(Type inst_type, JsonReader reader)
        {
            reader.Read();

            if (reader.Token == JsonToken.ArrayEnd)
                return null;

            if (reader.Token == JsonToken.Null)
            {

                if (!inst_type.IsClass && !inst_type.IsValueType)
                {
                    throw new JsonException(String.Format(
                            "Can't assign null to an instance of type {0}",
                            inst_type));
                }

                return null;
            }

            if (reader.Token == JsonToken.Double ||
                reader.Token == JsonToken.Int ||
                reader.Token == JsonToken.Long ||
                reader.Token == JsonToken.String ||
                reader.Token == JsonToken.Boolean)
            {

                Type json_type = reader.Value.GetType();

                if (inst_type.IsAssignableFrom(json_type))
                    return reader.Value;

                // If there's a custom importer that fits, use it
                if (custom_importers_table.ContainsKey(json_type) &&
                    custom_importers_table[json_type].ContainsKey(
                        inst_type))
                {

                    ImporterFunc importer =
                        custom_importers_table[json_type][inst_type];

                    return importer(reader.Value);
                }

                // Maybe there's a base importer that works
                if (base_importers_table.ContainsKey(json_type) &&
                    base_importers_table[json_type].ContainsKey(
                        inst_type))
                {

                    ImporterFunc importer =
                        base_importers_table[json_type][inst_type];

                    return importer(reader.Value);
                }

                // Maybe it's an enum
                if (inst_type.IsEnum)
                    return Enum.ToObject(inst_type, reader.Value);

                // Try using an implicit conversion operator
                MethodInfo conv_op = GetConvOp(inst_type, json_type);

                if (conv_op != null)
                    return conv_op.Invoke(null,
                                           new object[] { reader.Value });

                // No luck
                throw new JsonException(String.Format(
                        "Can't assign value '{0}' (type {1}) to type {2}",
                        reader.Value, json_type, inst_type));
            }

            object instance = null;

            if (reader.Token == JsonToken.ArrayStart)
            {

                AddArrayMetadata(inst_type);
                ArrayMetadata t_data = array_metadata[inst_type];

                if (!t_data.IsArray && !t_data.IsList)
                {
                    throw new JsonException(String.Format(
                            "Type {0} can't act as an array",
                            inst_type));
                }

                IList list;
                Type elem_type;

                if (!t_data.IsArray)
                {
                    list = (IList)Activator.CreateInstance(inst_type);
                    elem_type = t_data.ElementType;
                }
                else
                {
                    //list = new ArrayList();
                    list = new List<object>();
                    elem_type = inst_type.GetElementType();
                }

                while (true)
                {
                    object item = ReadValue(elem_type, reader);
                    if (reader.Token == JsonToken.ArrayEnd)
                        break;

                    list.Add(item);
                }

                if (t_data.IsArray)
                {
                    int n = list.Count;
                    instance = Array.CreateInstance(elem_type, n);

                    for (int i = 0; i < n; i++)
                        ((Array)instance).SetValue(list[i], i);
                }
                else
                    instance = list;

            }
            else if (reader.Token == JsonToken.ObjectStart)
            {
                bool isReferanceLoop = (JsonMapper.settings.ReferenceLoopHandling == ReferenceLoopHandling.Serialize);
                //create instance
                bool hasRead = false;
                if (settings.SerializeMetaData != SerializeMetaData.Ignore)
                {
                    reader.Read();
                    hasRead = true;
                    if (reader.Token == JsonToken.ObjectEnd)
                    {
                    }
                    else
                    {
                        string property = (string)reader.Value;
                        if (OBJECT_SUB_TYPE.Equals(property))
                        {
                            var subTypeStr = (string)ReadValue(typeof(string), reader);
                            if (settings.SerializeMetaData == SerializeMetaData.SerializeTiny)
                            {
                                if (settings.JsonMetaData.ContainsKey(subTypeStr))
                                {
                                    subTypeStr = settings.JsonMetaData[subTypeStr];
                                }
                                else
                                {
                                    throw new JsonException(String.Format("SerializeMetaData is SerializeTiny, but can not found JsonMetaData key :{0}", subTypeStr));
                                }
                            }

                            var objectSubType = Type.GetType(subTypeStr);
                            if (objectSubType == null)
                            {
                                objectSubType = settings.DoLoadTypeFromAssemblies(subTypeStr);
                            }
                            if (objectSubType == null)
                            {
                                throw new Exception("Can`t load Type [" + subTypeStr + "]. \nYou can implement function Settings.LoadTypeFromAssemblies !");
                            }
                            instance = Activator.CreateInstance(objectSubType);
                            reader.Read();//重新read next;
                        }
                        else
                        {
                            instance = Activator.CreateInstance(inst_type);
                        }
                    }
                }
                else
                {
                    instance = Activator.CreateInstance(inst_type);
                }

                if (settings.SerializeMetaData != SerializeMetaData.Ignore)
                {
                    inst_type = instance.GetType();
                }
                AddObjectMetadata(inst_type, instance);
                ObjectMetadata t_data = object_metadata[inst_type];
                while (true)
                {
                    if (!hasRead)
                    {
                        reader.Read();
                    }
                    else
                    {
                        hasRead = false;
                    }

                    if (reader.Token == JsonToken.ObjectEnd)
                        break;


                    string property = (string)reader.Value;
                    property = readPropertyName(inst_type, property);

                    //check Referance Loop
                    if (OBJECT_ID.Equals(property))
                    {
                        //do un reference loop
                        int obj_id_int = (Int32)ReadValue(typeof(Int32), reader);
                        if (isReferanceLoop)
                        {
                            if (!reader.ObjFlagDict.Keys.Contains(OBJECT_ID + ":" + obj_id_int))
                            {
                                reader.ObjFlagDict.Add(OBJECT_ID + ":" + obj_id_int, instance);
                            }
                            else
                            {
                                //if this object has added, then change instance, and continue to read JsonToken.ObjectEnd.
                                instance = reader.ObjFlagDict[OBJECT_ID + ":" + obj_id_int];
                                continue;
                            }
                        }
                    }
                    else if (t_data.Properties.ContainsKey(property))
                    {
                        PropertyMetadata prop_data = t_data.Properties[property];
                        if (prop_data.Type.FullName == "System.Type")
                        {
                            if (prop_data.IsField)
                            {
                                ((FieldInfo)prop_data.Info).SetValue(
                                    instance, Type.GetType((string)ReadValue(typeof(string), reader)));
                            }
                            else
                            {
                                PropertyInfo p_info =
                                    (PropertyInfo)prop_data.Info;

                                if (p_info.CanWrite)
                                {
                                    var fullName = (string)ReadValue(typeof(string), reader);
                                    Type tempType = null;
                                    try
                                    {
                                        tempType = settings.DoLoadTypeFromAssemblies(fullName);
                                    }
                                    catch (Exception e)
                                    {
                                        FormatterFactory.ErrorLog(e.Message);
                                    }
                                    p_info.SetValue(
                                        instance,
                                       tempType,
                                        null);
                                }
                                else
                                    ReadValue(prop_data.Type, reader);
                            }
                        }
                        else
                        {
                            if (prop_data.IsField)
                            {
                                ((FieldInfo)prop_data.Info).SetValue(
                                    instance, ReadValue(prop_data.Type, reader));
                            }
                            else
                            {
                                PropertyInfo p_info =
                                    (PropertyInfo)prop_data.Info;

                                if (p_info.CanWrite)
                                    p_info.SetValue(
                                        instance,
                                        ReadValue(prop_data.Type, reader),
                                        null);
                                else
                                    ReadValue(prop_data.Type, reader);
                            }
                        }
                    }
                    else
                    {
                        if (!t_data.IsDictionary)
                        {
                            throw new JsonException(String.Format(
                                    "The type {0} doesn't have the " +
                                    "property '{1}'", inst_type, property));
                        }
                        if (((IDictionary)instance).GetType().IsGenericType)
                        {
                            var types = ((IDictionary)instance).GetType().GetGenericArguments();
                            ((IDictionary)instance).Add(property, ReadValue(
                                    types[1], reader));
                        }
                        else
                        {
                            ((IDictionary)instance).Add(
                                property, ReadValue(
                                    t_data.ElementType, reader));
                        }
                    }

                }

            }

            return instance;
        }

        private static string readPropertyName(Type inst_type, string property)
        {
            if (settings.ContentProvider.KeyRenameMapping.Count > 0
                && settings.ContentProvider.KeyRenameMapping.ContainsKey(inst_type))
            {
                foreach (var item in settings.ContentProvider.KeyRenameMapping[inst_type])
                {
                    if (item._jsonName.Equals(property))
                    {
                        return item._typeMemberName;
                    }
                }
            }
            return property;
        }

        private static JsonWrapper ReadValue(WrapperFactory factory,
                                               JsonReader reader)
        {
            reader.Read();

            if (reader.Token == JsonToken.ArrayEnd ||
                reader.Token == JsonToken.Null)
                return null;

            JsonWrapper instance = factory();

            if (reader.Token == JsonToken.String)
            {
                instance.SetString((string)reader.Value);
                return instance;
            }

            if (reader.Token == JsonToken.Double)
            {
                instance.SetDouble((double)reader.Value);
                return instance;
            }

            if (reader.Token == JsonToken.Int)
            {
                instance.SetInt((int)reader.Value);
                return instance;
            }

            if (reader.Token == JsonToken.Long)
            {
                instance.SetLong((long)reader.Value);
                return instance;
            }

            if (reader.Token == JsonToken.Boolean)
            {
                instance.SetBoolean((bool)reader.Value);
                return instance;
            }

            if (reader.Token == JsonToken.ArrayStart)
            {
                instance.SetJsonType(JsonType.Array);

                while (true)
                {
                    JsonWrapper item = ReadValue(factory, reader);
                    if (reader.Token == JsonToken.ArrayEnd)
                        break;

                    ((IList)instance).Add(item);
                }
            }
            else if (reader.Token == JsonToken.ObjectStart)
            {
                instance.SetJsonType(JsonType.Object);

                while (true)
                {
                    reader.Read();

                    if (reader.Token == JsonToken.ObjectEnd)
                        break;

                    string property = (string)reader.Value;

                    ((IDictionary)instance)[property] = ReadValue(
                        factory, reader);
                }

            }

            return instance;
        }


        private static void RegisterBaseExporters()
        {
            base_exporters_table[typeof(byte)] =
                delegate(object obj, JsonWriter writer)
                {
                    writer.Write(Convert.ToInt32((byte)obj));
                };

            base_exporters_table[typeof(char)] =
                delegate(object obj, JsonWriter writer)
                {
                    writer.Write(Convert.ToString((char)obj));
                };

            base_exporters_table[typeof(DateTime)] =
                delegate(object obj, JsonWriter writer)
                {
                    writer.Write(Convert.ToString((DateTime)obj,
                                                    datetime_format));
                };

            base_exporters_table[typeof(decimal)] =
                delegate(object obj, JsonWriter writer)
                {
                    writer.Write((decimal)obj);
                };

            base_exporters_table[typeof(sbyte)] =
                delegate(object obj, JsonWriter writer)
                {
                    writer.Write(Convert.ToInt32((sbyte)obj));
                };

            base_exporters_table[typeof(short)] =
                delegate(object obj, JsonWriter writer)
                {
                    writer.Write(Convert.ToInt32((short)obj));
                };

            base_exporters_table[typeof(ushort)] =
                delegate(object obj, JsonWriter writer)
                {
                    writer.Write(Convert.ToInt32((ushort)obj));
                };

            base_exporters_table[typeof(uint)] =
                delegate(object obj, JsonWriter writer)
                {
                    writer.Write(Convert.ToUInt64((uint)obj));
                };

            base_exporters_table[typeof(ulong)] =
                delegate(object obj, JsonWriter writer)
                {
                    writer.Write((ulong)obj);
                };
        }

        private static void RegisterBaseImporters()
        {
            ImporterFunc importer;

            importer = delegate(object input)
            {
                return Convert.ToByte((int)input);
            };
            RegisterImporter(base_importers_table, typeof(int),
                              typeof(byte), importer);

            importer = delegate(object input)
            {
                return Convert.ToUInt64((int)input);
            };
            RegisterImporter(base_importers_table, typeof(int),
                              typeof(ulong), importer);

            importer = delegate(object input)
            {
                return Convert.ToSByte((int)input);
            };
            RegisterImporter(base_importers_table, typeof(int),
                              typeof(sbyte), importer);

            importer = delegate(object input)
            {
                return Convert.ToInt16((int)input);
            };
            RegisterImporter(base_importers_table, typeof(int),
                              typeof(short), importer);

            importer = delegate(object input)
            {
                return Convert.ToInt64(input);
            };
            RegisterImporter(base_importers_table, typeof(int),
                              typeof(Int64), importer);

            importer = delegate(object input)
            {
                return Convert.ToUInt16((int)input);
            };
            RegisterImporter(base_importers_table, typeof(int),
                              typeof(ushort), importer);

            importer = delegate(object input)
            {
                return Convert.ToUInt32((int)input);
            };
            RegisterImporter(base_importers_table, typeof(int),
                              typeof(uint), importer);

            importer = delegate(object input)
            {
                return Convert.ToSingle((int)input);
            };
            RegisterImporter(base_importers_table, typeof(int),
                              typeof(float), importer);

            importer = delegate(object input)
            {
                return Convert.ToDouble((int)input);
            };
            RegisterImporter(base_importers_table, typeof(int),
                              typeof(double), importer);

            importer = delegate(object input)
            {
                return Convert.ToDecimal((double)input);
            };
            RegisterImporter(base_importers_table, typeof(double),
                              typeof(decimal), importer);


            importer = delegate(object input)
            {
                return Convert.ToUInt32((long)input);
            };
            RegisterImporter(base_importers_table, typeof(long),
                              typeof(uint), importer);

            importer = delegate(object input)
            {
                return Convert.ToChar((string)input);
            };
            RegisterImporter(base_importers_table, typeof(string),
                              typeof(char), importer);

            importer = delegate(object input)
            {
                return Convert.ToDateTime((string)input, datetime_format);
            };
            RegisterImporter(base_importers_table, typeof(string),
                              typeof(DateTime), importer);
        }

        private static void RegisterCustomImporters()
        {
            ImporterFunc importer;
            importer = delegate(object input)
            {
                return Convert.ToDateTime((string)input, datetime_format);
            };
            RegisterImporter(custom_importers_table, typeof(string),
                              typeof(DateTime?), importer);

        }
        private static void RegisterImporter(
            IDictionary<Type, IDictionary<Type, ImporterFunc>> table,
            Type json_type, Type value_type, ImporterFunc importer)
        {
            if (!table.ContainsKey(json_type))
                table.Add(json_type, new Dictionary<Type, ImporterFunc>());

            table[json_type][value_type] = importer;
        }

        //递归的方式，解析每一个对象or字段和属性
        private static void WriteValue(object obj, JsonWriter writer,
                                        bool writer_is_private,
                                        int depth)
        {
            if (depth > settings.MaxNestingDepth)
            {
                throw new JsonException(
                    String.Format("Max allowed object depth reached while " +
                                   "trying to export from type {0}, Default depth is 100. \n Current json is : {1}",
                                   obj.GetType(), writer.ToString()));
            }

            if (obj == null)
            {
                writer.Write(null);
                return;
            }

            if (obj is JsonWrapper)
            {
                if (writer_is_private)
                    writer.TextWriter.Write(((JsonWrapper)obj).ToJson());
                else
                    ((JsonWrapper)obj).ToJson(writer);

                return;
            }

            if (obj is String)
            {
                writer.Write((string)obj);
                return;
            }

            if (obj is Double)
            {
                writer.Write((double)obj);
                return;
            }

            if (obj is Int32)
            {
                writer.Write((int)obj);
                return;
            }

            if (obj is Boolean)
            {
                writer.Write((bool)obj);
                return;
            }

            if (obj is Int64)
            {
                writer.Write((long)obj);
                return;
            }
            if (obj is Type)
            {
                writer.Write(((Type)obj).AssemblyQualifiedName);
                return;
            }
            if (obj is Array)
            {
                writer.WriteArrayStart();

                foreach (object elem in (Array)obj)
                    WriteValue(elem, writer, writer_is_private, depth + 1);

                writer.WriteArrayEnd();

                return;
            }

            if (obj is IList)
            {
                writer.WriteArrayStart();
                foreach (object elem in (IList)obj)
                    WriteValue(elem, writer, writer_is_private, depth + 1);
                writer.WriteArrayEnd();

                return;
            }

            if (obj is IDictionary)
            {
                writer.WriteObjectStart();
                foreach (DictionaryEntry entry in (IDictionary)obj)
                {
                    writer.WritePropertyName((string)entry.Key);
                    WriteValue(entry.Value, writer, writer_is_private,
                                depth + 1);
                }
                writer.WriteObjectEnd();

                return;
            }

            // See if there's a custom exporter for the object
            Type obj_type = obj.GetType();
            if (custom_exporters_table.ContainsKey(obj_type))
            {
                ExporterFunc exporter = custom_exporters_table[obj_type];
                exporter(obj, writer);

                return;
            }

            // If not, maybe there's a base exporter
            if (base_exporters_table.ContainsKey(obj_type))
            {
                ExporterFunc exporter = base_exporters_table[obj_type];
                exporter(obj, writer);

                return;
            }

            // Last option, let's see if it's an enum
            if (obj is Enum)
            {
                Type e_type = Enum.GetUnderlyingType(obj_type);

                if (e_type == typeof(long)
                    || e_type == typeof(uint)
                    || e_type == typeof(ulong))
                    writer.Write((ulong)obj);
                else
                {
                    writer.Write((byte)obj);
                    //writer.Write((int)obj);
                }

                return;
            }

            // Okay, so it looks like the input should be exported as an object
            AddTypeProperties(obj_type, obj);
            IList<PropertyMetadata> props = type_properties[obj_type];

            //check Referance Loop
            if (JsonMapper.settings.ReferenceLoopHandling == ReferenceLoopHandling.Serialize)
            {
                if (!writer.ObjFlagList.Contains(obj))
                {
                    writer.ObjFlagList.Add(obj);
                    writeObjectStart(writer, obj_type);
                    writer.WritePropertyName(OBJECT_ID);
                    writer.Write(writer.ObjFlagList.IndexOf(obj));
                }
                else
                {
                    //if this object has added, then add templete:{object_id:i}, and return.
                    //writeObjectStart(writer, obj_type);//modify by wolf.jiang on 2013-1-23，如果已经加过了，则不需要知道具体的Type信息了。
                    writer.WriteObjectStart();
                    writer.WritePropertyName(OBJECT_ID);
                    writer.Write(writer.ObjFlagList.IndexOf(obj));
                    writer.WriteObjectEnd();
                    return;
                }
            }
            else
            {
                writeObjectStart(writer, obj_type);
            }

            foreach (PropertyMetadata p_data in props)
            {
                string name = p_data.Info.Name;
                object value = null;
                bool canRead = true;
                if (p_data.IsField)
                {
                    value = ((FieldInfo)p_data.Info).GetValue(obj);
                }
                else
                {
                    PropertyInfo p_info = (PropertyInfo)p_data.Info;
                    if (p_info.CanRead)
                    {
                        value = p_info.GetValue(obj, null);
                    }
                    else
                    {
                        canRead = false;
                    }
                }
                if (canRead)
                {
                    //如果设置了忽略NULL信息，则跳过。。。。
                    if (settings.IgnoreNullMemberInfo == IgnoreNullMemberInfo.Ignore && value == null)
                        continue;
                    writePropertyName(writer, obj_type, name);
                    WriteValue(value, writer, writer_is_private, depth + 1);
                }
            }
            writer.WriteObjectEnd();
        }

        private static void writePropertyName(JsonWriter writer, Type obj_type, string name)
        {
            if (settings.ContentProvider.KeyRenameMapping.Count > 0
                && settings.ContentProvider.KeyRenameMapping.ContainsKey(obj_type))
            {
                foreach (var item in settings.ContentProvider.KeyRenameMapping[obj_type])
                {
                    if (item._typeMemberName.Equals(name))
                    {
                        name = item._jsonName;
                        break;
                    }
                }
            }
            writer.WritePropertyName(name);
        }

        private static void writeObjectStart(JsonWriter writer, Type obj_type)
        {
            writer.WriteObjectStart();
            if (settings.SerializeMetaData != SerializeMetaData.Ignore)
            {
                if (obj_type.BaseType != typeof(Object) || obj_type.GetInterfaces().Count() > 0)
                {
                    writer.WritePropertyName(OBJECT_SUB_TYPE);
                    var assemblyQualifiedName = (obj_type).AssemblyQualifiedName;
                    if (settings.SerializeMetaData == SerializeMetaData.SerializeTiny)
                    {
                        if (settings.JsonMetaData.Values.Contains(assemblyQualifiedName))
                        {
                            assemblyQualifiedName = settings.JsonMetaData.GetKeyByValue(assemblyQualifiedName);
                        }
                        else
                        {
                            var current = settings.JsonMetaData.Count.ToString();
                            settings.JsonMetaData.Add(current, assemblyQualifiedName);
                            assemblyQualifiedName = current;
                        }
                    }
                    writer.Write(assemblyQualifiedName);
                }
            }
        }

        #endregion


        public static string ToJson(object obj, Settings settings)
        {
            JsonMapper.settings = settings;
            lock (static_writer_lock)
            {
                static_writer.Reset();

                WriteValue(obj, static_writer, true, 0);

                static_writer.ObjFlagList.Clear();//clear cache
                return static_writer.ToString();
            }
        }


        public static JsonData ToObject(JsonReader reader, Settings settings = null)
        {
            JsonMapper.settings = settings;
            return (JsonData)ToWrapper(
                delegate { return new JsonData(); }, reader);
        }

        public static JsonData ToObject(TextReader reader, Settings settings = null)
        {
            JsonMapper.settings = settings;
            JsonReader json_reader = new JsonReader(reader);

            return (JsonData)ToWrapper(
                delegate { return new JsonData(); }, json_reader);
        }

        public static JsonData ToObject(string json, Settings settings = null)
        {
            JsonMapper.settings = settings;
            return (JsonData)ToWrapper(delegate { return new JsonData(); }, json);
        }

        public static T ToObject<T>(JsonReader reader, Settings settings = null)
        {
            JsonMapper.settings = settings;
            return (T)ReadValue(typeof(T), reader);
        }

        public static T ToObject<T>(TextReader reader, Settings settings = null)
        {
            JsonMapper.settings = settings;
            JsonReader json_reader = new JsonReader(reader);

            return (T)ReadValue(typeof(T), json_reader);
        }

        public static T ToObject<T>(string json, Settings settings = null)
        {
            JsonMapper.settings = settings;
            JsonReader reader = new JsonReader(json);

            return (T)ReadValue(typeof(T), reader);
        }

        public static object ToObject(string json, Type type, Settings settings = null)
        {
            JsonMapper.settings = settings;
            JsonReader reader = new JsonReader(json);

            return ReadValue(type, reader);
        }


        public static JsonWrapper ToWrapper(WrapperFactory factory,
                                              JsonReader reader)
        {
            return ReadValue(factory, reader);
        }

        public static JsonWrapper ToWrapper(WrapperFactory factory,
                                              string json)
        {
            JsonReader reader = new JsonReader(json);

            return ReadValue(factory, reader);
        }

    }

}
