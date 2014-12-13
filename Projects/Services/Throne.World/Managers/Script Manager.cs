using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using Throne.Shared.Exceptions;
using Throne.Shared.Logging;
using Throne.Shared.Runtime;
using Throne.Shared.Threading;
using Throne.Shared.Utilities;
using Throne.World.Properties.Settings;
using Throne.World.Scripting;
using Throne.World.Scripting.Compiler;
using Throne.World.Scripting.Scripts;
using Throne.World.Structures.World;

namespace Throne.World
{
    public sealed class ScriptManager : SingletonActor<ScriptManager>
    {
        private const string SystemIndexRoot = "system/scripts/";
        private const string IndexPath = SystemIndexRoot + "scripts.txt";

        private readonly LogProxy Log = new LogProxy("ScriptManager");

        private readonly CSharpCompiler _compiler;

        private readonly Dictionary<UInt32, MapScript> _mapScripts;
        private readonly Dictionary<string, Type> _scripts;

        private readonly List<IDisposableResource> _scriptsToDispose;

        private ScriptManager()
        {
            _compiler = new CSharpCompiler();

            _scripts = new Dictionary<string, Type>();
            _mapScripts = new Dictionary<UInt32, MapScript>();

            _scriptsToDispose = new List<IDisposableResource>();
        }

        /// <summary>
        ///     Loads all scripts.
        ///     First method called.
        /// </summary>
        public void Load()
        {
            LoadScripts();
        }

        private void ClearScriptContainers()
        {
            _mapScripts.Clear();
            _scripts.Clear();
            _scriptsToDispose.Clear();
        }

        /// <summary>
        ///     Loads scripts from list.
        /// </summary>
        private void LoadScripts()
        {
            ClearScriptContainers();

            Log.Info(StrRes.SMSG_ScriptLoad);

            if (!File.Exists(IndexPath))
            {
                Log.Error(StrRes.SMSG_NoScriptList, IndexPath);
                return;
            }

            var toLoad = new OrderedDictionary();

            using (var fr = new FileReader(IndexPath))
                foreach (string line in fr)
                {
                    string scriptPath = Path.Combine(SystemIndexRoot, line);
                    if (!File.Exists(scriptPath))
                    {
                        Log.Warn(StrRes.SMSG_NoScript, line);
                        continue;
                    }
                    toLoad[line] = scriptPath;
                }

            // Load scripts
            int done = 0, loaded = 0;
            foreach (string filePath in toLoad.Values)
            {
                Assembly asm = Compile(filePath);
                if (asm != null)
                {
                    LoadScriptAssembly(asm, filePath);
                    loaded++;
                }

                if (done%5 == 0)
                    Log.Progress(done + 1, toLoad.Count);

                done++;
            }

            // Init scripts
            InitializeScripts();

            Log.Info(StrRes.SMSG_ScriptListLoaded, loaded, toLoad.Count);
        }


        /// <summary>
        ///     Compiles script and returns the resulting assembly, or null.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private Assembly Compile(string path)
        {
            if (!File.Exists(path))
            {
                Log.Error("Script '{0}' not found.", path);
                return null;
            }

            string outPath = GetCachePath(path);

            try
            {
                return _compiler.Compile(path, outPath);
            }
            catch (CompilerErrorsException ex)
            {
                try
                {
                    File.Delete(outPath);
                }
                catch (UnauthorizedAccessException)
                {
                    Log.Warn("Unable to delete '{0}'", outPath);
                }

                string[] lines = File.ReadAllLines(path);

                foreach (CompilerError err in ex.Errors)
                {
                    // Error msg
                    Log.Error("In {0} on line {1}, column {2}", err.File, err.Line, err.Column);
                    Log.Error("          {0}", err.Message);

                    // Display lines around the error
                    int startLine = Math.Max(1, err.Line - 1);
                    int endLine = Math.Min(lines.Length, startLine + 2);
                    for (int i = startLine; i <= endLine; ++i)
                    {
                        // Make sure we don't get out of range.
                        // (ReadAllLines "trims" the input)
                        string line = (i <= lines.Length) ? lines[i - 1] : "";

                        Log.Error("  {2} {0:0000}: {1}", i, line, (err.Line == i ? '*' : ' '));
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("LoadScript: Problem while loading script '{0}'", path);
            }

            return null;
        }

        /// <summary>
        ///     Loads script classes inside assembly.
        /// </summary>
        /// <param name="asm"></param>
        /// <param name="filePath">Path of the script, for reference</param>
        /// <returns></returns>
        private void LoadScriptAssembly(Assembly asm, string filePath)
        {
            Type[] types = null;
            try
            {
                types = asm.GetTypes();
            }
            catch (ReflectionTypeLoadException)
            {
                // Happens if classes in source or other scripts change,
                // i.e. a class name changes. Only fixable by 
                // deleting the cache so they can "sync" again.

                Log.Error(StrRes.SMSG_LoadScriptAssemblyFailed, filePath);
                return;
            }

            if (types == null)
                return;

            foreach (
                Type type in
                    types.Where(
                        a => a.GetInterfaces().Contains(typeof (IScript)) && !a.IsAbstract && !a.Name.StartsWith("_")))
            {
                try
                {
                    // Make sure there's only one copy of each script.
                    if (_scripts.ContainsKey(type.Name))
                    {
                        Log.Error(StrRes.SMSG_DuplicateScript, type.Name, Path.GetFileName(filePath));
                        continue;
                    }

                    // Check overrides
                    var overide = type.GetCustomAttribute<OverrideAttribute>();
                    if (overide != null)
                    {
                        if (_scripts.ContainsKey(overide.TypeName))
                            _scripts.Remove(overide.TypeName);
                        else
                            Log.Warn(StrRes.SMSG_NonsuchOverride, overide.TypeName, type.Name,
                                Path.GetFileName(filePath));
                    }

                    // Check removes
                    var removes = type.GetCustomAttribute<RemoveAttribute>();
                    if (removes != null)
                    {
                        foreach (string rm in removes.TypeNames)
                        {
                            if (_scripts.ContainsKey(rm))
                                _scripts.Remove(rm);
                            else
                                Log.Warn(StrRes.SMSG_NonsuchRemove, rm, type.Name, Path.GetFileName(filePath));
                        }
                    }

                    // Add class to load list, even if it's a dummy for remove,
                    // we can't be sure it's not supposed to get initialized.
                    _scripts[type.Name] = type;
                }
                catch (Exception ex)
                {
                    ExceptionManager.RegisterException(ex);
                }
            }
        }

        /// <summary>
        ///     Initializes all scripts loaded from assemblies.
        /// </summary>
        private void InitializeScripts()
        {
            foreach (Type type in _scripts.Values)
            {
                try
                {
                    // Initiate script
                    var script = Activator.CreateInstance(type) as IScript;
                    if (!script.Init())
                    {
                        Log.Warn(StrRes.SMSG_ScriptInitiateFailed, type.Name);
                        continue;
                    }

                    if (type.GetInterfaces().Contains(typeof (IDisposableResource)))
                        _scriptsToDispose.Add(script as IDisposableResource);

                    if (type.GetInterfaces().Contains(typeof (IAutoLoader)))
                        (script as IAutoLoader).AutoLoad();
                }
                catch (Exception ex)
                {
                    Log.Warn(StrRes.SMSG_ScriptExcption, type.Name, ex.Message);
                }
            }
        }

        /// <summary>
        ///     Returns path for the compiled version of the script.
        ///     Creates directory structure if it doesn't exist.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private string GetCachePath(String path)
        {
            string result = (!path.StartsWith("cache")
                ? Path.Combine("cache", Path.ChangeExtension(path, ".lib"))
                : Path.ChangeExtension(path, ".lib"));
            string dir = Path.GetDirectoryName(result);

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            return result;
        }


        /// <summary>
        ///     Returns map script or null.
        /// </summary>
        /// <param name="mapId"></param>
        /// <returns></returns>
        public MapScript GetMapScript(Map map)
        {
            MapScript script;
            _mapScripts.TryGetValue(map.Id, out script);
            return script ?? new DummyMapScript(map);
        }

        /// <summary>
        /// Shoves a new map script into the script lineup by the map ID.
        /// </summary>
        /// <param name="scr"></param>
        public void AddMapScript(MapScript scr)
        {
            _mapScripts[scr.MapId] = scr;
        }
    }
}