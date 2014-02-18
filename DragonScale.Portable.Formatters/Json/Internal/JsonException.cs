/***
 * Author: Wolf
 * E-mail: 
 * Created Time: 2012-10-01
 * Copyright: If you want to use this module, please retain this comment.
 * You can change any code of this file and add your name to the developers list,
 * but you cannot delete this comment or modify all content above.
 * Here's the Developers List:
 */

using System;

namespace DragonScale.Portable.Formatters.Json
{
    internal class JsonException : Exception//ApplicationException
    {
        public JsonException() : base() { }

        internal JsonException(ParserToken token) :
            base(String.Format(
                   "Invalid token '{0}' in input string", token))
        {
        }

        internal JsonException(ParserToken token,
                                Exception inner_exception) :
            base(String.Format(
                    "Invalid token '{0}' in input string", token),
                inner_exception)
        {
        }

        internal JsonException(int c) :
            base(String.Format(
                   "Invalid character '{0}' in input string", (char)c))
        {
        }

        internal JsonException(int c, Exception inner_exception) :
            base(String.Format(
                   "Invalid character '{0}' in input string", (char)c),
               inner_exception)
        {
        }

        public JsonException(string message)
            : base(message)
        {
        }

        public JsonException(string message, Exception inner_exception) :
            base(message, inner_exception)
        {
        }
    }
}
