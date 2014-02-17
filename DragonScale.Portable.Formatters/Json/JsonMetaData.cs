using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DragonScale.Portable.Formatters.Json
{
    /// <summary>
    /// Json Meta Data
    /// </summary>
    public class JsonMetaData : Dictionary<string, string>
    {
        private Dictionary<string, string> dict = new Dictionary<string, string>();

        public JsonMetaData() { }

        public JsonMetaData(Dictionary<string, string> dict)
        {
            foreach (var item in dict)
            {
                this.Add(item.Key, item.Value);
            }
        }

        public string GetKeyByValue(string value)
        {
            if (dict.Count != this.Count)
            {
                dict.Clear();
                foreach (var item in this)
                {
                    dict.Add(item.Value, item.Key);
                }
            }
            return dict[value];
        }


    }
}
