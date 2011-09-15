// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GameType.cs" company="SC2ReplayParser">
//   Copyright © 2011 All Rights Reserved
// </copyright>
// <summary>
//   Describes the type of game played, whether on ladder or a custom game.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Starcraft2.ReplayParser
{
    /// <summary>
    /// Describes the type of game played, whether on ladder or a custom game.
    /// </summary>
    public enum GameType
    {
        /// <summary>
        /// Custom game created by a player.
        /// </summary>
        Private, 

        /// <summary>
        /// Open player, played on the Battle.NET ladder system.
        /// </summary>
        Open, 

        /// <summary>
        /// Single Player campaign replay.
        /// </summary>
        SinglePlayer
    }
}