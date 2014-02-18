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
