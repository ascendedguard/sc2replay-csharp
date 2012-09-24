// -----------------------------------------------------------------------
// <copyright file="SendResourcesEvent.cs">
// Copyright 2012 Robert Nix, Will Eddins
// </copyright>
// <summary>
//   In-Game event triggered when a player sends resources.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Starcraft2.ReplayParser
{
    using Streams;

    /// <summary>
    /// In-Game event triggered when a player sends resources.
    /// </summary>
    public class SendResourcesEvent : GameEventBase
    {
        public SendResourcesEvent(BitReader bitReader, Replay replay)
        {
            this.EventType = GameEventType.Other;

            var playerId = (int)bitReader.Read(4);
            Target = replay.GetPlayerById(playerId);

            var someFlags = (int)bitReader.Read(3);

            if (someFlags-- > 0) // 4
            {
                MineralsSent = ReadSignedAmount(bitReader.Read(32));
            }
            if (someFlags-- > 0) // 3
            {
                VespeneSent = ReadSignedAmount(bitReader.Read(32));
            }
            if (someFlags-- > 0) // 2
            {
                TerrazineSent = ReadSignedAmount(bitReader.Read(32));
            }
            if (someFlags-- > 0) // 1
            {
                CustomSent = ReadSignedAmount(bitReader.Read(32));
            }
        }

        int ReadSignedAmount(uint amount)
        {
            int result = ((amount & 0x80000000) != 0) ? 1 : -1;

            return result * (int)(amount & 0x7fffffff);
        }

        /// <summary> Amount of resources sent </summary>
        public int MineralsSent { get; private set; }

        /// <summary> Amount of resources sent </summary>
        public int VespeneSent { get; private set; }

        /// <summary> Amount of resources sent </summary>
        public int TerrazineSent { get; private set; }

        /// <summary> Amount of resources sent </summary>
        public int CustomSent { get; private set; }

        /// <summary> The target of the trade </summary>
        public Player Target { get; private set; }
    }
}
