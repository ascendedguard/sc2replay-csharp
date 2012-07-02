// -----------------------------------------------------------------------
// <copyright file="SubgroupData.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Starcraft2.ReplayParser.Version
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Provides information about unit subgroup priorities
    /// </summary>
    public class SubgroupData : DataFile
    {
        public SubgroupData()
            : base("subgroups.dat")
        {

            Values = new Dictionary<UnitType, int>(Data.Length);

            for (var i = 0; i < Data.Length; i++)
            {
                Values.Add((UnitType)i, Data[i]);
            }
        }

        public Dictionary<UnitType, int> Values;
    }
}
