using System;
using System.Collections.Generic;

namespace Throne.Framework.Commands
{
    public abstract class Command
    {
        /// <summary>
        ///     The permission required to execute this command.
        ///     If this is null, anyone may call this command. If non-null, only callers with the
        ///     specified permission may call it (i.e. the console, and any entity with the given
        ///     permission).
        /// </summary>
        public virtual IEnumerable<Type> RequestedPermissions
        {
            get { return null; }
        }

        /// <summary>
        ///     Indicates whether or not a sender is required. If true, the command cannot be called
        ///     from the console.
        /// </summary>
        public virtual bool RequiresSender
        {
            get { return false; }
        }

        public virtual string Description
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        ///     Executes the command.
        /// </summary>
        /// <param name="args">The arguments to the command.</param>
        /// <param name="sender">The sender of the command (may be null in the case of the console).</param>
        /// <returns>Whether or not arguments were valid.</returns>
        public abstract void Execute(CommandArguments args, ICommandUser sender);
    }
}