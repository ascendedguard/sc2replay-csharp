namespace Starcraft2.ReplayParser.TestApplication
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    class Program
    {
        static void Main()
        {
            string appPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            appPath = Path.GetDirectoryName(appPath);

            BenchmarkReplay(Path.Combine(appPath, "testReplay.1.1.3.SC2Replay"));
            BenchmarkReplay(Path.Combine(appPath, "testReplay.1.2.SC2Replay"));
            BenchmarkReplay(Path.Combine(appPath, "testReplay.1.3.4.SC2Replay"));
            BenchmarkReplay(Path.Combine(appPath, "testReplay.korean.1.3.4.SC2Replay"));

            // Replace this with your local Starcraft 2's replay folder to  test parallel parsing.
            const string replayLocation = @"C:\Users\Will\Documents\StarCraft II\Accounts\1300563\1-S2-1-268325\Replays\Multiplayer";

            string[] replayFiles = Directory.GetFiles(replayLocation);

            int filesTotal = replayFiles.Length;
            int filesSucceeded = 0;
            
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
                Console.Out.Write(player.Name + " ");
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
                                    .GroupBy((r) => r.Player);

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
    }
}
