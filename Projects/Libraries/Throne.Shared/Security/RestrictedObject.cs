using System;
using System.Collections.Concurrent;

namespace Throne.Framework.Security
{
    /// <summary>
    ///     A default implementation of IPermissible.
    /// </summary>
    public abstract class RestrictedObject : IPermissible
    {
        private readonly ConcurrentDictionary<Type, Permission> _permissions =
            new ConcurrentDictionary<Type, Permission>();

        public void AddPermission(Permission perm)
        {
            _permissions.Add(perm.GetType(), perm);
        }

        public void RemovePermission(Type permType)
        {
            _permissions.Remove(permType);
        }

        public bool HasPermission(Type permType)
        {
            return _permissions.TryGet(permType) != null;
        }
    }
}