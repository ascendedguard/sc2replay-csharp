// -----------------------------------------------------------------------
// <copyright file="DataFile.cs">
// Copyright 2012 Robert Nix, Will Eddins
// </copyright>
// -----------------------------------------------------------------------

namespace Starcraft2.ReplayParser.Version
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Collections.Generic;

    /// <summary>
    /// Provides header information and binary contents for .dat files
    /// </summary>
    public class DataFile
    {
        public DataFile(string fileName)
        {
            Stream fileStream;
            if (File.Exists(fileName)) // File override
            {
                fileStream = (Stream)File.Open(fileName, FileMode.Open, FileAccess.Read);
            }
            else
            {
                var currentAssembly = Assembly.GetExecutingAssembly();
                var embeddedFilename = this.GetType().Namespace + '.' + fileName;
                fileStream = currentAssembly.GetManifestResourceStream(embeddedFilename);
            }

            if (fileStream == null)
            {
                throw new FileNotFoundException(String.Format("Could not find data file: {0}", fileName)); 
            }

            using (fileStream)
            {
                // Read header
                var buf = new byte[16];
                fileStream.Read(buf, 0, 16);
                MagicWord = BitConverter.ToInt32(buf, 0);
                TypeWord = BitConverter.ToInt32(buf, 4);
                BuildNumber = BitConverter.ToInt32(buf, 8);
                DataLength = BitConverter.ToInt32(buf, 12);

                // Read file contents
                Data = new byte[DataLength];
                fileStream.Read(Data, 0, DataLength);
            }
        }

        public int MagicWord { get; private set; }

        public int TypeWord { get; private set; }

        public int BuildNumber { get; private set; }

        public int DataLength { get; private set; }

        public byte[] Data { get; private set; }
    }
}
