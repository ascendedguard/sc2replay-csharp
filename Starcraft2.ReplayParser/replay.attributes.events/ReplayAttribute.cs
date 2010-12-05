using System;
using System.IO;

namespace Starcraft2.ReplayParser
{
    public class ReplayAttribute
    {
        public int Header { get; set; }
        public int AttributeId { get; set; }
        public int PlayerId { get; set; }
        public byte[] Value { get; set; }

        /// <summary>
        /// Parses a single ReplayAttribute at the current position of the reader
        /// and advances the reader forward.
        /// </summary>
        /// <param name="reader">BinaryReader at the position to read the object</param>
        /// <returns>A ReplayAttribute containing the attribute found.</returns>
        public static ReplayAttribute Parse(BinaryReader reader)
        {
            var attribute = new ReplayAttribute
            {
                Header = BitConverter.ToInt32(reader.ReadBytes(4), 0),
                AttributeId = BitConverter.ToInt32(reader.ReadBytes(4), 0),
                PlayerId = reader.ReadByte(),
                Value = reader.ReadBytes(4)
            };

            return attribute;
        }
    }
}
