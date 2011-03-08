using System;

namespace Starcraft2.ReplayParser
{
    public class ChatMessage
    {
        public int Timestamp { get; set; }
        public int PlayerId { get; set; }
        public int MessageTarget { get; set; }
        public string Message { get; set; }

        public override string ToString()
        {
            return string.Format("[{0}] Player {1}: {2}", MessageTarget, PlayerId, Message);
        }
    }
}
