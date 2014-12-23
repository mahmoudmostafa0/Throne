using System;

namespace Throne.World.Structures.Task_Dialog
{
    public class DialogFeedback
    {
        public Int32 Time;
        public String Input;
        public Byte Option;

        public DialogFeedback() { }

        public DialogFeedback(Byte op)
        {
            Option = op;
            Input = String.Empty;
        }

        public DialogFeedback(Byte op, String input, Int32 feedbackTime)
        {
            Option = op;
            Input = input;
            Time = feedbackTime;
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