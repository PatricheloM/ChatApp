using System;

namespace ChatApp.Util
{
    static class MessageDeserializer
    {
        public static Message Serialize(string published)
        {
            string[] splits = published.Split('|');
            return new()
            {
                Sender = splits[0],
                MessageType = (MessageType)Enum.Parse(typeof(MessageType), splits[1]),
                Content = published.Remove(0, splits[0].Length + splits[1].Length + 2)
            };
        }

        public static string Deserialize(Message message)
        { 
            return message.Sender + '|' + message.MessageType + '|' + message.Content;
        }
    }
}
