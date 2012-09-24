// -----------------------------------------------------------------------
// <copyright file="GameStartEvent.cs">
// Copyright 2012 Robert Nix, Will Eddins
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
