using System;
using Throne.World.Network;
using Throne.World.Network.Messages;

namespace Throne.World.Structures.Task_Dialog
{
    public sealed class DialogPicture : Dialog
    {
        public DialogPicture(UInt16 pictureId)
        {
            PictureId = pictureId;
        }

        public UInt16 PictureId { get; set; }

        protected override WorldPacket MakePacket()
        {
            return new TaskDialog(0).Picture(PictureId);
        }
    }
}
