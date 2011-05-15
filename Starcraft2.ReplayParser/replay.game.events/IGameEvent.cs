namespace Starcraft2.ReplayParser
{
    public interface IGameEvent
    {
        GameEventType EventType { get; set; }
        Player Player { get; set; }
        Timestamp Time { get; set; }
    }
}
