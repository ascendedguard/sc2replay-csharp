namespace Starcraft2.ReplayParser.Tests
{
    using System.IO;

    using NUnit.Framework;

    [TestFixture]
    public class ReplayTests
    {
        /// <summary>
        /// The list of replays checked in each test.
        /// </summary>
        /// <remarks>
        /// Add replays to the /Replays/ directory in this project.
        /// </remarks>
        public static string[] TestReplays = Directory.GetFiles("../../Replays/", "*.SC2Replay");

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
        public void ReplayHasCorrectNumberOfPlayers(string filename)
        {
            Replay replay = Replay.Parse(filename);

            if (replay.TeamSize.Equals("1v1"))
            {
                Assert.That(replay.Players.Length == 2, "Replay didn't have 2 players in a 1v1.");
            }
            else if (replay.TeamSize.Equals("2v2"))
            {
                Assert.That(replay.Players.Length == 4, "Replay didn't have 4 players in a 2v2.");
            }
            else if (replay.TeamSize.Equals("3v3"))
            {
                Assert.That(replay.Players.Length == 6, "Replay didn't have 6 players in a 3v3.");
            }
            else if (replay.TeamSize.Equals("4v4"))
            {
                Assert.That(replay.Players.Length == 8, "Replay didn't have 8 players in a 4v4.");
            }
            
            foreach (var player in replay.Players)
            {
                Assert.IsNotNullOrEmpty(player.Name, "Player had an empty name.");
                Assert.IsNotNullOrEmpty(player.Color, "Player color was missing.");
            }
        }

        /// <summary>
        /// Asserts that the replay has an array of events, meaning the events were parsed correctly.
        /// </summary>
        /// <param name="filename">Path to replay.</param>
        [Test, TestCaseSource("TestReplays")]
        public void ReplayHasEvents(string filename)
        {
            Replay replay = Replay.Parse(filename);

            Assert.That(replay.PlayerEvents != null, "Events failed to parse.");
        }
    }
}
