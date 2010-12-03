using System.Windows.Media;

namespace Starcraft2.ReplayParser
{
    /// <summary>
    /// Class describing an individual player in a match.
    /// </summary>
    public class PlayerDetails
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
        /// Gets or sets the player's color.
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// Gets or sets the player's team number.
        /// </summary>
        public int Team { get; set; }
    }
}
