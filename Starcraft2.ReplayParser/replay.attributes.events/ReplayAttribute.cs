// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReplayAttribute.cs" company="Ascend">
//   Copyright © 2011 All Rights Reserved
// </copyright>
// <summary>
//   Defines the ReplayAttribute type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Starcraft2.ReplayParser
{
    using System;

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

                // Offset the PlayerID so it matches our array indices.
                PlayerId = buffer[offset + 8] - 1,
                Value = new byte[4],
            };

            Array.Copy(buffer, offset + 9, attribute.Value, 0, 4);

            return attribute;
        }
    }
}
