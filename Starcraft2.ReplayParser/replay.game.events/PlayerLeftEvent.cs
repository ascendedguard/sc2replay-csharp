// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlayerLeftEvent.cs" company="SC2ReplayParser">
//   Copyright © 2011 All Rights Reserved
// </copyright>
// <summary>
//   An event where a player has left the game.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Starcraft2.ReplayParser
{
    /// <summary>
    /// An event where a player has left the game.
    /// </summary>
    public class PlayerLeftEvent : GameEventBase
    {
        #region Constructors and Destructors

        /// <summary> Initializes a new instance of the <see cref="PlayerLeftEvent"/> class. </summary>
        public PlayerLeftEvent(Player player)
        {
            this.EventType = GameEventType.Inactive;
        }

        /// <summary> Initializes a new instance of the <see cref="PlayerLeftEvent"/> class. </summary>
        /// <param name="player"> The player who has left. </param>
        /// <param name="time"> The time at which the event occured. </param>
        public PlayerLeftEvent(Player player, Timestamp time)
            : base(player, time)
        {
            this.EventType = GameEventType.Inactive;
        }

        #endregion

        #region Public Methods

        /// <summary> Overrides ToString, formatting the event similar to how it would appear in the chat log. </summary>
        /// <returns> Returns the event as if it had occured in the chat log. </returns>
        public override string ToString()
        {
            return string.Format("{0} has left the game!", this.Player.Name);
        }

        #endregion
    }
}