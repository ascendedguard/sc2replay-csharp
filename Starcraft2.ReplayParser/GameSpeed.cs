// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GameSpeed.cs" company="SC2ReplayParser">
//   Copyright © 2011 All Rights Reserved
// </copyright>
// <summary>
//   Describes the game speed a game was originally played at.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Starcraft2.ReplayParser
{
    /// <summary>
    /// Describes the game speed a game was originally played at.
    /// </summary>
    public enum GameSpeed
    {
        /// <summary>
        /// Unknown game speed.
        /// </summary>
        Unknown = 0, 

        /// <summary>
        /// Slower speed setting.
        /// </summary>
        Slower = 1, 

        /// <summary>
        /// Slow speed setting.
        /// </summary>
        Slow, 

        /// <summary>
        /// Normal game speed.
        /// </summary>
        Normal, 

        /// <summary>
        /// Fast speed setting.
        /// </summary>
        Fast, 

        /// <summary>
        /// Faster speed setting.
        /// </summary>
        Faster, 
    }
}