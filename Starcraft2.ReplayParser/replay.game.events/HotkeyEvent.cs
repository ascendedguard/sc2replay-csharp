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
            this.EventType = GameEventType.Selection;

            ControlGroup = (int)bitReader.Read(4);

            // throws
            ActionType = (HotkeyActionType)(int)bitReader.Read(2);

            var updateType = (int)bitReader.Read(2);
            if (updateType > 1) // Debug:  if this isn't known, we're probably fucked
            {
                var zero = 0d;
            }

            // This is an internal update that will affect the precondition
            // of the other actions... it basically updates control groups
            // for units dying whenever needed, so we don't have to guess!
            if (updateType == 1) // Remove by flags
            {
                var numBits = (int)bitReader.Read(8);
                if (numBits > player.Hotkeys[ControlGroup].Count) // Debug:  Maybe we were wrong?
                {
                    var zero = 0d;
                }
                while (numBits >= 8)
                {
                }
            }

            if (ActionType == HotkeyActionType.AddToControlGroup)
            {
                var oldControlgroup = player.Hotkeys[ControlGroup];
                var newControlgroup = new List<Unit>(
                    player.Wireframe.Count + oldControlgroup.Count);

                foreach (Unit unit in oldControlgroup)
                {
                    newControlgroup.Add(unit);
                }
                foreach (Unit unit in player.Wireframe)
                {
                    newControlgroup.Add(unit);
                }
                newControlgroup.Sort((m, n) => m.Id - n.Id);

                player.Hotkeys[ControlGroup] = newControlgroup;
            }
            else if (ActionType == HotkeyActionType.SelectControlGroup)
            {
                // Don't think this needs to be a copy, but just in case...
                player.Wireframe = new List<Unit>(player.Hotkeys[ControlGroup]);
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