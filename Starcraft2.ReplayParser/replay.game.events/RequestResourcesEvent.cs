// -----------------------------------------------------------------------
// <copyright file="RequestResourcesEvent.cs" company="Microsoft">
// TODO: Update copyright text.
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
            if (someFlags != 4) // Debug: if this isn't 4, we're probably fucked.
            {       // I actually don't think they're flags, but an array length.
                var zero = 0d;
            }

            MineralsRequested = ReadSignedAmount(bitReader.Read(32));
            VespeneRequested = ReadSignedAmount(bitReader.Read(32));
            TerrazineRequested = ReadSignedAmount(bitReader.Read(32));
            CustomRequested = ReadSignedAmount(bitReader.Read(32));
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
