// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Player.cs" company="SC2ReplayParser">
//   Copyright © 2011 All Rights Reserved
// </copyright>
// <summary>
//   Describes an individual player in a replay.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Starcraft2.ReplayParser
{
    using System.Collections.Generic;

    /// <summary>
    /// Describes an individual player in a replay.
    /// </summary>
    public class Player
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the Battle.NET ID of a player, describing the URI to find the player profile.
        /// </summary>
        public int BattleNetId { get; set; }

        /// <summary>
        /// Gets or sets the Battle.NET Sub-ID of a player, describing the URI to find the player profile. 
        /// </summary>
        public int BattleNetSubId { get; set; }

        /// <summary>
        /// Gets or sets the player's color.
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// Gets or sets the difficulty of a computer player. 
        /// Human players will default to either Unknown or Medium.
        /// </summary>
        public Difficulty Difficulty { get; set; }

        /// <summary>
        /// Gets or sets the player's handicap.
        /// </summary>
        public int Handicap { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the player won the game.
        /// </summary>
        public bool IsWinner { get; set; }

        /// <summary>
        /// Gets or sets the player's name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type of player, whether he is human or computer.
        /// </summary>
        public PlayerType PlayerType { get; set; }

        /// <summary>
        /// Gets or sets the player's race. This race is usually in the replay's native language, and should be the actual race they spawned as.
        /// </summary>
        public string Race { get; set; }

        /// <summary>
        /// Gets or sets the race selected in the lobby. This includes whether the player selected Random.
        /// </summary>
        public Race SelectedRace { get; set; }

        /// <summary>
        /// Gets or sets the player's team number.
        /// </summary>
        public int Team { get; set; }

        /// <summary> The player's current wireframe </summary>
        internal List<Unit> Wireframe;

        /// <summary> Currently selected wireframe subgroup </summary>
        internal int WireframeSubgroup;

        /// <summary>
        /// The player's current control group setup.
        /// </summary>
        internal List<Unit>[] Hotkeys;

        internal static Player Global = new Player();

        #endregion
    }
}