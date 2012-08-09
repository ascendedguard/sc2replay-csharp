// -----------------------------------------------------------------------
// <copyright file="SelectionEvent.cs">
// Copyright 2012 Robert Nix, Will Eddins
// </copyright>
// -----------------------------------------------------------------------

namespace Starcraft2.ReplayParser
{
    using System.Collections.Generic;
    using System.Linq;

    using Streams;
    using Version;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class SelectionEvent : GameEventBase
    {
        public SelectionEvent(BitReader bitReader, Replay replay, Player player, UnitData data)
        {
            int wireframeLength = 8;
            if (replay.ReplayBuild >= 22612)
            {
                wireframeLength = 9; // Maximum selection size has been increased to 500, up from 255.
            }

            // Parse select event and update player wireframe accordingly
            WireframeIndex = (int)bitReader.Read(4);
            player.WireframeSubgroup = SubgroupIndex = (int)bitReader.Read(wireframeLength);

            if (WireframeIndex == 10)
            {
                this.EventType = GameEventType.Selection;
            }
            else // This is a control group update, likely from a CAbilMorph
            {
                this.EventType = GameEventType.Inactive;
            }
            
            List<Unit> affectedWireframe;
            if (WireframeIndex == 10)
            {
                affectedWireframe = player.Wireframe;
            }
            else
            {
                affectedWireframe = player.Hotkeys[WireframeIndex];
            }

            RemovedUnits = new List<Unit>();

            var updateFlags = (int)bitReader.Read(2);

            ClearSelection = false;

            if (updateFlags == 1)
            {
                var numBits = (int)bitReader.Read(wireframeLength);
                var unitsRemoved = new bool[numBits];

                var wireframeIndex = 0;
                while (numBits >= 8)
                {
                    numBits -= 8;
                    var flags = bitReader.Read(8);
                    for (int i = 0; i < 8; i++)
                    {
                        unitsRemoved[wireframeIndex + i] = (flags & (1 << i)) != 0;
                    }
                    wireframeIndex += 8;
                }
                if (numBits != 0)
                {
                    var flags = bitReader.Read(numBits);
                    for (int i = 0; i < numBits; i++)
                    {
                        unitsRemoved[wireframeIndex + i] = (flags & (1 << i)) != 0;
                    }
                    wireframeIndex += numBits;
                }

                for (int i = 0; i < wireframeIndex; i++)
                {
                    if (unitsRemoved[i])
                    {
                        RemovedUnits.Add(affectedWireframe[i]);
                    }
                }
            }
            else if (updateFlags == 2)
            {
                var indexArrayLength = (int)bitReader.Read(wireframeLength);
                if (indexArrayLength > 0)
                {
                    for (int i = 0; i < indexArrayLength; i++)
                    {
                        RemovedUnits.Add(affectedWireframe[(int)bitReader.Read(wireframeLength)]);
                    }
                }
            }
            else if (updateFlags == 3)
            {
                var indexArrayLength = (int)bitReader.Read(wireframeLength);
                if (indexArrayLength > 0)
                {
                    AddedUnits = new List<Unit>(indexArrayLength);
                    for (int i = 0; i < indexArrayLength; i++)
                    {
                        AddedUnits.Add(affectedWireframe[(int)bitReader.Read(wireframeLength)]);
                    }
                }

                ClearSelection = true;
            }

            // Build removed unit types
            RemovedUnitTypes = new Dictionary<UnitType, int>();
            foreach (var unit in RemovedUnits)
            {
                if (!RemovedUnitTypes.ContainsKey(unit.Type))
                {
                    RemovedUnitTypes.Add(unit.Type, 1);
                }
                else
                {
                    RemovedUnitTypes[unit.Type]++;
                }
            }

            HandleUnitArrays(bitReader, replay, data);

            // Now, update the player wireframe.
            UpdateWireframe(player);

            // Check for Morph update
            if (AddedUnits.SequenceEqual(RemovedUnits))
            {
                this.EventType = GameEventType.Inactive;
            }
        }

        /// <summary>
        /// Reads the 8 {16, 8, 8}, 8 {32} struct; the result is in AddedUnits / AddedUnitTypes.
        /// </summary>
        void HandleUnitArrays(BitReader bitReader, Replay replay, UnitData data)
        {
            int wireframeLength = 8;
            if (replay.ReplayBuild >= 22612)
            {
                wireframeLength = 9; // Maximum selection size has been increased to 500, up from 255.
            }

            var typesLength = (int)bitReader.Read(wireframeLength);
            AddedUnitTypes = new Dictionary<UnitType, int>(typesLength);

            // Guarantee order is maintained
            var subgroups = new List<KeyValuePair<UnitType, int>>(typesLength);

            for (int i = 0; i < typesLength; i++)
            {
                var unitTypeId = (int)bitReader.Read(16);
                var unitType = data.GetUnitType(unitTypeId);

                var unitSubtype = bitReader.Read(8);
                if (unitSubtype == 2) // hallucination -- cheers, Graylin
                {
                    unitType = data.GetHallucination(unitType);
                }

                var unitCountType = (int)bitReader.Read(wireframeLength);
                if (unitType == UnitType.Unknown && AddedUnitTypes.ContainsKey(UnitType.Unknown))
                {
                    AddedUnitTypes[UnitType.Unknown] += unitCountType;
                }
                else
                {
                    AddedUnitTypes.Add(unitType, unitCountType);
                }
                subgroups.Add(new KeyValuePair<UnitType, int>(unitType, unitCountType));
            }

            var idsLength = (int)bitReader.Read(wireframeLength);
            AddedUnits = AddedUnits ?? new List<Unit>(idsLength);
            if (idsLength == 0) return;

            var subgroupsEnumerator = subgroups.GetEnumerator();

            int currentSubgroupIndex;
            if (subgroupsEnumerator.MoveNext())
            {
                currentSubgroupIndex = subgroupsEnumerator.Current.Value;
            }
            else return;

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

                if (--currentSubgroupIndex <= 0)
                {
                    if (subgroupsEnumerator.MoveNext())
                    {
                        currentSubgroupIndex = subgroupsEnumerator.Current.Value;
                    }
                }
            }
        }

        /// <summary> Update the wireframe for a player using the data in the event </summary>
        void UpdateWireframe(Player player)
        {
            List<Unit> affectedWireframe;
            if (WireframeIndex == 10)
            {
                affectedWireframe = player.Wireframe;
            }
            else
            {
                affectedWireframe = player.Hotkeys[WireframeIndex];
            }

            if (!ClearSelection)
            {
                // Remove removed units, add added units, sort by unit id (top 14 bits)
                foreach (Unit unit in RemovedUnits)
                {
                    affectedWireframe.Remove(unit);
                }

                foreach (Unit unit in AddedUnits)
                {
                    affectedWireframe.Add(unit);
                }

                affectedWireframe.Sort((m, n) => m.Id - n.Id);
            }
            else
            {
                // Copy added units only to wireframe
                if (WireframeIndex == 10)
                {
                    player.Wireframe = new List<Unit>(AddedUnits);
                    player.Wireframe.Sort((m, n) => m.Id - n.Id);
                }
                else
                {
                    player.Hotkeys[WireframeIndex] = new List<Unit>(AddedUnits);
                    player.Hotkeys[WireframeIndex].Sort((m, n) => m.Id - n.Id);
                }
            }
        }

        /// <summary> 
        /// The index of the wireframe affected; 0~9 refer to control groups,
        /// 10 refers to the current selection
        /// </summary>
        public int WireframeIndex { get; private set; }

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

        /// <summary> A map of the removed unit types to the corresponding unit counts </summary>
        public Dictionary<UnitType, int> RemovedUnitTypes { get; private set; }
    }
}
