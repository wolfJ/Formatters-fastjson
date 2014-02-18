/***
 * Author: Aliqi
 * E-mail: aliqi@hotmail.com
 * Created Time: 2012-9-26
 * Copyright: If you want to use this module, please retain this comment.
 * You can change any code of this file and add your name to the developers list,
 * but you cannot delete this comment or modify all content above.
 * Here's the Developers List:
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace DragonScale.Portable
{
    /// <summary>
    /// Role abstract class.
    /// </summary>
    public abstract class Role : BasicObject, IEquatable<Role>
    {
        #region Fields
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private List<Role> _roles = new List<Role>();
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; protected set; }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public Type Type { get; protected set; }
        #endregion

        #region Add
        /// <summary>
        /// Adds the specified role.
        /// </summary>
        /// <param name="role">The role.</param>
        public void Add(Role role)
        {
            Guard.ArgumentNotNull(role, "role");
            if (role.Name == null)
                throw new InvalidOperationException("Name can not be null.");
            var lockable = (_roles as ICollection).SyncRoot;
            lock (lockable)
                _roles.Add(role);
        }

        /// <summary>
        /// Adds the specified roles.
        /// </summary>
        /// <param name="roles">The roles.</param>
        public void Add(IEnumerable<Role> roles)
        {
            Guard.ArgumentNotNull(roles, "roles");
            var lockable = (_roles as ICollection).SyncRoot;
            lock (lockable)
            {
                foreach (var role in roles)
                {
                    if (role.Name == null)
                        throw new InvalidOperationException("Name can not be null.");
                    _roles.Add(role);
                }
            }
        }
        #endregion

        #region Contains
        /// <summary>
        /// Determines whether [contains] [the specified role].
        /// </summary>
        /// <param name="role">The role.</param>
        /// <returns>
        ///   <c>true</c> if [contains] [the specified role]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(Role role)
        {
            if (this.Equals(role))
                return true;
            var _buildRoles = new Stack<Role>(32);
            _buildRoles.Clear();
            var current = this;
            _buildRoles.Push(this);
            while (_buildRoles.Count > 0)
            {
                current = _buildRoles.Pop();
                var lockable = (current._roles as ICollection).SyncRoot;
                lock (lockable)
                {
                    foreach (var item in current._roles)
                    {
                        if (!_buildRoles.Contains(item))
                        {
                            if (item.Equals(role))
                                return true;
                            _buildRoles.Push(item);
                        }
                    }
                }
            }
            return false;
        }
        #endregion

        #region Override
        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var fullName = Type == null ? string.Empty : Type.FullName;
            return new StringBuilder(fullName).Append(".").Append(Name).ToString();
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is Role)
                return Equals(obj as Role);
            return false;
        }

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        public bool Equals(Role other)
        {
            if (Name == other.Name && Type == other.Type)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            var roleHashCode = Name == null ? string.Empty.GetHashCode() : Name.GetHashCode();
            var typeHashCode = Type == null ? 0 : Type.GetHashCode();
            return base.GetHashCode() ^ roleHashCode ^ typeHashCode;
        }
        #endregion
    }

    /// <summary>
    /// Privilege abstract class.
    /// </summary>
    public class Privilege : Role
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Privilege"/> class.
        /// </summary>
        public Privilege() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Privilege"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        public Privilege(string name, Type type)
        {
            Name = name;
            Type = type;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Privilege"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <param name="privileges">The privileges.</param>
        public Privilege(string name, Type type, params Privilege[] privileges)
            : this(name, type)
        {
            Add(privileges);
        }
    }

    /// <summary>
    /// Privilege class.
    /// </summary>
    /// <typeparam name="T">The type of privilege.</typeparam>
    public sealed class Privilege<T> : Privilege
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Privilege&lt;T&gt;"/> class.
        /// </summary>
        public Privilege() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Privilege&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="privileges">The privileges.</param>
        public Privilege(string name, params Privilege[] privileges) : base(name, typeof(T), privileges) { }
    }
}
