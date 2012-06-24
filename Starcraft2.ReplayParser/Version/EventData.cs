// -----------------------------------------------------------------------
// <copyright file="EventData.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Starcraft2.ReplayParser.Version
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Provides a map from ability type to game event type
    /// </summary>
    public class EventData : DataFile
    {
        EventData()
            : base("events.dat")
        {
        }

        public GameEventType GetEventType(AbilityType type)
        {
            return (GameEventType)Data[(int)type];
        }

        private static EventData singleton;

        public static EventData GetInstance()
        {
            if (singleton == null)
            {
                singleton = new EventData();
            }

            return singleton;
        }
    }
}
