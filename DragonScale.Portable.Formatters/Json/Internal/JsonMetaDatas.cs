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
using System.Reflection;
using System.Text;

namespace DragonScale.Portable.Formatters.Json
{
    #region PropertyMetadata
    internal struct PropertyMetadata
    {
        public MemberInfo Info;
        public bool IsField;
        public Type Type;
    }
    #endregion

    #region ArrayMetadata
    internal struct ArrayMetadata
    {
        private Type element_type;
        public bool IsArray;
        public bool IsList;

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
    }
    #endregion

    #region ObjectMetadata
    internal struct ObjectMetadata
    {
        private Type element_type;
        public bool IsDictionary;
        public IDictionary<string, PropertyMetadata> Properties;

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
    }
    #endregion
}
