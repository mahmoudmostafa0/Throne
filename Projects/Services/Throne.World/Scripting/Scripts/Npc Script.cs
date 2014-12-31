using System;
using System.Threading;
using System.Threading.Tasks;
using Throne.Framework;
using Throne.World.Network.Messages;
using Throne.World.Structures;
using Throne.World.Structures.Objects;
using Throne.World.Structures.Objects.Actors;
using Throne.World.Structures.Task_Dialog;
using Throne.World.Structures.Travel;

namespace Throne.World.Scripting.Scripts
{
    public abstract class NpcScript : ScriptBase
    {
        public enum InteractionStates
        {
            Ongoing,
            Feedback,
            Ended
        }

        private readonly CancellationTokenSource _cancelToken;
        private readonly SemaphoreSlim _resumeSignal;

        private Character _character;
        protected DialogFeedback Feedback { get; private set; }

        protected NpcScript()
        {
            Npc = new Npc();
            Feedback = new DialogFeedback();
            _resumeSignal = new SemaphoreSlim(0, 1);
            _cancelToken = new CancellationTokenSource();
        }

        public Npc Npc { get; set; }
        public InteractionStates InteractionState { get; set; }

        public Character Character
        {
            get
            {
                if (!_character)
                    throw new Exception("{0}: Script must be cloned for use.".Interpolate(GetType().Name));
                return _character;
            }
            set { _character = value; }
        }

        public override bool Init()
        {
            Load();
            Npc.Script = this;

            if (!Npc.Location.Map)
                Log.Error("{0}/{1} could not be spawned.", Npc.LongName, Npc.DisplayName);
            else
                Npc.SpawnAtLocation();

            return true;
        }

        public virtual async void Interact()
        {
            InteractionState = InteractionStates.Ongoing;
            try
            {
                await Talk();
            }
            catch (OperationCanceledException)
            {
                InteractionEnded();
            }
            InteractionState = InteractionStates.Ended;
        }

        /// <summary>
        /// Ends the await for feedback in the scripts.
        /// </summary>
        public virtual void EndInteraction()
        {
            _cancelToken.Cancel();
        }        
        
        /// <summary>
        /// Called only when the player tells the NPC to eff off. (Or the player logged out)
        /// </summary>
        public virtual void InteractionEnded()
        {
            Character.User.Send("You ended the conversation with {0}.".Interpolate(Npc.LongName));
        }

        public NpcScript Clone(Character forChr)
        {
            //TODO: Test performance impact.. MemberwiseClone vs. Activator.CreateInstance
            var scriptClone = Activator.CreateInstance(GetType()) as NpcScript;
            scriptClone.Character = forChr;
            scriptClone.Npc = Npc;
            return scriptClone;
        }

        public Boolean IsValid()
        {
            return InteractionState != InteractionStates.Feedback;
        }

        #region Set attributes

        protected void SetLocation(Location loc)
        {
            Npc.Location = loc;
        }

        protected void SetFacing(Orientation facing)
        {
            Npc.Look.Facing = facing;
        }

        protected void SetMesh(UInt16 mesh)
        {
            Npc.Look.Mesh = mesh;
        }

        protected void SetFace(Int16 face)
        {
            Npc.Look.Face = face;
        }

        protected void SetIdentity(UInt32 id)
        {
            Npc.ID = id;
        }

        protected void SetLongName(String name)
        {
<<<<<<< HEAD
            Npc.LongName = name;
        }

        protected void SetDisplayName(String name)
        {
            Npc.DisplayName = name;
=======
            Npc.SendName = true;
            Npc.Name = name;
>>>>>>> origin/master
        }

        protected void SetType(NpcInformation.Types type)
        {
            Npc.Type = type;
        }

        #endregion

        #region Dialog

        /// <summary>
        /// Default method for talking.
        /// </summary>
        /// <remarks>
        /// The NPC doesn't shut up till it runs out of breath! 
        /// (or until you zap it, with the middle finger of BZZZZT)
        /// </remarks>
        /// <returns></returns>
        protected virtual async Task Talk()
        {
            await Task.Yield();
        }

        protected async Task<DialogFeedback> Response()
        {
            InteractionState = InteractionStates.Feedback;
            await _resumeSignal.WaitAsync(_cancelToken.Token);
            InteractionState = InteractionStates.Ongoing;
            return Feedback;
        }

        public Boolean Resume(DialogFeedback feedback)
        {
            if (feedback.Time == Feedback.Time)
                return true;
            if (feedback.Option == 0)
            {
                InteractionState = InteractionStates.Ended;
                return false;
            }

            Feedback = feedback;
            _resumeSignal.Release();
            return true;
        }

        private void ShowDialog(Boolean hide, Dialog dlg)
        {
            if (dlg.Children.Count > 1)
                dlg.Join(Face((ushort) (Npc.Look.Face == 0 ? 296 : Npc.Look.Face)));

            using (TaskDialog pkt = new TaskDialog(0).ShowDialog())
                Character.User.Send(hide ? dlg.MakeStream() : dlg.MakeStream().Join(pkt));
        }

        protected void Message(String msg, params Dialog[] elements)
        {
            //TODO: split large messages
            ShowDialog(false, new Dialog().Join(new DialogMessage(msg)).Join(elements));
        }

        protected DialogOption Option(String name, Byte option = 0)
        {
            return new DialogOption(name, option);
        }

        protected DialogPicture Face(UInt16 face)
        {
            return new DialogPicture(face);
        }

        protected DialogInput Input(String name, Byte op, UInt16 szInput = 16)
        {
            return new DialogInput(name, op, szInput);
        }

        #endregion
    }
}
