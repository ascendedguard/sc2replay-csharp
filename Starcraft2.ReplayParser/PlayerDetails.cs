using System.Windows.Media;

namespace Starcraft2.ReplayParser
{
    public class PlayerDetails
    {
        public string Name { get; set; }
        public string Race { get; set; }
        public PlayerType PlayerType { get; set; }
        public Color Color { get; set; }
        public int Team { get; set; }
    }
}
