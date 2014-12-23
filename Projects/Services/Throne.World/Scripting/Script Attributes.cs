using System;

namespace Throne.World.Scripting
{
    /// <summary>
    ///     Defines a type that's not to load.
    /// </summary>
    /// <remarks>
    ///     Use when overriding an existing NPC, to only load one version of it.
    /// </remarks>
    public class OverrideAttribute : Attribute
    {
        public OverrideAttribute(string typeName)
        {
            TypeName = typeName;
        }

        public string TypeName { get; private set; }
    }

    /// <summary>
    ///     Defines types to remove from loading list.
    /// </summary>
    /// <remarks>
    ///     List types that are to be removed from the loading list.
    ///     Similar to Override in functionality.
    /// </remarks>
    public class RemoveAttribute : Attribute
    {
        public RemoveAttribute(params string[] typeNames)
        {
            TypeNames = typeNames;
        }

        public string[] TypeNames { get; private set; }
    }

    /// <summary>
    ///     Auto load script attribute, defines method to fire on event.
    /// </summary>
    /// <remarks>
    ///     Any methods marked with this attribute in an auto loaded script
    ///     will be stored in the server's event manger for the specified
    ///     exception. When the named exception fires, the method will be executed
    ///     by that exception's event handler.
    /// </remarks>
    public class OnAttribute : Attribute
    {
        public OnAttribute(string evnt)
        {
            Event = evnt;
        }

        public string Event { get; protected set; }
    }
}