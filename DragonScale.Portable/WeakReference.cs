/***
 * Author: Aliqi
 * E-mail: aliqi@hotmail.com
 * Created Time: 2010-3-21
 * Copyright: If you want to use this module, please retain this comment.
 * You can change any code of this file and add your name to the developers list,
 * but you cannot delete this comment or modify all content above.
 * Here's the Developers List:
 */

using System;

namespace DragonScale.Portable
{
    /// <summary>
    /// WeakReference class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class WeakReference<T> : WeakReference
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WeakReference&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="target">The target.</param>
        public WeakReference(object target) : base(target) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakReference&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="target">An object to track.</param>
        /// <param name="trackResurrection">Indicates when to stop tracking the object. If true, the object is tracked after finalization; if false, the object is only tracked until finalization.</param>
        public WeakReference(object target, bool trackResurrection) : base(target, trackResurrection) { }

        /// <summary>
        /// Gets or sets the object (the target) referenced by the current <see cref="T:System.WeakReference"/> object.
        /// </summary>
        /// <returns>null if the object referenced by the current <see cref="T:System.WeakReference"/> object has been garbage collected; otherwise, a reference to the object referenced by the current <see cref="T:System.WeakReference"/> object.</returns>
        ///   
        /// <exception cref="T:System.InvalidOperationException">The reference to the target object is invalid. This exception can be thrown while setting this property if the value is a null reference or if the object has been finalized during the set operation.</exception>
        public new T Target
        {
            get { return (T)base.Target; }
            set { base.Target = value; }
        }
    }
}
