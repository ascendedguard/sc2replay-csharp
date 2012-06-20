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
        public SelectionEvent(BitReader bitReader, Replay replay, Player player)
        {
            // Parse select event and update player wireframe accordingly
            SelectionFlags = (int)bitReader.Read(4);
            player.WireframeSubgroup = SubgroupIndex = (int)bitReader.Read(8);

            var updateFlags = (int)bitReader.Read(2);

        }

        /// <summary>
        /// Reads the 8 {16, 8, 8}, 8 {32} struct; the result is in AddedUnits / AddedUnitTypes.
        /// </summary>
        void HandleUnitArrays(BitReader bitReader, Replay replay)
        {
            var typesLength = (int)bitReader.Read(8);
            AddedUnitTypes = new Dictionary<UnitType, int>(typesLength);

            // Guarantee order is maintained
            var subgroups = new List<KeyValuePair<UnitType, int>>(typesLength);

            for (int i = 0; i < typesLength; i++)
            {
                var unitTypeId = (int)bitReader.Read(16);
                var unitType = UnitData.GetUnitType(unitTypeId, replay.ReplayBuild);
                
                var unknown = bitReader.Read(8);
                if (unknown != 1) // Debug:  No idea
                {
                    var zero = 0d;
                }
                var unitCountType = (int)bitReader.Read(8);

                AddedUnitTypes.Add(unitType, unitCountType);
                subgroups.Add(new KeyValuePair<UnitType, int>(unitType, unitCountType));
            }

            var idsLength = (int)bitReader.Read(8);
            AddedUnits = AddedUnits ?? new List<Unit>(idsLength);
            if (idsLength == 0) return;

            var subgroupsEnumerator = subgroups.GetEnumerator();
            var currentSubgroupIndex = subgroupsEnumerator.Current.Value;
            for (int i = 0; i < idsLength; i++)
            {
                var unitId = (int)bitReader.Read(32);
                var unit = replay.GetUnitById(unitId);
                var unitType = subgroupsEnumerator.Current.Key;
                if (unit == null)
                {
                    unit = new Unit(unitId, unitType);
                    replay.GameUnits.Add(unitId, unit);
                }
                else
                {
                    unit.UpdateType(unitType);
                }
                AddedUnits.Add(unit);

                if (--currentSubgroupIndex == 0)
                {
                    if (subgroupsEnumerator.MoveNext())
                    {
                        currentSubgroupIndex = subgroupsEnumerator.Current.Value;
                    }
                }
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
