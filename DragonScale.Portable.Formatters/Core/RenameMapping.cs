using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DragonScale.Portable.Formatters.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class RenameMapping
    {
        internal string _jsonName;
        internal string _typeMemberName;

        /// <summary>
        /// Initializes a new instance of the <see cref="RenameMapping" /> class.
        /// </summary>
        /// <param name="typeMemberName">Name of the type member.</param>
        /// <param name="jsonName">Name of the json.</param>
        public RenameMapping(string typeMemberName, string jsonName)
        {
            _jsonName = jsonName;
            _typeMemberName = typeMemberName;
        }

    }
}
