namespace Starcraft2.ReplayParser
{
    public class Player
    {        
        /// <summary>
        /// Gets or sets the player's name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the player's race.
        /// </summary>
        public string Race { get; set; }

        /// <summary>
        /// Gets or sets the type of player, whether he is human or computer. 
        /// </summary>
        public PlayerType PlayerType { get; set; }

        /// <summary>
        /// Gets or sets the difficulty of a computer player. 
        /// Human players will default to either Unknown or Medium.
        /// </summary>
        public Difficulty Difficulty { get; set; }

        /// <summary>
        /// Gets or sets the player's color.
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// Gets or sets the player's handicap.
        /// </summary>
        public int Handicap { get; set; }

        /// <summary>
        /// Gets or sets the player's team number.
        /// </summary>
        public int Team { get; set; }
    }
}
