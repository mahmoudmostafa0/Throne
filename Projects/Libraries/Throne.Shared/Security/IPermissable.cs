using System;

namespace Throne.Shared.Security
{
    /// <summary>
    ///     An abstract way to manage permissions on an object.
    /// </summary>
    public interface IPermissible
    {
        void AddPermission(Permission perm);

        void RemovePermission(Type permType);

        bool HasPermission(Type permType);
    }
}