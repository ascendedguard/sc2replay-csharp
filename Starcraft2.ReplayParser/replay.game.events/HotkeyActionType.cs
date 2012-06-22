// -----------------------------------------------------------------------
// <copyright file="HotkeyActionType.cs" company="Microsoft">
// TODO: Update copyright text.
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
