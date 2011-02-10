using System;
using System.IO;

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

        public static PlayerDetails Parse(BinaryReader reader)
        {
            reader.ReadBytes(4); // playerHeader
            byte doubleShortNameLength = reader.ReadByte(); // doubleShortNameLength
            int shortNameLength = doubleShortNameLength / 2;

            string shortName = new String(reader.ReadChars(shortNameLength));

            // These variables could be deleted, they're only there for debugging to ensure proper values.
            // Perhaps the expected values should be verified?
            byte[] unknown1 = reader.ReadBytes(3); // unknown1 "02 05 08"
            KeyValueStruct param1 = KeyValueStruct.Parse(reader); // param1 - key = "00 09"
            byte[] unknown2 = reader.ReadBytes(6); // unknown2 - Unknown
            KeyValueStruct param2 = KeyValueStruct.Parse(reader); // param2 - key = "04 09"
            byte[] unknown3 = reader.ReadBytes(2); // unknown3 - "06 02"

            // Unknown4 should be "04 02". There is an unknown number of bytes (1-4) before this.
            // Once i find unknown4, i can continue parsing.

            byte unknown4Start = reader.ReadByte();
            byte unknown4End = 0;

            while (unknown4End != 2)
            {
                while (unknown4Start != 4)
                {
                    unknown4Start = reader.ReadByte();
                }

                unknown4End = reader.ReadByte();
            }

            byte doubleRaceLength = reader.ReadByte();
            int raceLength = doubleRaceLength / 2;

            string race = new String(reader.ReadChars(raceLength));

            reader.ReadBytes(3); // unknown5
            
            var keys = new KeyValueStruct[9];
            // paramList - This contains the player's color and should eventually be parsed.
            for (int i = 0; i < 9; i++)
            {
                keys[i] = KeyValueStruct.Parse(reader);
            }

            return new PlayerDetails
                       {
                           Name = shortName,
                           Race = race,
                           Color = string.Format("#{0}{1}{2}{3}", keys[0].Value.ToString("X2"),
                                                 keys[1].Value.ToString("X2"),
                                                 keys[2].Value.ToString("X2"),
                                                 keys[3].Value.ToString("X2")),
//                    Color.FromArgb((byte)keys[0].Value, (byte)keys[1].Value, (byte)keys[2].Value,
//                                   (byte)keys[3].Value),
                Handicap = keys[6].Value,
                Team = keys[8].Value,
            };
        }
    }
}
