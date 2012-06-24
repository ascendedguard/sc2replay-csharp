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

            // Files between 1.0.0 and 1.0.3 are compatible up to 1.0.3
            // We won't test these because we don't support them in replay.game.events
            // BenchmarkReplay(Path.Combine(appPath, "testReplay.1.0.0.16117.SC2Replay"));
            BenchmarkReplay(Path.Combine(appPath, "190334.SC2Replay"));

            BenchmarkReplay(Path.Combine(appPath, "testReplay.1.1.0.16561.SC2Replay"));
            BenchmarkReplay(Path.Combine(appPath, "testReplay.1.1.1.16605.SC2Replay"));
            BenchmarkReplay(Path.Combine(appPath, "testReplay.1.1.2.16755.SC2Replay"));
            BenchmarkReplay(Path.Combine(appPath, "testReplay.1.1.3.16939.SC2Replay"));
            BenchmarkReplay(Path.Combine(appPath, "testReplay.1.2.0.17326.SC2Replay"));
            BenchmarkReplay(Path.Combine(appPath, "testReplay.1.2.1.17682.SC2Replay"));
            BenchmarkReplay(Path.Combine(appPath, "testReplay.1.2.2.17811.SC2Replay"));
            BenchmarkReplay(Path.Combine(appPath, "testReplay.1.3.0.18092.SC2Replay"));
            BenchmarkReplay(Path.Combine(appPath, "testReplay.1.3.1.18221.SC2Replay"));
            BenchmarkReplay(Path.Combine(appPath, "testReplay.1.3.2.18317.SC2Replay"));
            BenchmarkReplay(Path.Combine(appPath, "testReplay.1.3.3.18574.SC2Replay"));
            BenchmarkReplay(Path.Combine(appPath, "testReplay.1.3.4.18701.SC2Replay"));
            BenchmarkReplay(Path.Combine(appPath, "testReplay.1.3.5.19132.SC2Replay"));
            BenchmarkReplay(Path.Combine(appPath, "testReplay.1.3.6.19269.SC2Replay"));
            BenchmarkReplay(Path.Combine(appPath, "testReplay.1.4.0.19679.SC2Replay"));
            BenchmarkReplay(Path.Combine(appPath, "testReplay.1.4.1.19776.SC2Replay"));
            BenchmarkReplay(Path.Combine(appPath, "testReplay.1.4.2.20141.SC2Replay"));
            BenchmarkReplay(Path.Combine(appPath, "testReplay.1.4.3.21029.SC2Replay"));

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
                if (player != null)
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
    }
}
