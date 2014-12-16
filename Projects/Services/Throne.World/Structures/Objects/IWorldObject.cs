using System;
using Throne.World.Network;
using Throne.World.Structures.Travel;

namespace Throne.World.Structures.Objects
{
    /// <summary>
    /// Interface to all world objects.
    /// </summary>
    public interface IWorldObject
    {
        UInt32 ID { get; }  // The unique identifier of the object.
        Location Location { get; set; }

        /// <summary>
        /// This method defines the spawn method of the object. It may be set by a script or by the server; however,
        /// in the end, it is called to spawn an object to the observer's screen.
        /// </summary>
        /// <param name="observer">The observer the object is being spawned to.</param>
        void SpawnFor(WorldClient observer);
    }
}
