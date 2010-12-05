using System;
using System.IO;

namespace Starcraft2.ReplayParser
{
    public class ReplayAttributeEvents
    {
        public ReplayAttribute[] Attributes { get; set; }

        public static ReplayAttributeEvents Parse(string filePath)
        {
            // All of the values in this format are in little-endian, and the arrays must first be reversed.

            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = new BinaryReader(fileStream))
                {
                    reader.ReadBytes(4); // File Header, usually 00 00 00 00

                    var numAttributes = BitConverter.ToInt32(reader.ReadBytes(4), 0);

                    var attributes = new ReplayAttribute[numAttributes];
                    
                    for(int i = 0; i < numAttributes; i++)
                    {
                        attributes[i] = ReplayAttribute.Parse(reader);    
                    }

                    var rae = new ReplayAttributeEvents {Attributes = attributes};
                    return rae;
                }
            }
        }
    }
}
