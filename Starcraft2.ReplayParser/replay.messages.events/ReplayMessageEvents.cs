using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Starcraft2.ReplayParser
{
    public class ReplayMessageEvents
    {
        /// <summary>
        /// Parses the Replay.Messages.Events file.
        /// </summary>
        /// <param name="buffer">Buffer containing the contents of the replay.messages.events file.</param>
        /// <returns>A list of chat messages parsed from the buffer.</returns>
        public static List<ChatMessage> Parse(byte[] buffer)
        {
            var messages = new List<ChatMessage>();

            using (var stream = new MemoryStream(buffer))
            {
                using (var reader = new BinaryReader(stream))
                {
                    int totalTime = 0;

                    while (reader.BaseStream.Position < reader.BaseStream.Length) // While not EOF
                    {
                        var message = new ChatMessage();

                        var time = ParseTimestamp(reader);
                        message.PlayerId = reader.ReadByte();

                        totalTime += time;
                        var opCode = reader.ReadByte();

                        if (opCode == 0x80)
                        {
                            reader.ReadBytes(4);
                        }
                        else if (opCode == 0x83)
                        {
                            reader.ReadBytes(8);
                        }
                        else if ((opCode & 0x80) == 0)
                        {
                            message.MessageTarget = opCode & 3;
                            var length = reader.ReadByte();

                            if ((opCode & 8) == 8) length += 64;
                            if ((opCode & 16) == 16) length += 128;

                            byte[] msg = reader.ReadBytes(length);

                            message.Message = Encoding.ASCII.GetString(msg);
                        }
                        
                        if (message.Message != null)
                        {
                            message.Timestamp = new TimeSpan(0,0,(int)Math.Ceiling(totalTime / 16.0));
                            messages.Add(message);                            
                        }
                    }
                }
            }

            return messages;
        }

        private static int ParseTimestamp(BinaryReader reader)
        {
            byte one = reader.ReadByte();
            if ((one & 3) > 0)
            {
                int two = reader.ReadByte();
                two = (short)(((one >> 2) << 8) | two);

                if ((one & 3) >= 2)
                {
                    var tmp = reader.ReadByte();
                    two = ((two << 8) | tmp);

                    if ((one & 3) == 3)
                    {
                        tmp = reader.ReadByte();
                        two = ((two << 8) | tmp);
                    }
                }

                return two;
            }

            return (one >> 2);
        }
    }
}
