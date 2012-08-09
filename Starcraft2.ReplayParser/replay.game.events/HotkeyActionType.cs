// -----------------------------------------------------------------------
// <copyright file="HotkeyActionType.cs">
// Copyright 2012 Robert Nix, Will Eddins
// </copyright>
// -----------------------------------------------------------------------

namespace Starcraft2.ReplayParser
{
    /// <summary> Type of hotkey events </summary>
    public enum HotkeyActionType
    {
        /// <summary> Replace control group with current selection </summary>
        SetControlGroup,
        /// <summary> Add current selection to control group </summary>
        AddToControlGroup,
        /// <summary> Replace current selection with control group </summary>
        SelectControlGroup
    }
}
