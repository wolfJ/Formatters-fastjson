/*
 * Author: Aliqi
 * E-mail: aliqi@hotmail.com
 * Created Time: 2012-07-22
 * Copyright: If you want to use this module, please retain this comment.
 * You can change any code of this file and add your name to the developers list,
 * but you cannot delete this comment or modify all content above.
 * Here's the Developers List:
 */

using System;

namespace DragonScale.Portable
{
    /// <summary>
    /// Guard static class.
    /// </summary>
    public static class Guard
    {
        /// <summary>
        /// Ensure the argument isn't null.
        /// </summary>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="argumentName">The name of the argument.</param>
        /// <exception cref="ArgumentNullException"><paramref name="argumentValue"/> is a null reference.</exception>
        public static void ArgumentNotNull(object argumentValue, string argumentName)
        {
            if (argumentValue == null)
                throw new ArgumentNullException(argumentName);
        }

        /// <summary>
        /// Ensure a string argument isn't null or empty.
        /// </summary>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="argumentName">The name of the argument.</param>
        /// <exception cref="ArgumentException"><paramref name="argumentValue"/> is a null reference or <see cref="string.Empty"/>.</exception>
        public static void ArgumentNotNullOrEmptyString(string argumentValue, string argumentName)
        {
            if (string.IsNullOrEmpty(argumentValue))
                throw new ArgumentException("String can not be null or empty.", argumentName);
        }

        /// <summary>
        /// Ensure a string argument isn't null or trimed empty.
        /// </summary>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="argumentName">The name of the argument.</param>
        /// <exception cref="ArgumentException"><paramref name="argumentValue"/> is a null reference or trimed <see cref="string.Empty"/>.</exception>
        public static void ArgumentNotNullOrTrimedEmptyString(string argumentValue, string argumentName)
        {
            if (Extensions.IsNullOrTrimedEmpty(argumentValue))
                throw new ArgumentException("String can not be null or trimed empty.", argumentName);
        }
    }
}
