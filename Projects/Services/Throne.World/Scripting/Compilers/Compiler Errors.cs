using System;
using System.Collections.Generic;

namespace Throne.World.Scripting.Compiler
{
    public class CompilerErrorsException : Exception
    {
        public CompilerErrorsException()
        {
            Errors = new List<CompilerError>();
        }

        public List<CompilerError> Errors { get; protected set; }
    }

    public class CompilerError : Exception
    {
        public CompilerError(string file, int line, int column, string message, bool isWarning)
            : base(message)
        {
            File = file;
            Line = line;
            Column = column;
            IsWarning = isWarning;
        }

        public string File { get; protected set; }
        public int Line { get; protected set; }
        public int Column { get; protected set; }
        public bool IsWarning { get; protected set; }
    }
}