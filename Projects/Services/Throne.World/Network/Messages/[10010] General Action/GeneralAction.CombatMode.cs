namespace Throne.World.Network.Messages
{
    public partial class GeneralAction
    {
        public enum CombatMode : byte
        {
            PK = 0,
            Peace = 1,
            Team = 2,
            Capture = 3,
            Revenge = 4,
            Guild = 6,
            JiangHu = 7
        }

        public void SetCombatMode()
        {
            Character.User.Send(this);
        }
    }
}