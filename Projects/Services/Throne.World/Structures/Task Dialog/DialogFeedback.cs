using System;

namespace Throne.World.Structures.Task_Dialog
{
    public class DialogFeedback
    {
        public String Input;
        public Byte Option;

        public DialogFeedback(Byte op)
        {
            Option = op;
            Input = String.Empty;
        }

        public DialogFeedback(Byte op, String input)
        {
            Option = op;
            Input = input;
        }

        public static implicit operator Byte(DialogFeedback dlgFeedback)
        {
            return dlgFeedback.Option;
        }

        public static implicit operator String(DialogFeedback dlgFeedback)
        {
            return dlgFeedback.Input;
        }
    }
}