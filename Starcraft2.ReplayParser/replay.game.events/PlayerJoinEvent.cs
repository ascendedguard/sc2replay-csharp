// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlayerJoinEvent.cs">
//   Copyright © 2012 All Rights Reserved
// </copyright>
// <summary>
//   In-game event signalling when a player has joined.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Starcraft2.ReplayParser
{
    using System.Collections.Generic;

    using Streams;

    /// <summary>
    /// In-game event signalling when a player has joined.
    /// </summary>
    public class PlayerJoinEvent : GameEventBase
    {
        public PlayerJoinEvent(BitReader bitReader, Replay replay, int playerIndex)
        {
            this.EventType = GameEventType.Inactive;

            // This should probably be a series of {shl; or} on .Read(1)
            // to make it version-independent
            if (replay.ReplayBuild < 22612)
            {
                this.JoinFlags = (int)bitReader.Read(4);
            }
            else
            {
                this.JoinFlags = (int)bitReader.Read(12); // unknown
            }

            // Initialize player if not exists (true for observers)
            Player player = replay.GetPlayerById(playerIndex);
            if (player == null)
            {
                var p = new Player { PlayerType = PlayerType.Spectator };
                replay.ClientList[playerIndex] = player = p;
            }

            // Initialize wireframe
            player.Wireframe = new List<Unit>();
            player.WireframeSubgroup = 0;

            // Initialize control groups
            player.Hotkeys = new List<Unit>[10];
        }

        public int JoinFlags { get; private set; }
    }
}
