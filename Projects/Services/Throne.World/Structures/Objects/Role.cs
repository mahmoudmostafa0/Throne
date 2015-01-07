using System;
using Throne.Framework;

namespace Throne.World.Structures.Objects
{
    public abstract partial class Role : WorldObject
    {
        protected Model _look;
        protected Hair _hair;

        public Role(uint ID)
            : base(ID)
        {
        }

        public virtual Model Look
        {
            get { return _look; }
            set { _look = value; }
        }

        public virtual Hair Hairstyle
        {
            get { return _hair; }
            set { _hair = value; }
        }

        /// <remarks>Taken from COSV3</remarks>
        public struct Hair
        {
            public int Id;

            public Hair(int id)
            {
                Id = id;
            }

            public Hair(int kind, int color)
            {
                Id = (color*100) + kind;
            }

            public int Color
            {
                get { return Id/100; }
                set { Id = (value*100) + Kind; }
            }

            public int Kind
            {
                get { return Id%100; }
                set { Id = (Color*100) + value; }
            }

            public static implicit operator Int16(Hair hair)
            {
                return (Int16) hair.Id;
            }
        }

        /// <remarks>Taken from COSV3</remarks>
        public class Model
        {
            public enum SexType
            {
                Asexual = 0,
                Male = 1,
                Female = 2
            }

            public static readonly Model None = new Model(0, SexType.Asexual, 0);

            public Int32 Id;

            public Model(Int32 id)
            {
                Id = id;
            }

            /// <summary>
            ///     Initializes a new instance of the <see cref="Model" /> class.
            /// </summary>
            /// <param name="mesh">The mesh.</param>
            /// <param name="sex">The sex.</param>
            /// <param name="avatar">The avatar.</param>
            /// <param name="hair">The hair.</param>
            public Model(Int32 mesh, SexType sex, Int32 avatar)
            {
                Id = (avatar*10000 + ((Int32) sex*1000) + mesh);
            }

            public Int32 Mesh
            {
                get { return Id%100; }
                set { Id = (Id/1000)*1000 + value; }
            }

            /// <summary>
            ///     Gets or sets the sex.
            /// </summary>
            /// <value>It's the sexual, pony is for sexual.</value>
            public SexType Sex
            {
                get { return (SexType) ((Id/1000)%10); }
                set { Id = ((Id/10000)*10000) + ((Int32) value*1000) + Mesh; }
            }

            /// <summary>
            ///     Gets or sets the avatar.
            /// </summary>
            /// <value>The avatar.</value>
            public Int32 Avatar
            {
                get { return (Id/10000)%100; }
                set { Id = (Overlay*10000000) + (value*10000) + (Id%10000); }
            }

            /// <summary>
            ///     Gets or sets the overlay.
            /// </summary>
            /// <value>The overlay.</value>
            public Int32 Overlay
            {
                get { return Id/10000000; }
                set { Id = (value*10000000) + (Id%10000000); }
            }

            public static implicit operator Int32(Model mdl)
            {
                return mdl.Id;
            }

            public override string ToString()
            {
                return "Mesh:{0} Sex:{1} Avatar:{2} Overlay:{3}".Interpolate(Mesh, Sex, Avatar, Overlay);
            }
        }
    }
}