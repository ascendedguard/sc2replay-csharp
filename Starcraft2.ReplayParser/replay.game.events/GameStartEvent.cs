// -----------------------------------------------------------------------
// <copyright file="GameStartEvent.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Starcraft2.ReplayParser
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GameStartEvent : GameEventBase
    {
        public GameStartEvent()
        {
            this.EventType = GameEventType.Inactive;
        }
    }
}
