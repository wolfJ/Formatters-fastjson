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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace DragonScale.Portable
{
    /// <summary>
    /// Basic object for this framework.
    /// </summary>
    public class BasicObject : IPropertyChanged
    {
        #region Fields
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Dictionary<int, object> pairs;

        [EditorBrowsable(EditorBrowsableState.Never)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Dictionary<int, PropertyChangedCallback> callbacks;

        [EditorBrowsable(EditorBrowsableState.Never)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Dictionary<int, BindBridge> bridges;

        [EditorBrowsable(EditorBrowsableState.Never)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Dictionary<int, InternalBindCollection> binds;

        /// <summary>
        /// Occurs when [property changed].
        /// </summary>
        public event EventHandler<PropertyChangedEventArgs> PropertyChanged;
        #endregion

        #region Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="BasicObject"/> class.
        /// </summary>
        public BasicObject() { }
        #endregion

        #region Get && Set
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>null if there is no such key. Otherwise value.</returns>
        public object GetValue(BasicProperty key)
        {
            if (key == null) throw new ArgumentNullException("key");
            if (pairs == null) pairs = new Dictionary<int, object>();
            object obj = null;
            pairs.TryGetValue(key.Hash, out obj);
            if (obj == null)
                obj = key.DefaultValue;
            return obj;
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void SetValue(BasicProperty key, object value)
        {
            Guard.ArgumentNotNull(key, "key");
            if (value == null && key.IsPropertyValueType)
                throw new InvalidOperationException("Value type can not be null.");
            if (value != null && !key.PropertyType.IsAssignableFrom(value.GetType()))
                throw new InvalidOperationException("Property type not match the type of value.");
            if (pairs == null) pairs = new Dictionary<int, object>();
            object obj = null;
            pairs.TryGetValue(key.Hash, out obj);
            if (obj == value) return;
            if ((obj != null && !obj.Equals(value)) || (obj == null && value != null))
            {
                pairs[key.Hash] = value;
                var e = new PropertyChangedEventArgs(key.Name, value);
                if (callbacks != null)
                {
                    PropertyChangedCallback callback = null;
                    if (callbacks.TryGetValue(key.Hash, out callback) && callback != null)
                        callback(this, e);
                }
                notify(e);
            }
        }

        /// <summary>
        /// Sets the property changed callback.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="callback">The callback.</param>
        public void SetPropertyChangedCallback(BasicProperty key, PropertyChangedCallback callback)
        {
            Guard.ArgumentNotNull(key, "key");
            if (callbacks == null) callbacks = new Dictionary<int, PropertyChangedCallback>();
            callbacks[key.Hash] = callback;
        }
        #endregion

        private void notify(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }

        /// <summary>
        /// Notifies the basic property of target using the specified property name and property value.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="propertyValue">Value of the property.</param>
        protected void Notify(string propertyName, object propertyValue)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName, propertyValue));
        }

        #region Get hash
        [EditorBrowsable(EditorBrowsableState.Never)]
        private static int getHash(object o0, object o1)
        {
            return o0.GetHashCode() ^ o1.GetHashCode();
        }
        #endregion

        /// <summary>
        /// Unbinds the specified bridge.
        /// </summary>
        /// <param name="bridge">The bridge.</param>
        public void Unbind(BindBridge bridge)
        {
            Guard.ArgumentNotNull(bridge, "bridge");
            if (bridge.Target != this)
                throw new InvalidOperationException("This is not the right bridge with current Target.");
            if (bridges != null)
            {
                var hashBind = getHash(bridge.Source, bridge.SourceProperty);
                var hash = getHash(hashBind, bridge.TargetProperty);
                InternalBindCollection bc = null;
                if (binds != null && binds.TryGetValue(hashBind, out bc))
                    bc.Remove(bridge.TargetProperty.Hash);
                bridges.Remove(hash);
            }
        }

        /// <summary>
        /// Binds to source.
        /// The specified basic property value changed when the property occured from source.
        /// </summary>
        /// <param name="targetBasicProp">The basic property in binding target.</param>
        /// <param name="source">The source.</param>
        /// <param name="srcPropName">Name of the property in source.</param>
        /// <returns></returns>
        public BindBridge Bind(BasicProperty targetBasicProp, IPropertyChanged source, string srcPropName)
        {
            return Bind(targetBasicProp, source, srcPropName, null);
        }

        /// <summary>
        /// Binds the specified target basic prop.
        /// </summary>
        /// <param name="targetBasicProp">The target basic prop.</param>
        /// <param name="source">The source.</param>
        /// <param name="srcPropName">Name of the SRC prop.</param>
        /// <param name="targetPropChangedCallback">The target prop changed callback.</param>
        /// <returns></returns>
        public BindBridge Bind(BasicProperty targetBasicProp, IPropertyChanged source, string srcPropName,
            PropertyChangedCallback targetPropChangedCallback)
        {
            Guard.ArgumentNotNull(targetBasicProp, "targetBasicProp");
            Guard.ArgumentNotNull(source, "source");
            Guard.ArgumentNotNullOrEmptyString(srcPropName, "srcPropName");

            var hashBind = getHash(source, srcPropName);
            var hash = getHash(source, targetBasicProp);
            if (bridges == null) bridges = new Dictionary<int, BindBridge>();
            BindBridge bridge = null;
            if (!bridges.TryGetValue(hash, out bridge))
            {
                bridges[hash] = bridge = new BindBridge(srcPropName, targetBasicProp, source, this);
                if (binds == null) binds = new Dictionary<int, InternalBindCollection>();
                InternalBindCollection bc = null;
                if (!binds.TryGetValue(hashBind, out bc))
                    binds[hashBind] = bc = new InternalBindCollection();
                bc[targetBasicProp.Hash] = targetBasicProp;
                if (callbacks == null) callbacks = new Dictionary<int, PropertyChangedCallback>();
                callbacks[targetBasicProp.Hash] = targetPropChangedCallback;
                source.PropertyChanged -= source_SignalEmitted;
                source.PropertyChanged += source_SignalEmitted;
                bridge.SetBindValue();
            }
            return bridge;
        }

        /// <summary>
        /// Gets the bridge.
        /// </summary>
        /// <param name="targetBasicProp">The basic property in binding target.</param>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public BindBridge GetBridge(BasicProperty targetBasicProp, IPropertyChanged source)
        {
            if (bridges == null) return null;
            Guard.ArgumentNotNull(targetBasicProp, "targetBasicProp");
            Guard.ArgumentNotNull(source, "source");
            var hash = getHash(source, targetBasicProp);
            BindBridge bridge = null;
            bridges.TryGetValue(hash, out bridge);
            return bridge;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        private void source_SignalEmitted(object sender, PropertyChangedEventArgs e)
        {
            var hashBind = getHash(sender, e.Name);
            InternalBindCollection bc = null;
            if (binds.TryGetValue(hashBind, out bc))
            {
                foreach (var prop in bc.Values)
                {
                    var hash = getHash(sender, prop);
                    BindBridge bridge = null;
                    if (bridges.TryGetValue(hash, out bridge))
                        SetValue(bridge.TargetProperty, e.Value);
                }
            }
        }
    }
}
