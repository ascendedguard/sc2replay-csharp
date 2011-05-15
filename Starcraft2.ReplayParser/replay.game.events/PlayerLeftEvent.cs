using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Starcraft2.ReplayParser
{
    public class PlayerLeftEvent : GameEventBase
    {
        public PlayerLeftEvent()
        {
            this.EventType = GameEventType.Inactive;
        }

        public PlayerLeftEvent(Player player, Timestamp time) : base(player, time)
        {
            this.EventType = GameEventType.Inactive;
        }

        public override string ToString()
        {
            return string.Format("{0} has left the game!", Player.Name);
        }
    }
}
