using StackExchange.Redis;
using System;
using ChatApp.Redis;

namespace ChatApp.Util
{
    static class ConnectionTester
    {
        public static void Test()
        {
            try
            {
                RedisRepository.Ping();
            }
            catch (Exception)
            {
                throw new RedisConnectionException(ConnectionFailureType.SocketFailure, "Can not connect to this adderss or invalid password!");
            }
        }
    }
}
