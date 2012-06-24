// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GameEventType.cs" company="SC2ReplayParser">
//   Copyright © 2011 All Rights Reserved
// </copyright>
// <summary>
//   Describes a classification of events.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Starcraft2.ReplayParser
{
    /// <summary>
    /// Describes a classification of events.
    /// </summary>
    public enum GameEventType
    {
        /// <summary>
        /// Unknown event
        /// </summary>
        Unknown = 0, 

        /// <summary>
        /// The event is considered macro, or pertaining to a player's economy building.
        /// </summary>
        Macro, 

        /// <summary>
        /// The event is considered micro, or pertaining to a player's unit control.
        /// </summary>
        Micro, 

        /// <summary>
        /// Getting upgrades or building tech structures
        /// </summary>
        Tech,

        /// <summary>
        /// An ability that is caused by a right click
        /// </summary>
        RightClick,

        /// <summary>
        /// Unit selection
        /// </summary>
        Selection, 

        /// <summary>
        /// Other event type
        /// </summary>
        Other, 

        /// <summary>
        /// A static event that does not contribute towards APM.
        /// </summary>
        Inactive, 
    }
}