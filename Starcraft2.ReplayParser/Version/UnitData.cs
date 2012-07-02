// -----------------------------------------------------------------------
// <copyright file="UnitData.cs" company="Microsoft">
// TODO: Update copyright text.
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

        SubgroupData subgroupData;
    }
}
