// -----------------------------------------------------------------------
// <copyright file="PlayerJoinEvent.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Starcraft2.ReplayParser
{
    using Streams;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class PlayerJoinEvent : GameEventBase
    {
        public PlayerJoinEvent(BitReader bitReader)
        {
            this.EventType = GameEventType.Inactive;
            this.JoinFlags = (int)bitReader.Read(4);
        }

        public int JoinFlags { get; private set; }
    }
}
