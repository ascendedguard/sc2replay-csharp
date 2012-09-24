// -----------------------------------------------------------------------
// <copyright file="UnitData.cs">
// Copyright 2012 Robert Nix, Will Eddins
// </copyright>
// -----------------------------------------------------------------------

namespace Starcraft2.ReplayParser.Version
{
    using System.Collections.Generic;

    /// <summary>
    /// Provides version dispatch for unit type data
    /// </summary>
    public class UnitData : DataFile
    {
        public UnitData(int build)
            : base("unit" + build + ".dat")
        {
            subgroupData = new SubgroupData();
        }

        public UnitType GetUnitType(int typeId)
        {
            var index = typeId * 2;
            if (index >= Data.Length)
            {
                return UnitType.Unknown;
            }
            return (UnitType)(Data[index + 1] << 8 | Data[index]);
        }

        public int GetUnitSubgroupPriority(UnitType unitType)
        {
            var priority = 0;
            subgroupData.Values.TryGetValue(unitType, out priority);
            return priority;
        }

        public UnitType GetHallucination(UnitType t)
        {
            switch (t)
            {
                case UnitType.Probe:
                    return UnitType.ProbeHallucination;
                case UnitType.Zealot:
                    return UnitType.ZealotHallucination;
                case UnitType.Stalker:
                    return UnitType.StalkerHallucination;
                case UnitType.HighTemplar:
                    return UnitType.HighTemplarHallucination;
                case UnitType.Archon:
                    return UnitType.ArchonHallucination;
                case UnitType.Immortal:
                    return UnitType.ImmortalHallucination;
                case UnitType.WarpPrism:
                    return UnitType.WarpPrismHallucination;
                case UnitType.WarpPrismPhasing:
                    return UnitType.WarpPrismPhasingHallucination;
                case UnitType.Colossus:
                    return UnitType.ColossusHallucination;
                case UnitType.Phoenix:
                    return UnitType.PhoenixHallucination;
                case UnitType.VoidRay:
                    return UnitType.VoidRayHallucination;
                default:
                    return UnitType.Unknown; // Could throw, but it doesn't break the parser.
            }
        }

        SubgroupData subgroupData;
    }
}
