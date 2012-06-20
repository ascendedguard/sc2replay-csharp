// -----------------------------------------------------------------------
// <copyright file="SelectionEvent.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Starcraft2.ReplayParser
{
    using System.Collections.Generic;

    using Streams;
    using Version;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class SelectionEvent : GameEventBase
    {
        public SelectionEvent(BitReader bitReader, Replay replay)
        {
            // ...
        }

        void HandleUnitArrays(BitReader bitReader, Replay replay)
        {
            var typesLength = (int)bitReader.Read(8);
            for (int i = 0; i < typesLength; i++)
            {
                var unitTypeId = (int)bitReader.Read(16);
                var unitType = UnitData.GetUnitType(unitTypeId, replay.ReplayBuild);
                
                var unknown = bitReader.Read(8);
                if (unknown != 1) // Debug
                {
                    var zero = 0d;
                }
                var unitCountType = (int)bitReader.Read(8);

                AddedUnitTypes.Add(unitType, unitCountType);
            }

            var idsLength = (int)bitReader.Read(8);
            for (int i = 0; i < idsLength; i++)
            {
                var unitId = (int)bitReader.Read(32);
                var unit = replay.GetUnitById(unitId);
                if (unit == null)
                {
                    var unitType = UnitType.Unknown;
                    if (AddedUnitTypes.Count == 1)
                    {
                        unitType = AddedUnitTypes.Keys.GetEnumerator().Current;
                    }
                    unit = new Unit(unitId, unitType);
                }
                AddedUnits.Add(unit);
            }
        }

        /// <summary>  4 bit flags, unknown purpose </summary>
        public int SelectionFlags { get; private set; }

        /// <summary>
        /// The index of the subset of selected units in the wireframe,
        /// changed by pressing Tab
        /// </summary>
        public int SubgroupIndex { get; private set; }

        /// <summary> True if the selection was cleared by the event </summary>
        public bool ClearSelection { get; private set; }

        public List<Unit> AddedUnits { get; private set; }

        public List<Unit> RemovedUnits { get; private set; }

        /// <summary> A map of the added unit types to the corresponding unit counts </summary>
        public Dictionary<UnitType, int> AddedUnitTypes { get; private set; }
    }
}
