// -----------------------------------------------------------------------
// <copyright file="AbilityData.cs">
// Copyright 2012 Robert Nix, Will Eddins
// </copyright>
// -----------------------------------------------------------------------

namespace Starcraft2.ReplayParser.Version
{
    using System.Collections.Generic;

    /// <summary>
    /// Provides version dispatch for Ability types
    /// </summary>
    public class AbilityData : DataFile
    {
        public AbilityData(int build)
            : base("abil" + build + ".dat")
        {
        }

        public AbilityType GetAbilityType(int typeId, int buttonId)
        {
            var index = (typeId << 5 | buttonId) * 2;
            if (index >= Data.Length)
            {
                return AbilityType.Unknown;
            }
            return (AbilityType)(Data[index + 1] << 8 | Data[index]);
        }
    }
}
