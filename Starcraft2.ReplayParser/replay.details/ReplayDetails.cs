// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReplayDetails.cs" company="SC2ReplayParser">
//   Copyright © 2011 All Rights Reserved
// </copyright>
// <summary>
//   Parses the replay.details file in the MPQ Archive
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;

namespace Starcraft2.ReplayParser
{
    using System;
    using System.IO;
    using System.Text;

    /// <summary>
    /// Parses the replay.details file in the MPQ Archive
    /// </summary>
    public class ReplayDetails
    {
        #region Public Methods

        /// <summary> Parses the replay.details file, applying it to a Replay object. </summary>
        /// <param name="replay"> The replay object to apply the parsed information to. </param>
        /// <param name="buffer"> The buffer containing the replay.details file. </param>
        public static void Parse(Replay replay, byte[] buffer)
        {
            using (var stream = new MemoryStream(buffer, false))
            {
                Parse(replay, stream);

                stream.Close();
            }
        }

        /// <summary> Parses the replay.details file, applying it to a Replay object. </summary>
        /// <param name="replay"> The replay object to apply the parsed information to. </param>
        /// <param name="stream"> The stream containing the replay.details file. </param>
        public static void Parse(Replay replay, Stream stream)
        {
            using (var reader = new BinaryReader(stream))
            {
                reader.ReadBytes(6);
                var playerCount = reader.ReadByte() >> 1;

                var players = new Player[playerCount];

                // Parsing Player Info
                for (int i = 0; i < playerCount; i++)
                {
                    var parsedPlayer = PlayerDetails.Parse(reader);

                    // The references between both of these classes are the same on purpose.
                    // We want updates to one to propogate to the other.
                    players[i] = parsedPlayer;
                    replay.ClientList[i + 1] = parsedPlayer;

                    if (replay.ReplayBuild >= 25180)
                    {
                        reader.ReadBytes(5);
                    }
                }

                replay.Players = players;

                var mapNameLength = KeyValueStruct.Parse(reader).Value;

                var mapBytes = reader.ReadBytes(mapNameLength);

                replay.Map = Encoding.UTF8.GetString(mapBytes);

                var stringLength = KeyValueStruct.Parse(reader).Value;

                // This is typically an empty string, no need to decode.
                var unknownString = reader.ReadBytes(stringLength);

                reader.ReadBytes(3);

                var mapPreviewNameLength = KeyValueStruct.Parse(reader).Value;
                var mapPreviewNameBytes = reader.ReadBytes(mapPreviewNameLength);

                replay.MapPreviewName = Encoding.UTF8.GetString(mapPreviewNameBytes);

                reader.ReadBytes(3);

                var saveTime = KeyValueLongStruct.Parse(reader).Value;
                var saveTimeZone = KeyValueLongStruct.Parse(reader).Value;
                var time = DateTime.FromFileTime(saveTime);

                // Subtract the timezone to get the appropriate UTC time.
                time = time.Subtract(new TimeSpan(saveTimeZone));

                // We create a new timestamp so we can properly set this as UTC time.
                replay.Timestamp = new DateTime(time.Ticks, DateTimeKind.Utc);

                // don't know what the next 14 bytes are for, so we skip them
                reader.ReadBytes(14);

                var resources = new List<ResourceInfo>();
                reader.ReadBytes(2); // there are 2 bytes before each "s2ma" string
                var s2ma = Encoding.UTF8.GetString(reader.ReadBytes(4));

                while (s2ma == "s2ma")
                {
                    reader.ReadBytes(2); // 0x00, 0x00

                    resources.Add(
                        new ResourceInfo
                            {
                                Gateway = Encoding.UTF8.GetString(reader.ReadBytes(2)),
                                Hash = reader.ReadBytes(32),
                            });

                    reader.ReadBytes(2);
                    s2ma = Encoding.UTF8.GetString(reader.ReadBytes(4));
                }

                var map = resources.Last();
                replay.MapGateway = map.Gateway;
                replay.MapHash = map.Hash;

                reader.Close();
            }
        }

        #endregion

        private class ResourceInfo {
            public string Gateway { get; set; }
            public byte[] Hash { get; set; }
        }
    }
}