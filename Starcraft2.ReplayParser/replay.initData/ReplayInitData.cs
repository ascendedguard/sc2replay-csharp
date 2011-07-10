// -----------------------------------------------------------------------
// <copyright file="ReplayInitData.cs" company="Ascend">
//   Copyright © 2011 All Rights Reserved
// </copyright>
// -----------------------------------------------------------------------

namespace Starcraft2.ReplayParser
{
    using System.IO;
    using System.Text;

    /// <summary> Parses the replay.Initdata file in the replay file. </summary>
    public class ReplayInitData
    {
        public static void Parse(Replay replay, byte[] buffer)
        {
            using (var stream = new MemoryStream(buffer))
            {
                using (var reader = new BinaryReader(stream))
                {
                    var i = reader.ReadByte();

                    var playerList = new string[i];
                    for (int j = 0; j < i; j++)
                    {
                        var str = ReadString(reader);

                        playerList[j] = str;
                        reader.ReadBytes(5);
                    }

                    if (positionAfter(reader, new byte[] { 115, 50, 109, 97 }))
                    {
                        reader.ReadBytes(2);
                        var gatewayStr = reader.ReadBytes(2);

                        var gateway = Encoding.UTF8.GetString(gatewayStr);
                        replay.Gateway = gateway;
                    }
                    else
                    {
                        replay.GameType = GameType.SinglePlayer;
                    }
                }                
            }
        }

        private static string ReadString(BinaryReader reader)
        {
            var numBytes = reader.ReadByte();
            var strArr = reader.ReadBytes(numBytes);

            return Encoding.UTF8.GetString(strArr);
        }

        protected static bool positionAfter(BinaryReader reader, byte[] paramArrayOfByte)
        {
            var i = reader.BaseStream.Position;

            var stream = reader.BaseStream;

            var k = stream.Length - paramArrayOfByte.Length;

            var arr = reader.ReadBytes((int)k);

            int j = 0;
            for (j = 0; j < k; j++)
            {
                int l = 0;
                for (l = 0; l < paramArrayOfByte.Length; l++)
                {
                    if (arr[j + l] != paramArrayOfByte[l])
                    {
                        break;
                    }
                }

                if (l == paramArrayOfByte.Length)
                {
                    break;
                }
            }

            if (j < k)
            {
                reader.BaseStream.Position = i + j + paramArrayOfByte.Length;
                return true;
            }

            reader.BaseStream.Position = i;
            return false;            
        }
    }
}
