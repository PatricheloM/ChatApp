using StackExchange.Redis;
using System;

namespace ChatApp.Redis
{
    static class RedisRepository
    {

        private static readonly IDatabase db = RedisConnection.GetConnection();

        public static void Publish(string channel, string message)
        {
            db.Publish(channel, message);
        }

        public static string HGet(string key, string field)
        {
            return db.HashGet(key, field);
        }

        public static void HMSet(string key, params HashEntry[] values)
        {
            db.HashSet(key, values);
        }

        public static HashEntry[] HGetAll(string key)
        {
            return db.HashGetAll(key);
        }

        public static void SAdd(string key, string value)
        {
            db.SetAdd(key, value);
        }

        public static string[] SMembers(string key)
        {
            return db.SetMembers(key).ToStringArray();
        }

        public static void SRem(string key, string value)
        {
            db.SetRemove(key, value);
        }

        public static bool SIsMember(string key, string value)
        {
            return Array.IndexOf(SMembers(key), value) != -1;
        }

        public static void Ping()
        {
            db.Ping();
        }
    }
}
