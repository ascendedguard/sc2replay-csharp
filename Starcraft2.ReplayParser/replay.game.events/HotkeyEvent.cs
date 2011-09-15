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
    /// <summary> Describes an event where the player has set or used a hotkey. </summary>
    public class HotkeyEvent : GameEventBase
    {
        #region Constructors and Destructors

        /// <summary> Initializes a new instance of the <see cref="HotkeyEvent"/> class. </summary>
        public HotkeyEvent()
        {
            this.EventType = GameEventType.Other;
        }

        /// <summary> Initializes a new instance of the <see cref="HotkeyEvent"/> class. </summary>
        /// <param name="player"> The player pressing the hotkey. </param>
        /// <param name="time"> The time at which the event occured. </param>
        public HotkeyEvent(Player player, Timestamp time)
            : base(player, time)
        {
            this.EventType = GameEventType.Other;
        }

        #endregion

        #region Public Properties

        /// <summary> Gets or sets the action performed. </summary>
        public int Action { get; set; }

        /// <summary> Gets or sets the pressed key. </summary>
        public int Key { get; set; }

        #endregion
    }
}