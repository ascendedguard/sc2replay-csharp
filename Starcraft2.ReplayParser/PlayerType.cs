// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlayerType.cs" company="SC2ReplayParser">
//   Copyright © 2011 All Rights Reserved
// </copyright>
// <summary>
//   Enumeration describing whether a player is a human or computer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Starcraft2.ReplayParser
{
    /// <summary>
    /// Enumeration describing whether a player is a human or computer.
    /// </summary>
    public enum PlayerType
    {
        /// <summary>
        /// Human player.
        /// </summary>
        Human, 

        /// <summary>
        /// CPU-controlled player.
        /// </summary>
        Computer,

        /// <summary>
        /// Observer
        /// </summary>
        Spectator
    }
}