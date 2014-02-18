
using System;
using System.Collections.Generic;
using System.Text;

namespace DragonScale.Portable.Formatters.Json
{
    /// <summary>
    /// JsonArrangement class.
    /// </summary>
    public sealed class JsonArrangement
    {
        #region Fields
        private const string Space = " ";
        private const int DefaultIndent = 0;
        private const string Indent = "\t";
        private static readonly string NewLine = Environment.NewLine;

        private bool inDoubleString;
        private bool inSingleString;
        private bool inVariableAssignment;
        private char prevChar = '\0';
        private Stack<JsonContextType> context = new Stack<JsonContextType>();
        #endregion

        #region JsonContextType
        [Flags]
        private enum JsonContextType : byte { Object, Array }
        #endregion

        #region	Methods
        private static void BuildIndents(int indents, StringBuilder output)
        {
            indents += DefaultIndent;
            for (; indents > 0; indents--)
                output.Append(Indent);
        }

        private void reset()
        {
            inDoubleString = false;
            inSingleString = false;
            inVariableAssignment = false;
            prevChar = '\0';
            context.Clear();
        }

        private bool InString()
        {
            return inDoubleString || inSingleString;
        }

        /// <summary>
        /// Beautifies the input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public string Beautify(string input)
        {
            reset();
            var output = new StringBuilder(input.Length * 2);
            char c;

            for (int i = 0; i < input.Length; i++)
            {
                c = input[i];
                switch (c)
                {
                    case '{':
                        if (!InString())
                        {
                            if (inVariableAssignment || (context.Count > 0 &&
                                context.Peek() != JsonContextType.Array))
                            {
                                output.Append(NewLine);
                                BuildIndents(context.Count, output);
                            }
                            output.Append(c);
                            context.Push(JsonContextType.Object);
                            output.Append(NewLine);
                            BuildIndents(context.Count, output);
                        }
                        else
                            output.Append(c);
                        break;
                    case '}':
                        if (!InString())
                        {
                            output.Append(NewLine);
                            context.Pop();
                            BuildIndents(context.Count, output);
                            output.Append(c);
                        }
                        else
                            output.Append(c);
                        break;
                    case '[':
                        output.Append(c);
                        if (!InString())
                            context.Push(JsonContextType.Array);
                        break;
                    case ']':
                        if (!InString())
                        {
                            output.Append(c);
                            context.Pop();
                        }
                        else
                            output.Append(c);
                        break;
                    case '=':
                        output.Append(c);
                        break;
                    case ',':
                        output.Append(c);

                        if (!InString() && context.Peek() != JsonContextType.Array)
                        {
                            BuildIndents(context.Count, output);
                            output.Append(NewLine);
                            BuildIndents(context.Count, output);
                            inVariableAssignment = false;
                        }
                        break;
                    case '\'':
                        if (!inDoubleString && prevChar != '\\')
                            inSingleString = !inSingleString;
                        output.Append(c);
                        break;
                    case ':':
                        if (!InString())
                        {
                            inVariableAssignment = true;
                            output.Append(Space);
                            output.Append(c);
                            output.Append(Space);
                        }
                        else
                            output.Append(c);
                        break;
                    case '"':
                        if (!inSingleString && prevChar != '\\')
                            inDoubleString = !inDoubleString;
                        output.Append(c);
                        break;
                    case ' ':
                        if (InString())
                            output.Append(c);
                        break;
                    default:
                        output.Append(c);
                        break;
                }
                prevChar = c;
            }

            return output.ToString();
        }
        #endregion
    }
}
