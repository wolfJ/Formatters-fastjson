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
using System.Diagnostics;
using System.Reflection;

namespace DragonScale.Portable.Formatters
{
    public sealed class FlattenHierarchyProxy
    {
        [DebuggerDisplay("{Value}", Name = "{Name,nq}", Type = "{Type.ToString(),nq}")]
        public struct Member
        {
            internal string Name;
            internal object Value;
            internal Type Type;
            internal bool IsField;
            internal MemberInfo Info;
            internal Member(string name, object value, Type type, bool isField, MemberInfo info)
            {
                Name = name;
                Value = value;
                Type = type;
                IsField = isField;
                Info = info;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly object _target;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Settings _settings;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Member[] _memberList;

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public Member[] Items
        {
            get
            {
                if (_memberList == null)
                    _memberList = BuildMemberList().ToArray();
                return _memberList;
            }
        }

        public FlattenHierarchyProxy(object target, Settings settings)
        {
            _target = target;
            _settings = settings;
        }

        private List<Member> BuildMemberList()
        {
            var list = new List<Member>();
            if (_target == null)
                return list;
            //var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            List<string> nameList = new List<string>();
            var type = _target.GetType();
            while (type != (typeof(Object)))
            {
                var fields = _settings.ContentProvider.GetFields(type);
                foreach (var field in fields)
                {
                    var atts = System.Attribute.GetCustomAttributes(field, typeof(TransientAttribute));
                    if (atts != null && atts.Count() > 0)
                        continue;

                    if (!nameList.Contains(field.Name))
                    {
                        nameList.Add(field.Name);
                        if (field.Name.StartsWith("<") && field.Name.EndsWith(">k__BackingField"))
                            continue;

                        var value = field.GetValue(_target);
                        list.Add(new Member(field.Name, value, field.FieldType, true, field));
                    }
                }

                var properties = _settings.ContentProvider.GetProperties(type);
                foreach (var prop in properties)
                {
                    var atts = System.Attribute.GetCustomAttributes(prop, typeof(TransientAttribute));
                    if (atts != null && atts.Count() > 0)
                        continue;

                    if (!nameList.Contains(prop.Name))
                    {
                        nameList.Add(prop.Name);
                        object value = null;
                        try
                        {
                            value = prop.GetValue(_target, null);
                        }
                        catch (Exception ex)
                        {
                            value = ex;
                        }
                        list.Add(new Member(prop.Name, value, prop.PropertyType, false, prop));
                    }
                }
                type = type.BaseType;
            }
            return list;
        }
    }
}
