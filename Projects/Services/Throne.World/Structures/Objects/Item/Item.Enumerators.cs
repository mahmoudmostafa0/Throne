namespace Throne.World.Structures.Objects
{
    partial class Item
    {
        public enum Mode : byte
        {
            None = 0,
            AddOrMove = 1,
            Trade = 2,
            Update = 3,
            View = 4,
            Confiscated = 8,
            Link = 9,
            Mail = 11,
            Auction = 12
        }

        public enum Color : byte
        {
            Black = 2,
            Orange = 3,
            LightBlue = 4,
            Red = 5,
            Blue = 6,
            Yellow = 7,
            Purple = 8,
            White = 9
        }

        public enum ActiveEffect
        {
            None = 0,
            Steed = 100,
            Poison = 200,
            HP = 201,
            MP = 202,
            Shield = 203
        }
    }
}