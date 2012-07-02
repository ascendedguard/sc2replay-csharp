// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Ascend">
//   Copyright 2012 All Rights Reserved
// </copyright>
// <summary>
//   Defines the Program type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Starcraft2.ReplayParser.TestApplication
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    /// <summary> Main application entry point </summary>
    public class Program
    {
        /// <summary> Main application entry point </summary>
        public static void Main()
        {
            // If you'd like to test a specific folder, replace this line.
            var replayFolder = GetDefaultReplayDirectory();

            if (replayFolder == null)
            {
                // Close and do nothing if the parsing directory was invalid.
                return;
            }

            var replayFiles = Directory.GetFiles(replayFolder, "*.SC2Replay", SearchOption.AllDirectories);

            int filesTotal = replayFiles.Length;
            int filesSucceeded = 0;

            if (filesTotal == 0)
            {
                Console.Out.WriteLine("No replays found in selected directory.");
                return;
            }

            foreach (string replay in replayFiles)
            {
                try
                {
                    BenchmarkReplay(replay);
                    filesSucceeded++;
                }
                catch
                {
                    Console.WriteLine("Failed to parse: " + Path.GetFileName(replay));
                }
            }
            
            Console.WriteLine(string.Format("Total Parsed: {0}/{1} ({2}%)", filesSucceeded, filesTotal, filesSucceeded * 100 / filesTotal));

            Console.WriteLine("Press enter to quit.");
            Console.ReadLine();
        }

        private static void BenchmarkReplay(string filePath)
        {
            Replay replay = Replay.Parse(filePath);

            CalculateAPM(replay);

            Console.Out.Write("Replay players: ");
        
            foreach (var player in replay.Players)
            {
                if (player != null)
                {
                    if (player.IsWinner)
                    {
                        Console.Out.Write("*");
                    }

                    Console.Out.Write(player.Name + " ");
                }
            }

            Console.Out.WriteLine();
        }

        private static void CalculateAPM(Replay replay)
        {
            var events = replay.PlayerEvents;

            if (events == null)
            {
                // This is experimental. With older replays, it appears to return a close approximation.
                return;
            }

            var eventGroups = events.Where(r => r.Player != null)
                                    .Where(r => r.EventType != GameEventType.Inactive)
                                    .GroupBy(r => r.Player);

            foreach (var group in eventGroups)
            {
                var order = group.OrderBy((r) => r.Time);
                var last = order.Last();
                var count = group.Count();

                // Calculates APM per second.
                var apm = count / last.Time.TimeSpan.TotalSeconds;

                apm *= 60;
                
                Debug.WriteLine(last.Player.Name + "'s APM: " + apm);
            }
        }

        private static string GetDefaultReplayDirectory()
        {
            // The following gets a user's Starcraft II replay folder automatically.
            var sc2Accounts = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Starcraft II", "Accounts");

            if (Directory.Exists(sc2Accounts) == false)
            {
                Console.Out.WriteLine("Starcraft II is not installed on the system.");
                return null;
            }

            var accounts = Directory.GetDirectories(sc2Accounts);
            var selectedAccount = accounts.FirstOrDefault();

            if (selectedAccount == null || Directory.Exists(selectedAccount) == false)
            {
                // This likely shouldn't happen. I don't think these folders would
                // get created until the user has logged into the game for the first time.
                Console.Out.WriteLine("The user has never played Starcraft II, but it is installed.");
                return null;
            }

            var players = Directory.GetDirectories(selectedAccount);
            var selectedPlayer = players.FirstOrDefault();

            if (selectedPlayer == null || Directory.Exists(selectedPlayer) == false)
            {
                // I'm not sure why this would happen either. You CAN, however, have multiple players
                // registered to a single account. For example, my EU and US accounts were merged onto
                // a single account, and both players (with 1- and 2- regions) show up here.
                Console.Out.WriteLine("The user has never played Starcraft II, but it is installed.");
                return null;
            }

            var replayFolder = Path.Combine(selectedPlayer, "Replays", "Multiplayer");

            if (replayFolder == null || Directory.Exists(replayFolder) == false)
            {
                // This can happen if the user just has never saved any replays.
                Console.Out.WriteLine("The replay directory for the selected user does not exist.");
                return null;
            }

            return replayFolder;
        }
    }
}
