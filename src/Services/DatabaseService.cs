using StackExchange.Redis;
using System;
using System.Collections.Generic;
using ChatApp.Redis;

namespace ChatApp.Services
{
    class DatabaseService
    {
        private const string CHATDB = "mainchannel";

        public void SaveMessageToDatabse(DateTime timeStamp, string publishedMsg)
        {
            RedisRepository.HMSet(CHATDB, new HashEntry(timeStamp.ToString("O"), publishedMsg));
        }

        public Dictionary<DateTime, string> GetChatHistory()
        {
            Dictionary<DateTime, string> messages = new();
            foreach (var entry in RedisRepository.HGetAll(CHATDB))
            {
                messages.Add(DateTime.Parse(entry.Name), entry.Value);
            }
            return messages;
        }
    }
}
