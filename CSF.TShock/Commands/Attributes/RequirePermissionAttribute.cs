using System;
using System.Collections.Generic;
using System.Text;

namespace CSF.TShock
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class RequirePermissionAttribute : Attribute
    {
        /// <summary>
        ///     Defines a permission node for this command. Permission nodes will be joined by '.' if they're specified at nested level.
        /// </summary>
        public string PermissionNode { get; }

        /// <summary>
        ///     Defines a new <see cref="RequirePermissionAttribute"/> from provided permission node.
        /// </summary>
        /// <param name="permissionNode">The permission node for this command.</param>
        public RequirePermissionAttribute(string permissionNode)
        {
            PermissionNode = permissionNode;
        }
    }
}
