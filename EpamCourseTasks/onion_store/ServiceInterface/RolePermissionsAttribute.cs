using System;

namespace ServiceInterface
{
    /// <summary>
    /// The attribute used to specify role permissions for a method.
    /// </summary>
    public class RolePermissionsAttribute : Attribute
    {
        /// <summary>
        /// Permitted roles array.
        /// </summary>
        public Role[] Roles { get; }

        /// <summary>
        /// Attribute constructor.
        /// </summary>
        /// <param name="values">Permitted roles array.</param>
        public RolePermissionsAttribute(params Role[] values)
        {
            Roles = values;
        }
    }
}