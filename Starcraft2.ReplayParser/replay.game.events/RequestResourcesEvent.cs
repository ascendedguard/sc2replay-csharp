// -----------------------------------------------------------------------
// <copyright file="RequestResourcesEvent.cs">
// Copyright 2012 Robert Nix, Will Eddins
// </copyright>
// -----------------------------------------------------------------------

namespace Starcraft2.ReplayParser
{
    using Streams;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class RequestResourcesEvent : GameEventBase
    {
        public RequestResourcesEvent(BitReader bitReader, Replay replay)
        {
            this.EventType = GameEventType.Other;

            var someFlags = (int)bitReader.Read(3);
            
            if (someFlags-- > 0) // 4
            {
                MineralsRequested = ReadSignedAmount(bitReader.Read(32));
            }
            if (someFlags-- > 0) // 3
            {
                VespeneRequested = ReadSignedAmount(bitReader.Read(32));
            }
            if (someFlags-- > 0) // 2
            {
                TerrazineRequested = ReadSignedAmount(bitReader.Read(32));
            }
            if (someFlags-- > 0) // 1
            {
                CustomRequested = ReadSignedAmount(bitReader.Read(32));
            }
        }

        int ReadSignedAmount(uint amount)
        {
            int result = ((amount & 0x80000000) != 0) ? 1 : -1;

            return result * (int)(amount & 0x7fffffff);
        }

        /// <summary> Amount of resources requested </summary>
        public int MineralsRequested { get; private set; }

        /// <summary> Amount of resources requested </summary>
        public int VespeneRequested { get; private set; }

        /// <summary> Amount of resources requested </summary>
        public int TerrazineRequested { get; private set; }

        /// <summary> Amount of resources requested </summary>
        public int CustomRequested { get; private set; }
    }
}
