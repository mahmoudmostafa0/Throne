using System;
using System.Collections;

namespace Throne.World.Scripting.Scripts
{
    /// <summary>
    /// TODO: replace ExecutionResult with exceptions.
    /// </summary>
    public abstract class DynamicScript : ScriptBase
    {
        public enum ExecutionResult
        {
            Success,
            NonSuchScript,
            Failed
        }


        public String Name;

        public override bool Init()
        {
            Load();
            ScriptManager.Instance.AddDynamicScript(this);

            return true;
        }

        public void SetName(String name)
        {
            Name = name;
        }

        /// <summary>
        /// Should not be called directly.
        /// Call TryExecution for null-ref safety.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual ExecutionResult Execute(IEnumerator param)
        {
            return ExecutionResult.Success;
        }
    }

    public static class DynamicScriptExtension
    {
        /// <summary>
        /// Null-ref safe execution for dynamic parameter scripting.
        /// </summary>
        /// <param name="script"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static DynamicScript.ExecutionResult TryExecution(this DynamicScript script, params dynamic[] parameters)
        {
            if (!script)
                return DynamicScript.ExecutionResult.NonSuchScript;

            var enumerator = parameters.GetEnumerator();
            return script.Execute(enumerator);
        }
    }
}