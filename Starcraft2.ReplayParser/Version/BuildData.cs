// -----------------------------------------------------------------------
// <copyright file="BuildData.cs">
// Copyright 2012 Robert Nix, Will Eddins
// </copyright>
// -----------------------------------------------------------------------

namespace Starcraft2.ReplayParser.Version
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Provides build version information
    /// </summary>
    public class BuildData : DataFile
    {
        BuildData()
            : base("builds.dat")
        {
            buildInformation = new Dictionary<int, int>();

            // Parse to a dictionary.  We don't care about memory usage, so though we
            // could add a method to deallocate the underlying array, there's no need.
            for (var i = 0; i < DataLength; i+=4)
            {
                buildInformation.Add(Data[i] | Data[i + 1] << 8, Data[i + 2] | Data[i + 3] << 8);
            }
        }

        /// <summary>
        /// Returns the effective build number for a given build number.
        /// </summary>
        public int GetEffectiveBuild(int build)
        {
            int result = 0;
            if (!buildInformation.TryGetValue(build, out result))
            {
                // Let's find the closest match, but if it's a future
                // version, we'll return 0.
                int bestDelta = Int32.MaxValue;
                foreach (var pair in buildInformation)
                {
                    var delta = pair.Key - build;
                    if (delta < bestDelta && delta > 0)
                    {
                        bestDelta = delta;
                        result = pair.Value;
                    }
                }
            }
            return result;
        }

        Dictionary<int, int> buildInformation;

        /// <summary>
        /// Singleton
        /// </summary>
        private static BuildData singleton;

        /// <summary>
        /// Gets an instance of the singleton BuildData
        /// </summary>
        public static BuildData GetInstance()
        {
            if (singleton == null)
            {
                singleton = new BuildData();
            }

            return singleton;
        }
    }
}
