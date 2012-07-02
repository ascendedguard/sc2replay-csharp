// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IGameEvent.cs" company="SC2ReplayParser">
//   Copyright © 2011 All Rights Reserved
// </copyright>
// <summary>
//   Interface for a basic game event.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Starcraft2.ReplayParser
{
    using Streams;

    /// <summary> Interface for a basic game event. </summary>
    public interface IGameEvent
    {
        #region Public Properties

        /// <summary> Gets or sets the type of event. </summary>
        GameEventType EventType { get; set; }

        /// <summary> Gets or sets the player performing the event. </summary>
        Player Player { get; set; }

        /// <summary> Gets or sets the timestamp at which the event occured. </summary>
        Timestamp Time { get; set; }

        #endregion
    }
}