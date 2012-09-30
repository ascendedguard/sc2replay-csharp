namespace Starcraft2.ReplayParser.Tests
{
    using NUnit.Framework;

    [TestFixture]
    public class Replay1v1Tests
    {
        /// <summary>
        /// The list of replays checked in each test.
        /// </summary>
        /// <remarks>
        /// Add replays to the /Replays/ directory in this project. Add the replay in visual studio,
        /// and ensure 'Copy to Output Directory' is set to 'Copy if newer'.
        /// </remarks>
        public static string[] TestReplays = 
                { 
                    "./Replays/testReplay.1.1.3.SC2Replay", 
                    "./Replays/testReplay.1.2.SC2Replay", 
                    "./Replays/testReplay.1.3.4.SC2Replay", 
                    "./Replays/testReplay.1.4.3.SC2Replay", 
                    "./Replays/testReplay.1.5.3.SC2Replay", 
                };

        /// <summary>
        /// A basic test ensuring that a replay parses without throwing an exception.
        /// </summary>
        /// <param name="filename">Path to replay.</param>
        [Test, TestCaseSource("TestReplays")]
        public void ReplayParsesWithoutError(string filename)
        {
            Replay replay = Replay.Parse(filename);
            
            Assert.That(replay != null);
        }

        /// <summary>
        /// Ensures parsing without events does just that, while still succeeding.
        /// </summary>
        /// <param name="filename">Path to replay.</param>
        [Test, TestCaseSource("TestReplays")]
        public void ReplayParseWithoutEvents(string filename)
        {
            Replay replay = Replay.Parse(filename, true);

            Assert.That(replay != null);
            Assert.That(replay.PlayerEvents == null, "The replay still had parsed events.");
        }

        /// <summary>
        /// Makes sure map information was successfully extracted.
        /// </summary>
        /// <param name="filename">Path to replay.</param>
        [Test, TestCaseSource("TestReplays")]
        public void ReplayHasValidMapInfo(string filename)
        {
            Replay replay = Replay.Parse(filename);

            Assert.IsNotNullOrEmpty(replay.Map);
            Assert.IsNotNullOrEmpty(replay.MapGateway);
            Assert.IsNotNullOrEmpty(replay.MapPreviewName);
            Assert.That(replay.MapHash != null);
        }

        /// <summary>
        /// Asserts that there are only 2 active players. We are only testing 1v1 in this test class.
        /// </summary>
        /// <param name="filename">Path to replay.</param>
        [Test, TestCaseSource("TestReplays")]
        public void ReplayHasTwoPlayers(string filename)
        {
            Replay replay = Replay.Parse(filename);

            Assert.That(replay.Players.Length == 2, "Replay didn't have 2 players in a 1v1.");
            Assert.That(replay.TeamSize.Equals("1v1"));

            foreach (var player in replay.Players)
            {
                Assert.IsNotNullOrEmpty(player.Name, "Player had an empty name.");
                Assert.IsNotNullOrEmpty(player.Color, "Player color was missing.");
            }
        }
    }
}
