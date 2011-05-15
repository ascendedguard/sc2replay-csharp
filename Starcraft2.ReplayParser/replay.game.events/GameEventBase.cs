using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Starcraft2.ReplayParser
{
    public class GameEventBase : IGameEvent
    {
        public GameEventBase()
        {
            
        }

        public GameEventBase(Player player, Timestamp time)
        {
            this.Player = player;
            this.Time = time;
        }

        public GameEventType EventType { get; set; }

        public Player Player { get; set; }

        public Timestamp Time { get; set; }
    }
}
