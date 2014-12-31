using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using Throne.Framework;
using Throne.Framework.Commands;
using Throne.Framework.Threading;
using Throne.Framework.Threading.Actors;
using Throne.World.Network.Messages;
using Throne.World.Properties.Settings;

namespace Throne.World
{
    public sealed class ChatManager : SingletonActor<ChatManager>
    {
        private static readonly char CommandPrefix = SystemSettings.Default.CommandPrefix;

        private Queue<ChatMessage> _broadcastQueue;
        private ActorTimer _broadcastTicker;

        private ChatManager()
        {
            _broadcastTicker = new ActorTimer(this, CycleBroadcast, TimeSpan.Zero, new TimeSpan(0, 0, 0, 15));
        }

        public void CycleBroadcast()
        {
        }

        /// <summary>
        /// Handle chat messages.
        /// TODO: Sanitize messages before echoing to other clients
        /// </summary>
        /// <param name="msg"></param>
        public void ProcessChatMessage(ChatMessage msg)
        {
            if (msg.Message.StartsWith(CommandPrefix.ToString(CultureInfo.InvariantCulture)))
            {
                var arguments = new CommandArguments(msg.Message.ParseCommand(),
                    msg.Message.Contains(new String(CommandPrefix, 2)));
                CommandManager.Instance.PostAsync(cm => cm.ExecuteCommand(arguments, msg.Client));
            }
<<<<<<< HEAD
            msg.Color = Color.Green;
=======
            msg.Color = System.Drawing.Color.Green;
>>>>>>> origin/master

            switch (msg.Type)
            {
                case MessageChannel.Talk:
                    msg.Client.Character.SendToLocal(msg);
                    break;
                case MessageChannel.BroadcastMessage:
                    _broadcastQueue.Enqueue(msg);
                    break;
                default:
                    msg.Client.Send("This channel isn't supported yet.");
                    break;
            }
        }
    }
}
