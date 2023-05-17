using StackExchange.Redis;
using ChatApp.Json.Settings;

namespace ChatApp.Redis
{
    static class RedisConnection
    {
        private static readonly string connAddr = SettingsRepository.GetSettings().RedisConnection;
        private static readonly string connPass = ",password=" + SettingsRepository.GetSettings().RedisPassword;
        private static readonly ConnectionMultiplexer connection = ConnectionMultiplexer.Connect(connAddr + connPass);

        public static IDatabase GetConnection()
        {
            return connection.GetDatabase();
        }

        public static ChannelMessageQueue Subscribe(string channel)
        {
            return connection.GetSubscriber().Subscribe(channel);
        }
    }
}
