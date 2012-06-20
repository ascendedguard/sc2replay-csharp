// -----------------------------------------------------------------------
// <copyright file="PlayerJoinEvent.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Starcraft2.ReplayParser
{
    using System.Collections.Generic;

    using Streams;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class PlayerJoinEvent : GameEventBase
    {
        public PlayerJoinEvent(BitReader bitReader, Player player)
        {
            this.EventType = GameEventType.Inactive;

            // This should probably be a series of {shl; or} on .Read(1)
            // to make it version-independent
            this.JoinFlags = (int)bitReader.Read(4);

            // Initialize wireframe
            player.Wireframe = new Unit[255];
            player.WireframeCount = 0;
            player.WireframeSubgroup = 0;

            // Initialize control groups
            player.Hotkeys = new List<Unit>[10];
        }

        public int JoinFlags { get; private set; }
    }
}
