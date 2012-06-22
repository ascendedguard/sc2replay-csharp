// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReplayDetails.cs" company="SC2ReplayParser">
//   Copyright © 2011 All Rights Reserved
// </copyright>
// <summary>
//   Parses the replay.details file in the MPQ Archive
// </summary>
// --------------------------------------------------------------------------------------------------------------------

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

                // Parsing Player Info
                for (int i = 0; i < playerCount; i++)
                {
                    replay.Players[i + 1] = PlayerDetails.Parse(reader);
                }

                var mapNameLength = KeyValueStruct.Parse(reader).Value;

                var mapBytes = reader.ReadBytes(mapNameLength);

                replay.Map = Encoding.UTF8.GetString(mapBytes);

                var stringLength = KeyValueStruct.Parse(reader).Value;

                // This is typically an empty string, no need to decode.
                var unknownString = reader.ReadBytes(stringLength);

                reader.ReadBytes(3);

                var mapPreviewNameLength = KeyValueStruct.Parse(reader).Value;
                var mapPreviewNameBytes = reader.ReadBytes(mapPreviewNameLength);

                // What have I learned:
                // While I can get the name of the map preview file, apparently MPQLib.dll will not
                // Support exporting the file since its not in the file list. I tried, and it threw an error...
                // Maybe my buffer wasn't big enough, but it was some temporary file error, so I don't think so.
                var mapPreviewName = Encoding.UTF8.GetString(mapPreviewNameBytes);

                reader.ReadBytes(3);

                var saveTime = KeyValueLongStruct.Parse(reader).Value;
                var saveTimeZone = KeyValueLongStruct.Parse(reader).Value;
                var time = DateTime.FromFileTime(saveTime);

                // Subtract the timezone to get the appropriate UTC time.
                time = time.Subtract(new TimeSpan(saveTimeZone));

                // We create a new timestamp so we can properly set this as UTC time.
                replay.Timestamp = new DateTime(time.Ticks, DateTimeKind.Utc);

                reader.Close();
            }
        }

        #endregion
    }
}