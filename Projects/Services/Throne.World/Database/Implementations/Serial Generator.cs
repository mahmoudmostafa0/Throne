using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Throne.Framework.Logging;
using Throne.Framework.Persistence.Mapping;
using Throne.Framework.Threading;

namespace Throne.World.Database.Records.Implementations
{
    public class SerialGenerator : WorldDatabaseRecord
    {
        private long _serial;

        /// <summary>
        ///     Used for the database layer
        /// </summary>
        protected SerialGenerator()
        {
        }

        /// <summary> Creates a new instance of the <code>SerialGenerator</code> </summary>
        /// <remarks>Should be saved to the database.</remarks>
        /// <param name="for">
        ///     Generally the name of the class which will use the new instance of <code>SerialGenerator</code>
        /// </param>
        /// <param name="minVal">Lowest possible serial. The generator will start with this value.</param>
        /// <param name="maxVal">
        ///     Highest possible serial. The generator will warn if the highest value is close to 95% of this
        ///     value, but will not prevent new serials from being generated.
        /// </param>
        public SerialGenerator(String @for, UInt32 minVal, UInt32 maxVal)
        {
            Minimum =
                Highest = minVal;
            Maximum = maxVal;
            Id = @for;
        }


        public virtual UInt32 LastId
        {
            get
            {
                _serial = Highest;
                return (UInt32) Interlocked.Read(ref _serial);
            }
        }

        public virtual String Id { get; protected set; }
        public virtual UInt32 Minimum { get; protected set; }
        public virtual UInt32 Maximum { get; protected set; }

        /// <summary>
        ///     The current or highest serial in use.
        /// </summary>
        public virtual UInt32 Highest
        {
            get { return (uint) _serial; }
            set { _serial = value; }
        }

        /// <summary> Gets or creates (if necessary) a new instance of the <code>SerialGenerator</code> </summary>
        /// <remarks>Should be saved to the database.</remarks>
        /// <param name="for">
        ///     Generally the name of the class which will use the new instance of <code>SerialGenerator</code>
        /// </param>
        /// <param name="minVal">Lowest possible serial. The generator will start with this value.</param>
        /// <param name="maxVal">
        ///     Highest possible serial. The generator will warn if the highest value is close to 95% of this
        ///     value, but will not prevent new serials from being generated.
        /// </param>
        public virtual SerialGenerator GetSerialGenerator(String @for, UInt32 minVal, UInt32 maxVal)
        {
            SerialGenerator sg = null;
            WorldServer.Instance.WorldDbContext.PostWait(
                ctx => sg = ctx.Find<SerialGenerator>(g => g.Id == @for).First());

            return sg ?? new SerialGenerator(@for, minVal, maxVal);
        }

        /// <summary>
        ///     Next serial.
        /// </summary>
        /// <returns>The increment of <code>Highest</code></returns>
        public virtual UInt32 Next()
        {
            _serial = Highest;
            try
            {
                return (UInt32) Interlocked.Increment(ref _serial);
            }
            finally
            {
                Update();
            }
        }
    }

    public sealed class SerialGeneratorManager : SingletonActor<SerialGeneratorManager>
    {
        public readonly LogProxy Log;
        private readonly List<SerialGenerator> _generators;

        private SerialGeneratorManager()
        {
            Log = new LogProxy("SerialGenerator");
            Log.Info("Loading serial generators...");

            _generators = new List<SerialGenerator>();
            var generators =
                WorldServer.Instance.WorldDbContext.FindAll<SerialGenerator>().ToList();
            foreach (var acc in generators)
                _generators.Add(acc);

            Log.Info("Loaded serial generators.");
        }

        private SerialGenerator CreateSerialGenerator(String @for, UInt32 minVal, UInt32 maxVal)
        {
            var generator = new SerialGenerator(@for, minVal, maxVal);
            generator.Create();
            _generators.Add(generator);
            return generator;
        }

        public SerialGenerator GetSerialGenerator(String @for, UInt32 minVal, UInt32 maxVal)
        {
            return _generators.Find(generator => generator.Id == @for) ?? CreateSerialGenerator(@for, minVal, maxVal);
        }

        /// <summary>
        ///     Used implicitly at <code>ActorApplication.Heartbeat</code>
        /// </summary>
        public void UpdateGenerators()
        {
            foreach (var serialGenerator in _generators)
            {
                serialGenerator.Update();
                if (serialGenerator.Maximum - serialGenerator.LastId > serialGenerator.Maximum*0.95)
                    Log.Warn("{0}Id - near it's maximum value.", serialGenerator.Id);
            }
        }
    }

    public sealed class SerialGeneratorMapping : MappableObject<SerialGenerator>
    {
        public SerialGeneratorMapping()
        {
            Id(r => r.Id).GeneratedBy.Assigned();
            Map(sg => sg.Minimum);
            Map(sg => sg.Maximum);
            Map(sg => sg.Highest);
        }
    }
}