using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Starcraft2.ReplayParser
{
    public enum GameEventType
    {
        /// <summary> The event type is unknown. </summary>
        Unknown = 0,

        Macro,

        Micro,

        Selection,

        Other,

        /// <summary> A static event that does not contribute towards APM. </summary>
        Inactive,
        
    }
}
