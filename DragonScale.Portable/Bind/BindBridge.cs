/***
 * Author: Aliqi
 * E-mail: aliqi@hotmail.com
 * Created Time: 2013-06-14
 * Copyright: If you want to use this module, please retain this comment.
 * You can change any code of this file and add your name to the developers list,
 * but you cannot delete this comment or modify all content above.
 * Here's the Developers List:
 */

using System.ComponentModel;
using System.Reflection;

namespace DragonScale.Portable
{
    /// <summary>
    /// BindBridge class.
    /// </summary>
    public sealed class BindBridge
    {
        internal readonly PropertyInfo SrcProperty;

        /// <summary>
        /// Gets the source property.
        /// </summary>
        /// <value>
        /// The source property.
        /// </value>
        public string SourceProperty { get; private set; }

        /// <summary>
        /// Gets the target property.
        /// </summary>
        /// <value>
        /// The target property.
        /// </value>
        public BasicProperty TargetProperty { get; private set; }

        /// <summary>
        /// Gets the source.
        /// </summary>
        /// <value>
        /// The source.
        /// </value>
        public IPropertyChanged Source { get; private set; }

        /// <summary>
        /// Gets the target.
        /// </summary>
        /// <value>
        /// The target.
        /// </value>
        public BasicObject Target { get; private set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal BindBridge(string sourceProperty, BasicProperty targetProperty,
            IPropertyChanged source, BasicObject target)
        {
            SrcProperty = source.GetType().GetProperty(sourceProperty);
            if (SrcProperty == null)
                throw new System.InvalidOperationException(
                    "There is no such property in the source. [" + sourceProperty + "]");
            SourceProperty = sourceProperty;
            TargetProperty = targetProperty;
            Source = source;
            Target = target;
            if (!targetProperty.PropertyType.IsAssignableFrom(SrcProperty.PropertyType))
                throw new System.InvalidOperationException(
                    "Property type not match the type of basic property.");
        }

        internal void SetBindValue()
        {
            Target.SetValue(TargetProperty, SrcProperty.GetValue(Source, null));
        }

        /// <summary>
        /// Breaks the bind connection.
        /// </summary>
        public void Break()
        {
            Target.Unbind(this);
        }
    }
}
