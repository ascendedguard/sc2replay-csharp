namespace Starcraft2.ReplayParser
{
    /// <summary>
    /// Defines who a message in-game was sent to. This also depends
    /// on whether the player was a spectator or player, as spectator chat
    /// is also considered "All"
    /// </summary>
    public enum ChatMessageTarget
    {
        All = 0,
        Allies = 2,
    }
}
