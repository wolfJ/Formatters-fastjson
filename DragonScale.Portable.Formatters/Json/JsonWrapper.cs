using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DragonScale.Portable.Formatters.Json
{
    public enum JsonType
    {
        None,

        Object,
        Array,
        String,
        Int,
        Long,
        Double,
        Boolean
    }

    public interface JsonWrapper : System.Collections.IList
    {
        bool IsArray { get; }
        bool IsBoolean { get; }
        bool IsDouble { get; }
        bool IsInt { get; }
        bool IsLong { get; }
        bool IsObject { get; }
        bool IsString { get; }

        bool GetBoolean();
        double GetDouble();
        int GetInt();
        JsonType GetJsonType();
        long GetLong();
        string GetString();


        void SetBoolean(bool val);
        void SetDouble(double val);
        void SetInt(int val);
        void SetJsonType(JsonType type);
        void SetLong(long val);
        void SetString(string val);

        string ToJson();

        void ToJson(JsonWriter writer);

    }
}
