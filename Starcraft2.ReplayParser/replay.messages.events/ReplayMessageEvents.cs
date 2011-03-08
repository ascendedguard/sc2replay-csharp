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
                            message.MessageTarget = (ChatMessageTarget)(opCode & 3);
                            var length = reader.ReadByte();

                            if ((opCode & 8) == 8) length += 64;
                            if ((opCode & 16) == 16) length += 128;

                            byte[] msg = reader.ReadBytes(length);
                            
                            message.Message = Encoding.ASCII.GetString(msg);
                        }
                        
                        if (message.Message != null)
                        {
                            message.Timestamp = new TimeSpan(0,0,(int)Math.Round(totalTime / 16.0));
                            messages.Add(message);                            
                        }
                    }
                }
            }

            return messages;
        }

        public static byte[] GenerateChatMessage(byte[] buffer, string message, int playerId, int seconds)
        {
            if (message.Length > 64)
            {
                throw new NotSupportedException("This call does not support strings longer than 64 characters yet.");
            }

            int targetValue = seconds*16;

            var completeFile = new List<byte>();

            bool hasBeenWritten = false;
            bool adjustTimestamps = false;

            using (var stream = new MemoryStream(buffer))
            {
                using (var reader = new BinaryReader(stream))
                {
                    int totalTime = 0;
                    int shiftValue = 0;

                    while (reader.BaseStream.Position < reader.BaseStream.Length) // While not EOF
                    {
                        int timestamp = ParseTimestamp(reader);

                        totalTime += timestamp;

                        if (hasBeenWritten == false && totalTime > targetValue)
                        {
                            var orgValue = totalTime - timestamp;

                            shiftValue = targetValue - orgValue;

                            var shiftTimestamp = CreateTimestamp(shiftValue);

                            var bytes = new List<byte>();

                            bytes.AddRange(shiftTimestamp);
                            bytes.Add((byte)playerId); //playerid
                            bytes.Add(0); //opcode
                            bytes.Add((byte) message.Length);
                            bytes.AddRange(Encoding.ASCII.GetBytes(message));

                            completeFile.AddRange(bytes);
                            hasBeenWritten = true;
                            adjustTimestamps = true;
                        }

                        if (adjustTimestamps)
                        {
                            completeFile.AddRange(CreateTimestamp(timestamp - shiftValue));
                        }
                        else
                        {
                            completeFile.AddRange(CreateTimestamp(timestamp));                            
                        }

                        completeFile.Add(reader.ReadByte()); // PlayerID

                        var opCode = reader.ReadByte();

                        completeFile.Add(opCode);

                        if (opCode == 0x80)
                        {
                            completeFile.AddRange(reader.ReadBytes(4));
                        }
                        else if (opCode == 0x83)
                        {
                            completeFile.AddRange(reader.ReadBytes(8));
                        }
                        else if ((opCode & 0x80) == 0)
                        {
                            var length = reader.ReadByte();

                            completeFile.Add(length);

                            if ((opCode & 8) == 8) length += 64;
                            if ((opCode & 16) == 16) length += 128;

                            completeFile.AddRange(reader.ReadBytes(length));
                        }
                    }
                }
            }

            return completeFile.ToArray();
        }

        public static byte[] CreateTimestamp(int value)
        {
            int bytesNeeded = 0;

            if (value > Math.Pow(2, 30))
                throw new ArgumentOutOfRangeException("Timestamp value cannot be greater than 2^30");

            if (value > Math.Pow(2, 22)) bytesNeeded = 3;
            if (value > Math.Pow(2, 14)) bytesNeeded = 2;
            if (value > Math.Pow(2, 6)) bytesNeeded = 1;

            var bytes = BitConverter.GetBytes(value);//.Where((i) => i > 0).ToArray();
            bytes[bytesNeeded] = (byte)((bytes[bytesNeeded] << 2) | bytesNeeded);
            
            //bytes[0] = (byte)((bytes[0] << 2) | bytesNeeded);

            return bytes.Where((i) => i > 0).Reverse().ToArray();
        }

        public static int ParseTimestamp(BinaryReader reader)
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
