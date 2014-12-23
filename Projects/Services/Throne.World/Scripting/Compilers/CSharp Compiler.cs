using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using csscript;
using CSScriptLibrary;
using Throne.Framework.Exceptions;

namespace Throne.World.Scripting.Compiler
{
    public class CSharpCompiler : Compiler
    {
        public override Assembly Compile(string path, string outPath)
        {
            Assembly asm = null;
            try
            {
                if (ExistsAndUpToDate(path, outPath))
                    return Assembly.LoadFrom(outPath);

                asm = CSScript.LoadCode(PreCompile(File.ReadAllText(path)));

                SaveAssembly(asm, outPath);
            }
            catch (CompilerException ex)
            {
                var errors = ex.Data["Errors"] as CompilerErrorCollection;
                var newExs = new CompilerErrorsException();

                foreach (System.CodeDom.Compiler.CompilerError err in errors)
                    // Line -1 to compensate lines added by the pre-compiler.
                    newExs.Errors.Add(new CompilerError(path, err.Line - 1, err.Column, err.ErrorText, err.IsWarning));

                throw newExs;
            }
            catch (UnauthorizedAccessException)
            {
                // Thrown if file can't be copied. Happens if script was
                // initially loaded from cache.
                // TODO: Also thrown if CS-Script can't create the file.
            }
            catch (Exception ex)
            {
                ExceptionManager.RegisterException(ex);
            }

            return asm;
        }

        public string PreCompile(string script)
        {
            // Default usings and compiler options
            var add = new StringBuilder();

            // Mono needs this to not treat harmless warnings as errors
            // (like a missing await in an async Task) and to not spam
            // us with warnings.
            add.AppendLine("//css_co /warnaserror- /warn:0;");

            add.Append("using System;");
            add.Append("using System.Collections.Generic;");
            add.Append("using System.Collections;");
            add.Append("using System.Linq;");
            add.Append("using System.Text;");
            add.Append("using System.Threading.Tasks;");
            add.Append("using System.Timers;");
            add.Append("using Microsoft.CSharp;");
            script = add + script;

            // Return();
            // --> yield break;
            // Stops Enumerator and the conversation.
            script = Regex.Replace(script,
                @"([\{\}:;\t ])?Return\s*\(\s*\)\s*;",
                "$1yield break;",
                RegexOptions.Compiled);

            // Do|Call(<method_call>);
            // --> foreach(var __callResult in <method_call>) yield return __callResult;
            // Loops through Enumerator returned by the method called and passes
            // the results to the main Enumerator.
            script = Regex.Replace(script,
                @"([\{\}:;\t ])?(Call|Do)\s*\(([^;]*)\)\s*;",
                "$1foreach(var __callResult in $3) yield return __callResult;",
                RegexOptions.Compiled);

            // duplicate <new_class> : <old_class> { <content_of_load> }
            // --> public class <new_class> : <old_class> { public override void OnLoad() { base.OnLoad(); <content_of_load> } }
            // Makes a new class, based on another one, calls the inherited
            // load first, and the new load afterwards.
            script = Regex.Replace(script,
                @"duplicate +([^\s:]+) *: *([^\s{]+) *{ *([^}]+) *}",
                "public class $1 : $2 { public override void Load() { base.Load(); $3 } }",
                RegexOptions.Compiled);

            return script;
        }
    }
}