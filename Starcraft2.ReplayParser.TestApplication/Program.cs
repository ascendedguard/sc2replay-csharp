using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Starcraft2.ReplayParser.TestApplication
{
    class Program
    {
        private const string ReplayLocation = @"C:\Users\Will\Documents\StarCraft II\Accounts\1300563\1-S2-1-268325\Replays\Unsaved";

        static void Main()
        {
            string appPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            appPath = Path.GetDirectoryName(appPath);

            string replayPath = Path.Combine(appPath, "testReplay.SC2Replay");

            Replay replay = Replay.Parse(replayPath);

            Console.Out.Write("Replay players: ");

            foreach(var player in replay.Players)
            {
                Console.Out.Write(player.Name + " ");
            }

            Console.Out.WriteLine();

            Console.Out.WriteLine("Testing Parsing Speed: 1000 Replay Files");

            var watch = new Stopwatch();
            watch.Start();

            for(int i = 0; i < 1000; i++)
            {
                Replay.Parse(replayPath);
            }

            watch.Stop();

            Console.Out.WriteLine("Total time: {0}ms. Average: {1}ms.", 
                                  watch.ElapsedMilliseconds, watch.ElapsedMilliseconds / 1000.0);

            string[] replayFiles = Directory.GetFiles(ReplayLocation);

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


            Console.WriteLine("Press enter to quit.");
            Console.ReadLine();
        }
    }
}
