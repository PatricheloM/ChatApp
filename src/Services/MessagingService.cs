using ChatApp.Redis;
using ChatApp.Util;

namespace ChatApp.Services
{
    class MessagingService
    {
        public void SendMessage(string channel, Message msg)
        {
            RedisRepository.Publish(channel, MessageDeserializer.Deserialize(msg));
        }
    }
}
