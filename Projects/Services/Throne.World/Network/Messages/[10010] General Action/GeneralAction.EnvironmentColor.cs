using System;

namespace Throne.World.Network.Messages
{
    public partial class GeneralAction
    {
        public GeneralAction UpdateEnvironmentColor()
        {

            ProcessTimestamp = Environment.TickCount;
            ObjectId = Character.ID;
            Argument = Character.Location.Map.EnvironmentColor.ToArgb();
            return this;
        }
    }
}