using System;
using System.IO;

namespace Starcraft2.ReplayParser.TestApplication
{
    class Program
    {
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
        }
    }
}
