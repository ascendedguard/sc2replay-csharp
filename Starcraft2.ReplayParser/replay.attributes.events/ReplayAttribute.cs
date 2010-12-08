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

        public static ReplayAttribute Parse(byte[] buffer, int offset)
        {
            var attribute = new ReplayAttribute
            {
                Header = BitConverter.ToInt32(buffer, offset),
                AttributeId = BitConverter.ToInt32(buffer, offset + 4),
                PlayerId = buffer[offset + 8],
                Value = new byte[4],
            };

            Array.Copy(buffer, offset + 9, attribute.Value, 0, 4);

            return attribute;
        }
    }
}
