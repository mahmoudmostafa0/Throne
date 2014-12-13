using System.IO;
using System.Reflection;
using Throne.Shared.Logging;

namespace Throne.World.Scripting.Compiler
{
    public abstract class Compiler
    {
        protected readonly LogProxy Log = new LogProxy("ScriptCompiler");

        /// <summary>
        /// Compiles script or fetches it from cache.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="outPath"></param>
        /// <returns></returns>
        public abstract Assembly Compile(string path, string outPath);

        /// <summary>
        /// Returns true if the out file exists and is newer than the script.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="outPath"></param>
        /// <returns></returns>
        protected bool ExistsAndUpToDate(string path, string outPath)
        {
            // Check existence of compiled assembly
            if (!File.Exists(outPath))
                return false;

            // Check if changes were made to script
            return File.GetLastWriteTime(path) <= File.GetLastWriteTime(outPath);
        }

        /// <summary>
        /// Saves assembly to outPath, overwrites existing file.
        /// </summary>
        /// <param name="asm"></param>
        /// <param name="outPath"></param>
        protected void SaveAssembly(Assembly asm, string outPath)
        {
            var outRoot = Path.GetDirectoryName(outPath);

            if (File.Exists(outPath))
                File.Delete(outPath);
            else if (!Directory.Exists(outRoot))
                Directory.CreateDirectory(outRoot);

            File.Copy(asm.Location, outPath);
        }
    }
}
