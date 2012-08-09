// -----------------------------------------------------------------------
// <copyright file="AbilityEvent.cs">
// Copyright 2012 Robert Nix, Will Eddins
// </copyright>
// -----------------------------------------------------------------------

namespace Starcraft2.ReplayParser
{
    using System;
    using System.Collections.Generic;

    using Streams;
    using Version;

    /// <summary>
    /// A player ability action.
    /// </summary>
    public class AbilityEvent : GameEventBase
    {
        public AbilityEvent(BitReader bitReader, Replay replay, Player player, AbilityData abilityData, UnitData unitData)
        {
            uint flags;
            //   1.3.3 patch notes:
            //   - Fixed an issue where the APM statistic could be artificially increased.
            // This adds the "failed" flag, which is triggered usually by holding down a
            // hotkey, leading to key repeat spamming the event throughout a single tick.
            if (replay.ReplayBuild < 18574) // < 1.3.3
            {
                flags = bitReader.Read(17);
            }
            else if (replay.ReplayBuild < 22612) // < 1.5.0
            {
                flags = bitReader.Read(18);
            }
            else
            {
                flags = bitReader.Read(20);
            }
            Queued = (flags & 2) != 0;
            RightClick = (flags & 8) != 0;
            WireframeClick = (flags & 0x20) != 0;
            ToggleAbility = (flags & 0x40) != 0;
            EnableAutoCast = (flags & 0x80) != 0;
            AbilityUsed = (flags & 0x100) != 0;
            WireframeUnload = (flags & 0x200) != 0;
            WireframeCancel = (flags & 0x400) != 0;
            MinimapClick = (flags & 0x10000) != 0;
            AbilityFailed = (flags & 0x20000) != 0;

            // flags & 0xf815 -> Debug for unknown flags
            // Never found any across all test data.

            DefaultAbility = (bitReader.Read(1) == 0);
            DefaultActor = true;
            if (!DefaultAbility)
            {
                AbilityType = abilityData.GetAbilityType(
                    (int)bitReader.Read(16),
                    (int)bitReader.Read(5));
                DefaultActor = (bitReader.Read(1) == 0);
                if (!DefaultActor)
                {   // I'm thinking this would be an array type... but I can't
                    // find anything that causes this bit to be set.
                    throw new InvalidOperationException("Unsupported: non-default actor");
                }
            }
            if (DefaultActor)
            {
                // Deep copy the current wireframe as the actor list
                // -----
                // If a user wants to deal with subgroups to get a more
                // concise actor list, the data is all here.  We're not
                // going to bother, though, because there are several
                // exceptions to account for in determining event actors.
                Actors = new List<Unit>(player.Wireframe.Count);
                foreach (var unit in player.Wireframe)
                {
                    Actors.Add(new Unit(unit));
                }
            }

            var targetType = bitReader.Read(2);
            if (targetType == 1) // Location target
            {
                var targetX = bitReader.Read(20);
                var targetY = bitReader.Read(20);
                var targetZ = bitReader.Read(32);
                TargetLocation = Location.FromEventFormat(targetX, targetY, targetZ);
            }
            else if (targetType == 2) // Unit + Location target
            {
                TargetFlags = (int)bitReader.Read(8);
                WireframeIndex = (int)bitReader.Read(8);

                var unitId = (int)bitReader.Read(32);
                var unit = replay.GetUnitById(unitId);
                var unitTypeId = (int)bitReader.Read(16);
                if (unit == null)
                {
                    var unitType = unitData.GetUnitType(unitTypeId);
                    unit = new Unit(unitId, unitType);
                    unit.typeId = unitTypeId;
                    replay.GameUnits.Add(unitId, unit);
                }

                TargetUnit = unit;

                var targetHasPlayer = bitReader.Read(1) == 1;
                if (targetHasPlayer)
                {
                    TargetPlayer = (int)bitReader.Read(4);
                }

                // 1.4.0 -- Don't really know what this was meant to fix
                if (replay.ReplayBuild >= 19595)
                {
                    var targetHasTeam = bitReader.Read(1) == 1;
                    if (targetHasTeam)
                    {
                        TargetTeam = (int)bitReader.Read(4);
                    }
                }

                var targetX = bitReader.Read(20);
                var targetY = bitReader.Read(20);
                var targetZ = bitReader.Read(32);
                TargetLocation = Location.FromEventFormat(targetX, targetY, targetZ);
            }
            else if (targetType == 3) // Unit target
            {
                var id = bitReader.Read(32);
                // Again, if the user wants to determine exactly which
                // queue item is canceled in the case of a queue cancel
                // event (the most common case of this target specifier's
                // occurence), they can; however, it requires an additional
                // data structure that I don't want to bother with; however,
                // all the underlying data is available in the events list.
                TargetId = id;
            }

            var lastBit = bitReader.Read(1); // Should be 0; if not, misalignment is likely

            if (!AbilityFailed)
            {
                if (RightClick)
                {
                    this.EventType = GameEventType.RightClick;
                }
                else
                {
                    this.EventType = EventData.GetInstance().GetEventType(this.AbilityType);
                }
            }
            else
            {
                this.EventType = GameEventType.Inactive;
            }
        }

        // ----------- flags ------------

        /// <summary>
        /// True if the event was created as a queued
        /// ability, i.e. by holding down Shift
        /// </summary>
        public bool Queued { get; private set; }

        /// <summary>
        /// True if the event was created using right click
        /// </summary>
        public bool RightClick { get; private set; }

        /// <summary>
        /// True if the event was created using a button
        /// on the wireframe and not the command card
        /// </summary>
        public bool WireframeClick { get; private set; }

        /// <summary>
        /// True if the event was created by toggling an ability's auto-cast
        /// functionality, i.e. by holding Alt or right clicking the command card
        /// </summary>
        public bool ToggleAbility { get; private set; }

        /// <summary>
        /// True if the event turns on auto-cast for the ability
        /// </summary>
        public bool EnableAutoCast { get; private set; }

        /// <summary>
        /// True if the ability was used.
        /// </summary>
        public bool AbilityUsed { get; private set; }

        /// <summary>
        /// True if the event was created by clicking a unit in the
        /// wireframe in a Transport unit (i.e. unloading a single unit)
        /// </summary>
        public bool WireframeUnload { get; private set; }

        /// <summary>
        /// True if the event was created by clicking a queue item in the
        /// wireframe (as opposed to simply pressing Cancel)
        /// </summary>
        public bool WireframeCancel { get; private set; }

        /// <summary>
        /// True if the ability target was chosen through the minimap
        /// </summary>
        public bool MinimapClick { get; private set; }

        /// <summary>
        /// True if the ability failed to complete
        /// </summary>
        public bool AbilityFailed { get; private set; }

        // --------- data ----------

        /// <summary>
        /// True if the ability is a default ability, such as harvest
        /// for SCVs, produced by right clicking with a unit selected
        /// </summary>
        public bool DefaultAbility { get; private set; }

        /// <summary>
        /// The type of an ability if not default, otherwise Unknown
        /// </summary>
        public AbilityType AbilityType { get; private set; }

        /// <summary>
        /// True if the actor is default, otherwise sometimes people
        /// are really good at micro.
        /// </summary>
        public bool DefaultActor { get; private set; }

        /// <summary>
        /// The actors
        /// </summary>
        public List<Unit> Actors { get; private set; }

        /// <summary>
        /// True if the event uses a default target, e.g. for self-cast abilities
        /// </summary>
        public bool DefaultTarget { get; private set; }

        /// <summary> 8 bits of flags used in Unit+Location targets.  Unknown. </summary>
        public int TargetFlags { get; private set; }

        /// <summary> The wireframe index of the target unit (unconfirmed) </summary>
        public int WireframeIndex { get; private set; }

        /// <summary> The target unit of the ability or null </summary>
        public Unit TargetUnit { get; private set; }

        /// <summary> The long id of the target unit or queue item </summary>
        public uint TargetId { get; private set; }

        /// <summary> The target location of the ability or null </summary>
        public Location TargetLocation { get; private set; }

        public int TargetPlayer { get; private set; }

        public int TargetTeam { get; private set; }
    }
}
