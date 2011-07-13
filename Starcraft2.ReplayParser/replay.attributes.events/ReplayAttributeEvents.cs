using System;
  using System.IO;
using System.Linq;
using System.Text;

namespace Starcraft2.ReplayParser
{
    using System.Collections.Generic;

    public class ReplayAttributeEvents
    {
        public ReplayAttribute[] Attributes { get; set; }

        public static void Parse(Replay replay, byte[] buffer)
        {
            int headerSize = 4;

            if (replay.ReplayBuild >= 17326) // 1.2.0
            {
                headerSize = 5;
            }

            var numAttributes = BitConverter.ToInt32(buffer, headerSize);

            var attributes = new ReplayAttribute[numAttributes];

            int initialOffset = 4 + headerSize;

            for (int i = 0; i < numAttributes; i++)
            {
                attributes[i] = ReplayAttribute.Parse(buffer, initialOffset + (i*13));
            }

            var rae = new ReplayAttributeEvents { Attributes = attributes };
            rae.ApplyAttributes(replay);
        }

        /// <summary>
        /// Applies the set of attributes to a replay.
        /// </summary>
        /// <param name="replay">Replay to apply the attributes to.</param>
        public void ApplyAttributes(Replay replay)
        {
            // I'm not entirely sure this is the right encoding here. Might be unicode...
            Encoding encoding = Encoding.UTF8;

            var attributes1 = new List<ReplayAttribute>();
            var attributes2 = new List<ReplayAttribute>();
            var attributes3 = new List<ReplayAttribute>();
            var attributes4 = new List<ReplayAttribute>();
            var attributesffa = new List<ReplayAttribute>();

            foreach (var attribute in this.Attributes)
            {
                switch (attribute.AttributeId)
                {
                    case PlayerTypeAttribute: // 500
                        {
                            string type = encoding.GetString(attribute.Value.Reverse().ToArray());

                            if (type.ToLower().Equals("comp"))
                            {
                                replay.Players[attribute.PlayerId].PlayerType = PlayerType.Computer;
                            }
                            else if (type.ToLower().Equals("humn"))
                            {
                                replay.Players[attribute.PlayerId].PlayerType = PlayerType.Human;
                            }
                            else
                            {
                                throw new Exception("Unexpected value");
                            }

                            break;
                        }

                    case TeamSizeAttribute:
                        {
                            // This fixes issues with reversing the string before encoding. Without this, you get "\01v1"
                            var teamSizeChar = encoding.GetString(attribute.Value, 0, 3).Reverse().ToArray();

                            replay.TeamSize = new string(teamSizeChar);
                            break;
                        }

                    case DifficultyLevelAttribute:
                        {
                            string diffLevel = encoding.GetString(attribute.Value.Reverse().ToArray());
                            diffLevel = diffLevel.ToLower();

                            var player = replay.Players[attribute.PlayerId];

                            switch (diffLevel)
                            {
                                case "vyey":
                                    player.Difficulty = Difficulty.VeryEasy;
                                    break;
                                case "easy":
                                    player.Difficulty = Difficulty.Easy;
                                    break;
                                case "medi":
                                    player.Difficulty = Difficulty.Medium;
                                    break;
                                case "hard":
                                    player.Difficulty = Difficulty.Hard;
                                    break;
                                case "vyhd":
                                    player.Difficulty = Difficulty.VeryHard;
                                    break;
                                case "insa":
                                    player.Difficulty = Difficulty.Insane;
                                    break;
                            }

                            break;
                        }

                    case GameSpeedAttribute:
                        {
                            string speed = encoding.GetString(attribute.Value.Reverse().ToArray());
                            speed = speed.ToLower();
                            switch (speed)
                            {
                                case "slor":
                                    replay.GameSpeed = GameSpeed.Slower;
                                    break;
                                case "slow":
                                    replay.GameSpeed = GameSpeed.Slow;
                                    break;
                                case "norm":
                                    replay.GameSpeed = GameSpeed.Normal;
                                    break;
                                case "fast":
                                    replay.GameSpeed = GameSpeed.Fast;
                                    break;
                                case "fasr":
                                    replay.GameSpeed = GameSpeed.Faster;
                                    break;

                                // Otherwise, Game Speed will remain "Unknown"
                            }

                            break;
                        }

                    case PlayerRaceAttribute:
                        {
                            var race = encoding.GetString(attribute.Value.Reverse().ToArray()).ToLower();
                            var player = replay.Players[attribute.PlayerId];

                            switch (race)
                            {
                                case "prot":
                                    player.SelectedRace = Race.Protoss;
                                    break;
                                case "zerg":
                                    player.SelectedRace = Race.Zerg;
                                    break;
                                case "terr":
                                    player.SelectedRace = Race.Terran;
                                    break;
                                case "rand":
                                    player.SelectedRace = Race.Random;
                                    break;
                            }

                            break;
                        }

                    case PlayerTeam1v1Attribute:
                        {
                            attributes1.Add(attribute);
                            break;
                        }

                    case PlayerTeam2v2Attribute:
                        {
                            attributes2.Add(attribute);
                            break;
                        }

                    case PlayerTeam3v3Attribute:
                        {
                            attributes3.Add(attribute);
                            break;
                        }

                    case PlayerTeam4v4Attribute:
                        {
                            attributes4.Add(attribute);
                            break;
                        }

                    case PlayerTeamFFAAttribute:
                        {
                            attributesffa.Add(attribute);
                            break;
                        }


                    case GameTypeAttribute:
                        {
                            string gameTypeStr = encoding.GetString(attribute.Value.Reverse().ToArray());
                            gameTypeStr = gameTypeStr.ToLower().Trim('\0');

                            switch (gameTypeStr)
                            {
                                case "priv":
                                    replay.GameType = GameType.Private;
                                    break;
                                case "amm":
                                    replay.GameType = GameType.Open;
                                    break;
                            }

                            break;
                        }
                        
                }
            }

            List<ReplayAttribute> currentList = null;

            if (replay.TeamSize.Equals("1v1"))
            {
                currentList = attributes1;
            }
            else if (replay.TeamSize.Equals("2v2"))
            {
                currentList = attributes2;
            }
            else if (replay.TeamSize.Equals("3v3"))
            {
                currentList = attributes3;                
            }
            else if (replay.TeamSize.Equals("4v4"))
            {
                currentList = attributes4;
            }
            else if (replay.TeamSize.Equals("FFA"))
            {
                currentList = attributesffa;
            }

            if (currentList != null)
            {
                foreach (var att in currentList)
                {
                    // Reverse the values then parse, you don't notice the effects of this until theres 10+ teams o.o
                    var team = encoding.GetString(att.Value.Reverse().ToArray()).Trim('\0', 'T');
                    replay.Players[att.PlayerId].Team = int.Parse(team);
                }
            }

            // Skipping parsing the handicap, colors, and handicap since this is parsed elsewhere.
        }
             

        public const int PlayerTypeAttribute = 500;
        public const int TeamSizeAttribute = 2001;
        public const int PlayerTeam1v1Attribute = 2002;
        public const int PlayerTeam2v2Attribute = 2003;
        public const int PlayerTeam3v3Attribute = 2004;
        public const int PlayerTeam4v4Attribute = 2005;
        public const int PlayerTeamFFAAttribute = 2006;
        public const int GameSpeedAttribute = 3000;
        public const int PlayerRaceAttribute = 3001;
        public const int PlayerColorIndexAttribute = 3002;
        public const int PlayerHandicapAttribute = 3003;
        public const int DifficultyLevelAttribute = 3004;
        public const int GameTypeAttribute = 3009;
    }
}
