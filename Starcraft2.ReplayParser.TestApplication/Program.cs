using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Starcraft2.ReplayParser.TestApplication
{
    class Program
    {
        static void Main()
        {
            string appPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            appPath = Path.GetDirectoryName(appPath);

            Replay.AddChatMessage("badreplay.SC2replay", "Welcome to AscendTV.", 1, 5);
            var replay = Replay.Parse("badreplay.SC2replay");
            
            //BenchmarkReplay(Path.Combine(appPath, "testReplay.1.1.3.SC2Replay"));
            //BenchmarkReplay(Path.Combine(appPath, "testReplay.1.2.SC2Replay"));

            // Replace this with your local Starcraft 2's replay folder to  test parallel parsing.
            const string replayLocation = @"C:\Users\Will\Documents\StarCraft II\Accounts\1300563\1-S2-1-268325\Replays\Unsaved\Multiplayer";
            
            if (Directory.Exists(replayLocation))
            {
                BenchmarkParallel(replayLocation);    
            }
            else
            {
                Console.WriteLine("Cannot test parallel parsing - Directory does not exist.");
            }

            Console.WriteLine("Press enter to quit.");
            Console.ReadLine();
        }

        private static void BenchmarkParallel(string replayLocation)
        {
            var watch = new Stopwatch();

            string[] replayFiles = Directory.GetFiles(replayLocation);

            Console.Out.WriteLine("Testing Parallel Parsing Speed: {0} Replay Files", replayFiles.Length);

            object replayLock = new object();
            var replays = new List<Replay>();

            watch.Reset();
            watch.Start();

            Parallel.ForEach(replayFiles, replayFilename =>
            {
                var rep = Replay.Parse(replayFilename);

                lock (replayLock)
                {
                    replays.Add(rep);
                }
            });

            watch.Stop();

            Console.Out.WriteLine("Total time: {0}ms. Average: {1}ms.",
                                  watch.ElapsedMilliseconds, watch.ElapsedMilliseconds / (double)replayFiles.Length);
        }

        private static void BenchmarkReplay(string filePath)
        {
            Replay replay = Replay.Parse(filePath);

            Console.Out.Write("Replay players: ");
            
            foreach (var player in replay.Players)
            {
                Console.Out.Write(player.Name + " ");
            }

            Console.Out.WriteLine();

            Console.Out.WriteLine("Testing Parsing Speed: 1000 Replay Files");

            var watch = new Stopwatch();
            watch.Start();

            for (int i = 0; i < 1000; i++)
            {
                Replay.Parse(filePath);
            }

            watch.Stop();

            Console.Out.WriteLine("Total time: {0}ms. Average: {1}ms.",
                                  watch.ElapsedMilliseconds, watch.ElapsedMilliseconds / 1000.0);
        }
    }
}
