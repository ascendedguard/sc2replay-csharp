namespace Starcraft2.ReplayParser
{
    using System.Diagnostics;
    using System.IO;
    using System;
    using System.Collections.Generic;

    using Starcraft2.ReplayParser.Streams;

    public class ReplayGameEvents
    {
        public static List<IGameEvent> Parse(Replay replay, byte[] buffer)
        {
            // The GameEvent file changes significantly after 16561.
            // This is sometime around the first patch after release. Since
            // parsing replays this old should be extremely rare, I don't believe
            // its worth the effort to try to support both. If it is, it should be
            // done in another method.
            if (replay.ReplayBuild < 16561)
            {
                throw new NotSupportedException(
                    "Replay builds under 16561 are not supported for parsing GameEvent log.");
            }

            var events = new List<IGameEvent>();

            // Keep a reference to know the game length.
            var ticksElapsed = 0;

            using (var stream = new MemoryStream(buffer))
            {
                var bitReader = new BitReader(stream);

                while (!bitReader.EndOfStream)
                {
                    var intervalLength = 6 + (bitReader.Read(2) << 3);
                    var interval = bitReader.Read(intervalLength);
                    ticksElapsed += (int)interval;
                    var playerIndex = (int)bitReader.Read(5);
                    Player player;
                    if (playerIndex < 0x10)
                    {
                        player = replay.GetPlayerById(playerIndex);
                    }
                    else
                    {
                        player = Player.Global;
                    }

                    var eventType = bitReader.Read(7);
                    IGameEvent gameEvent;
                    switch (eventType)
                    {
                        case 0x05: // Game start
                            gameEvent = new GameStartEvent();
                            break;
                        case 0x0b:
                        case 0x0c: // Join game
                            gameEvent = new PlayerJoinEvent(bitReader, replay, playerIndex);
                            break;
                        case 0x19: // Leave game
                            gameEvent = new PlayerLeftEvent(player);
                            break;
                        case 0x1b: // Ability
                            gameEvent = new AbilityEvent(bitReader, replay, player);
                            break;
                        case 0x1c: // Selection
                            gameEvent = new SelectionEvent(bitReader, replay, player);
                            break;
                        case 0x1d: // Control groups
                            gameEvent = new HotkeyEvent(bitReader, replay, player);
                            break;
                        case 0x1f: // Send resources
                            gameEvent = new SendResourcesEvent(bitReader, replay);
                            break;
                        case 0x23: // ??
                            gameEvent = new GameEventBase();
                            gameEvent.EventType = GameEventType.Inactive;
                            bitReader.Read(8);
                            break;
                        case 0x26: // ??
                            gameEvent = new GameEventBase();
                            gameEvent.EventType = GameEventType.Inactive;
                            bitReader.Read(32);
                            bitReader.Read(32);
                            break;
                        case 0x31: // Camera
                            gameEvent = new CameraEvent(bitReader, replay);
                            break;
                        case 0x46: // Request resources
                            gameEvent = new RequestResourcesEvent(bitReader, replay);
                            break;
                        default: // debug
                            throw new InvalidOperationException(String.Format(
                                "Unknown event type {0:x} at {1:x} in replay.game.events",
                                eventType, bitReader.Cursor));
                            return null; // Irrecoverable
                    }

                    gameEvent.Player = player;
                    gameEvent.Time = Timestamp.Create(ticksElapsed);
                    events.Add(gameEvent);

                    bitReader.AlignToByte();
                }
            }

            replay.GameLength = Timestamp.Create(ticksElapsed).TimeSpan;

            return events;
        }

        private static void TrackLeavingPlayer(List<Player> remainingPlayers, Player player)
        {
            if (remainingPlayers.Contains(player))
            {
                remainingPlayers.Remove(player);
            }
        }
    }
}
