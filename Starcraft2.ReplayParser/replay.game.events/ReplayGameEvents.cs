namespace Starcraft2.ReplayParser
{
    using System.Diagnostics;
    using System.IO;
    using System;
    using System.Collections.Generic;

    using Streams;
    using Version;

    public class ReplayGameEvents
    {
        public static List<IGameEvent> Parse(Replay replay, byte[] buffer)
        {
            // The GameEvent file changes significantly after 16561.
            // This is sometime around the first patch after release. Since
            // parsing replays this old should be extremely rare, I don't believe
            // its worth the effort to try to support both. If it is, it should be
            // done in another method.
            //
            // Still a bitstream, but stuff's moved around and event codes are different.
            if (replay.ReplayBuild < 16561)
            {
                throw new NotSupportedException(
                    "Replay builds under 16561 are not supported for parsing GameEvent log.");
            }

            // Initialize Ability and Unit data.
            var effectiveBuild = BuildData.GetInstance().GetEffectiveBuild(replay.ReplayBuild);
            if (effectiveBuild == 0)
            {
                throw new NotSupportedException(
                    String.Format("Replay build {0} is not supported by the current event database", replay.ReplayBuild));
            }
            var abilityData = new AbilityData(effectiveBuild);
            var unitData = new UnitData(effectiveBuild);

            var events = new List<IGameEvent>();

            // Keep a reference to know the game length.
            var ticksElapsed = 0;

            using (var stream = new MemoryStream(buffer))
            {
                var bitReader = new BitReader(stream);

                var playersGone = new bool[0x10];

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
                            playersGone[playerIndex] = true;
                            DetectWinners(playersGone, replay);
                            break;
                        case 0x1b: // Ability
                            gameEvent = new AbilityEvent(bitReader, replay, player, abilityData, unitData);
                            break;
                        case 0x1c: // Selection
                            gameEvent = new SelectionEvent(bitReader, replay, player, unitData);
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
                        case 0x27: // Target critter - special
                            gameEvent = new GameEventBase();
                            gameEvent.EventType = GameEventType.Selection;
                            var unitId = bitReader.Read(32);
                            break;
                        case 0x31: // Camera
                            gameEvent = new CameraEvent(bitReader, replay);
                            break;
                        case 0x37: // UI Event
                            gameEvent = new GameEventBase();
                            gameEvent.EventType = GameEventType.Other;
                            bitReader.Read(32);
                            bitReader.Read(32);
                            break;
                        case 0x38: // weird sync event
                            {
                                gameEvent = new GameEventBase();
                                gameEvent.EventType = GameEventType.Other;
                                for (var j = 0; j < 2; j++)
                                {
                                    var length = bitReader.Read(8);
                                    for (var i = 0; i < length; i++)
                                    {
                                        bitReader.Read(32);
                                    }
                                }
                                break;
                            }
                        case 0x3c: // ???
                            gameEvent = new GameEventBase();
                            gameEvent.EventType = GameEventType.Inactive;
                            bitReader.Read(16);
                            break;
                        case 0x46: // Request resources
                            gameEvent = new RequestResourcesEvent(bitReader, replay);
                            break;
                        case 0x47: // ?? -- associated with send minerals
                            gameEvent = new GameEventBase();
                            gameEvent.EventType = GameEventType.Inactive;
                            bitReader.Read(32);
                            break;
                        case 0x4C: // ?? -- seen with spectator
                            bitReader.Read(4);
                            gameEvent = new GameEventBase();
                            gameEvent.EventType = GameEventType.Inactive;
                            break;
                        case 0x59: // ?? -- sync flags maybe?
                            bitReader.Read(32);
                            gameEvent = new GameEventBase();
                            gameEvent.EventType = GameEventType.Inactive;
                            break;
                        default: // debug
                            throw new InvalidOperationException(String.Format(
                                "Unknown event type {0:x} at {1:x} in replay.game.events",
                                eventType, bitReader.Cursor));
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

        private static void DetectWinners(bool[] playersGone, Replay replay)
        {
            var teamsStillActive = new bool[0x10];
            for (var i = 0; i < playersGone.Length; i++)
            {
                var player = replay.GetPlayerById(i);
                if (player != null && // player exists
                    player.Team != 0 && // player is not neutral
                    // -- Technically player team is 16 for spectators I think, but not defined here => 0
                    player.PlayerType != PlayerType.Spectator && // player is playing
                    playersGone[i] == false) // player is still in-game
                {
                    teamsStillActive[player.Team] = true;
                }
            }

            var winCandidate = 0;
            for (var i = 1; i < 0x10; i++)
            {
                if (teamsStillActive[i] && winCandidate == 0)
                {
                    winCandidate = i;
                }
                else if (teamsStillActive[i]) // .Count(n=>n) > 0
                {
                    winCandidate = -1;
                }
            }

            if (winCandidate > 0)
            {
                foreach (var player in replay.Players)
                {
                    if (player != null && player.Team == winCandidate)
                    {
                        player.IsWinner = true;
                    }
                }
            }
        }
    }
}
