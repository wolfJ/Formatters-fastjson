using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DragonScale.Portable.Formatters.Json
{
    internal class GenericTypeModel
    {
        string TypeQualifiedName { get; set; }

        string TypeName { get; set; }
        string AssName { get; set; }
        string GenericParameter { get; set; }

        GenericTypeModel[] Parameters { get; set; }

        public GenericTypeModel(string typeQualifiedName)
        {
            this.TypeQualifiedName = typeQualifiedName;
            if (typeQualifiedName.Contains("`"))
            {
                TypeName = typeQualifiedName.Substring(0, typeQualifiedName.IndexOf('`') + 2);
                AssName = typeQualifiedName.Substring(typeQualifiedName.LastIndexOf(']') + 2).Split(',')[0];
                GenericParameter = typeQualifiedName.Substring(typeQualifiedName.IndexOf('[') + 1, (typeQualifiedName.LastIndexOf(']') - typeQualifiedName.IndexOf('[') - 1));

                var paraSize = int.Parse(typeQualifiedName.Substring(typeQualifiedName.IndexOf('`') + 1, 1));
                Parameters = new GenericTypeModel[paraSize];
                string[] paraModelName = getParaModelNameString(GenericParameter, paraSize);
                for (int i = 0; i < paraModelName.Length; i++)
                {
                    if (!string.IsNullOrEmpty(paraModelName[i]))
                        Parameters[i] = new GenericTypeModel(paraModelName[i]);
                }
            }
            else
            {
                if (typeQualifiedName.Contains(","))
                {
                    TypeName = typeQualifiedName.Split(',')[0];
                    AssName = typeQualifiedName.Split(',')[1];
                }
                else
                {
                    TypeName = typeQualifiedName;
                }
            }
        }

        private string[] getParaModelNameString(string GenericPara, int paraSize)
        {
            string[] paraModelName = new string[paraSize];
            int startIndex = 0, endIndex = GenericPara.Length - 1, flagCount = 0, sizeCount = 0;
            for (int i = 0; i < GenericPara.Length; i++)
            {
                while (GenericPara[startIndex] != '[')
                {
                    if (GenericPara.Substring(startIndex).Contains(","))
                    {
                        endIndex = startIndex + 1 + GenericPara.Substring(startIndex).IndexOf(',');
                    }
                    paraModelName[sizeCount] = GenericPara.Substring(startIndex, endIndex - startIndex);
                    sizeCount++;
                    startIndex = endIndex + 1;
                    i = startIndex;
                }

                if (GenericPara[i] == '[')
                {
                    flagCount++;
                    if (flagCount == 1)
                    {
                        startIndex = i;
                    }
                }
                else if (GenericPara[i] == ']')
                {
                    flagCount--;
                    if (flagCount == 0)
                    {
                        endIndex = i;
                    }
                    paraModelName[sizeCount] = GenericPara.Substring(startIndex + 1, endIndex - 1 - startIndex);
                    sizeCount++;
                    startIndex = endIndex + 1;
                }
            }
            return paraModelName;
        }

        internal Type GetModelType(List<System.Reflection.Assembly> asses)
        {
            Type type = Type.GetType(TypeName + (AssName == null ? "" : ", " + AssName));
            if (type == null)
            {
                type = findTypeByAsses(asses, TypeName, AssName);
            }
            if (type != null && Parameters != null && Parameters.Length > 0)
            {
                Type[] types = new Type[Parameters.Length];
                for (int i = 0; i < Parameters.Length; i++)
                {
                    types[i] = Parameters[i].GetModelType(asses);
                }
                type = type.MakeGenericType(types);
            }
            return type;
        }

        private Type findTypeByAsses(List<System.Reflection.Assembly> asses, string typeName, string assName)
        {
            Type type = null;
            if (asses != null)
            {
                foreach (var item in asses)
                {
                    if (item.FullName.Split(',')[0].Equals(assName.Trim()))
                    {
                        type = item.GetType(typeName);
                    }
                }
            }
            return type;
        }
    }
}
