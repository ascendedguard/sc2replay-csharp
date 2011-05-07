// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChatMessage.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the ChatMessage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Starcraft2.ReplayParser
{
    using System;

    public class ChatMessage
    {
        public TimeSpan Timestamp { get; set; }
        public int PlayerId { get; set; }
        public ChatMessageTarget MessageTarget { get; set; }
        public string Message { get; set; }

        public override string ToString()
        {
            return string.Format("({0}) [{1}] Player {2}: {3}", Timestamp, MessageTarget, PlayerId, Message);
        }
    }
}
