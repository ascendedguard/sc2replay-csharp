// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HotkeyEvent.cs" company="SC2ReplayParser">
//   Copyright © 2011 All Rights Reserved
// </copyright>
// <summary>
//   Describes an event where the player has set or used a hotkey.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Starcraft2.ReplayParser
{
    using System.Collections.Generic;

    using Streams;

    /// <summary> Describes an event where the player has set or used a hotkey. </summary>
    public class HotkeyEvent : GameEventBase
    {
        public HotkeyEvent(BitReader bitReader, Replay replay, Player player)
        {
            int wireframeLength = 8;
            if (replay.ReplayBuild >= 22612)
            {
                wireframeLength = 9; // Maximum selection size has been increased to 500, up from 255.
            }
            this.EventType = GameEventType.Selection;

            ControlGroup = (int)bitReader.Read(4);

            // throws
            ActionType = (HotkeyActionType)(int)bitReader.Read(2);

            var updateType = (int)bitReader.Read(2);

            // This is an internal update that is somewhat asynchronous to
            // the main wireframe.
            var unitsRemovedList = new List<Unit>();
            if (updateType == 1) // Remove by flags
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
                        unitsRemovedList.Add(player.Hotkeys[ControlGroup][i]);
                    }
                }
            }
            else if (updateType == 2)
            {
                var numIndices = (int)bitReader.Read(wireframeLength);
                for (int i = 0; i < numIndices; i++)
                {
                    unitsRemovedList.Add(player.Hotkeys[ControlGroup][(int)bitReader.Read(wireframeLength)]);
                }
            }
            else if (updateType == 3) // Replace control group with portion of control group
            {
                // This happens fairly rarely, so I'll just invert the output
                unitsRemovedList = new List<Unit>(player.Hotkeys[ControlGroup]);

                var numIndices = (int)bitReader.Read(wireframeLength);
                for (int i = 0; i < numIndices; i++)
                {
                    unitsRemovedList.Remove(player.Hotkeys[ControlGroup][(int)bitReader.Read(wireframeLength)]);
                }
            }

            if (ActionType == HotkeyActionType.AddToControlGroup)
            {
                var oldControlgroup = player.Hotkeys[ControlGroup];
                List<Unit> newControlgroup;
                if (oldControlgroup != null)
                {
                    newControlgroup = new List<Unit>(player.Wireframe.Count + oldControlgroup.Count);

                    foreach (Unit unit in oldControlgroup)
                    {
                        newControlgroup.Add(unit);
                    }
                }
                else
                {
                    newControlgroup = new List<Unit>(player.Wireframe.Count);
                }

                foreach (Unit unit in player.Wireframe)
                {
                    if (oldControlgroup == null || !oldControlgroup.Contains(unit))
                    {
                        newControlgroup.Add(unit);
                    }
                }
                newControlgroup.Sort((m, n) => m.Id - n.Id);

                player.Hotkeys[ControlGroup] = newControlgroup;
            }
            else if (ActionType == HotkeyActionType.SelectControlGroup)
            {
                player.Wireframe = new List<Unit>(player.Hotkeys[ControlGroup]);

                // Only see these two together because of the nature of it
                foreach (Unit unit in unitsRemovedList)
                {
                    player.Wireframe.Remove(unit);
                }
            }
            else if (ActionType == HotkeyActionType.SetControlGroup)
            {
                player.Hotkeys[ControlGroup] = new List<Unit>(player.Wireframe);
            }

            // Copy ref list to property.  Idk if this is a great idea, but it's likely
            // never more than 30 ish dwords per event?  Can't be more than another meg
            // or three per replay.  i.e. Can't be more than lolJava.
            ControlGroupUnits = new List<Unit>(player.Hotkeys[ControlGroup]);
        }

        /// <summary> The control group (0~9) of the event (note: the leftmost control
        /// group is 1 and the rightmost is 0) </summary>
        public int ControlGroup { get; private set; }

        /// <summary> The type of control group action </summary>
        public HotkeyActionType ActionType { get; private set; }

        /// <summary> A list of the units in the player's
        /// control group after the event completes </summary>
        public List<Unit> ControlGroupUnits { get; private set; }
    }
}
