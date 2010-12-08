using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Starcraft2.ReplayParser
{
    public class ReplayAttributeEvents
    {
        public ReplayAttribute[] Attributes { get; set; }

        public static ReplayAttributeEvents Parse(byte[] buffer)
        {
            var numAttributes = BitConverter.ToInt32(buffer, 4);

            var attributes = new ReplayAttribute[numAttributes];

            for (int i = 0; i < numAttributes; i++)
            {
                attributes[i] = ReplayAttribute.Parse(buffer, 8 + (i*13)); // attributes[i] = ReplayAttribute.Parse(reader);
            }

            var rae = new ReplayAttributeEvents { Attributes = attributes };
            return rae;
        }

        /// <summary>
        /// Applies the set of attributes to a replay.
        /// </summary>
        /// <param name="replay">Replay to apply the attributes to.</param>
        public void ApplyAttributes(Replay replay)
        {
            // I'm not entirely sure this is the right encoding here. Might be unicode...
            Encoding encoding = Encoding.ASCII;

            // Player Types (Human, Computer)
            var playerTypes = from a in Attributes
                              where a.AttributeId == PlayerTypeAttribute
                              select a;

            foreach (var playerType in playerTypes)
            {
                if (playerType.PlayerId > replay.Players.Length)
                {
                    // Player doesn't exist for some reason...
                    continue;
                }

                string type = encoding.GetString(playerType.Value.Reverse().ToArray());
                
                // "Comp" for computer, "Humn" for human.
                if (type.ToLower().Equals("comp"))
                {
                    replay.Players[playerType.PlayerId - 1].PlayerType = PlayerType.Computer;    
                }
                else
                {
                    replay.Players[playerType.PlayerId - 1].PlayerType = PlayerType.Human;    
                }
            }

            // Game Speed
            var gameSpeed = (from a in Attributes
                             where a.AttributeId == GameSpeedAttribute
                             select a).Single();

            string speed = encoding.GetString(gameSpeed.Value.Reverse().ToArray());
            speed = speed.ToLower();
            switch(speed)
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

            // Difficulty
            var difficulties = from a in Attributes
                               where a.AttributeId == DifficultyLevelAttribute
                               select a;

            foreach (var diff in difficulties)
            {
                if (diff.PlayerId > replay.Players.Length)
                {
                    // Player doesn't exist for some reason...
                    continue;
                }

                string diffLevel = encoding.GetString(diff.Value.Reverse().ToArray());
                diffLevel = diffLevel.ToLower();

                var player = replay.Players[diff.PlayerId - 1];

                switch(diffLevel)
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
            }

            // Team Size
            var teamSize = (from a in Attributes
                             where a.AttributeId == TeamSizeAttribute
                             select a).Single();

            // This fixes issues with reversing the string before encoding. Without this, you get "\01v1"
            var teamSizeChar = encoding.GetString(teamSize.Value, 0, 3).Reverse().ToArray();

            replay.TeamSize = new string(teamSizeChar);

            var gameType = (from a in Attributes
                             where a.AttributeId == GameTypeAttribute
                             select a).Single();

            string gameTypeStr = encoding.GetString(gameType.Value.Reverse().ToArray());
            gameTypeStr = gameTypeStr.ToLower();

            switch(gameTypeStr)
            {
                case "priv":
                    replay.GameType = GameType.Private;
                    break;
                case "amm":
                    replay.GameType = GameType.Open;
                    break;
            }

            // We may parse the player Teams if this file turns out to be more reliable for finding a player's team number.
            // Skipping parsing the handicap, player teams, colors, and handicap since this is parsed elsewhere.
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
