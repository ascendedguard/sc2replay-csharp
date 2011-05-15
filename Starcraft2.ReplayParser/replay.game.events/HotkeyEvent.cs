using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Starcraft2.ReplayParser
{
    public class HotkeyEvent : GameEventBase
    {
        public HotkeyEvent()
        {
            this.EventType = GameEventType.Other;
        }

        public HotkeyEvent(Player player, Timestamp time) : base(player, time)
        {
            this.EventType = GameEventType.Other;
        }

        public int Key { get; set; }

        public int Action { get; set; }
    }
}
