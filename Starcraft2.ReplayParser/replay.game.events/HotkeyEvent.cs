// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HotkeyEvent.cs" company="SC2ReplayParser">
//   Copyright © 2011 All Rights Reserved
// </copyright>
// <summary>
//   Describes an event where the player has set or used a hotkey.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Starcraft2.ReplayParser
{
    using Streams;

    /// <summary> Describes an event where the player has set or used a hotkey. </summary>
    public class HotkeyEvent : GameEventBase
    {
        public HotkeyEvent(BitReader bitReader, Replay replay)
        {
            // ...
        }
    }
}