//Thanks to Spirited Fang for documenting this class.

using System.Runtime.InteropServices;

namespace Throne.Shared.Native
{
    /// <summary>
    ///     This class encapsulates the console control handler. When the application is terminated in some way or another,
    ///     this class handles the callback function for that action. It can be used to execute last second instructions
    ///     as the process is terminated.
    /// </summary>
    public static partial class NativeMethods
    {
        /// <summary> This delegate defines the parameters for an event handler for the console handler function. </summary>
        /// <param name="dwCtrlType">The type of control signal received by the handler.</param>
        public delegate int EventHandler(CtrlType dwCtrlType);

        /// <summary> This enumeration type defines the types of control signals received by the handler. </summary>
        public enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }

        /// <summary>
        ///     This function adds or removes an application-defined HandlerRoutine function from the list of handler
        ///     functions for the calling process. It is used by the server to save character data before terminating
        ///     the process. It returns zero if the function completed successfully.
        /// </summary>
        /// <param name="handler">The function handling the event.</param>
        /// <param name="add">If true, adds the handler. Else, handler is removed.</param>
        [DllImport("Kernel32")]
        public static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);
    }
}