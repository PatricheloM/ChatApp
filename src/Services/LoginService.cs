using System;
using System.Linq;
using ChatApp.Redis;
using ChatApp.Util;

namespace ChatApp.Services
{
    class LoginService
    {
        private const string USERNAMESETKEY = "users";
        private const string USERNAMEPASSCOMBOSKEY = "userpasscombos";
        private const string ONLINEUSERS = "onlineusers";
        private const string INFOCHANNEL = "INFOCHANNEL";

        private MessagingService messagingService = new MessagingService();

        private void SendLoginLogoutInfo(string username, MessageType messageType)
        {
            Message msg = new()
            {
                Sender = username,
                MessageType = messageType,
                Content = ""
            };
            messagingService.SendMessage(INFOCHANNEL, msg);
        }

        public bool UserNameExists(string username) 
        {
            return RedisRepository.SMembers(USERNAMESETKEY).Contains(username.ToLower());
        }

        public bool ValidateUser(string username, string password) 
        {
            if (UserNameExists(username.ToLower()))
            {
                if (SHA1Encrypt.Hash(password).ToLower() == RedisRepository.HGet(USERNAMEPASSCOMBOSKEY, username.ToLower()).ToLower()) return true;
            }
            return false;
        }

        public void RegisterUser(string username, string password)
        {
            RedisRepository.SAdd(USERNAMESETKEY, username.ToLower());
            RedisRepository.HMSet(USERNAMEPASSCOMBOSKEY, new StackExchange.Redis.HashEntry(username.ToLower(), SHA1Encrypt.Hash(password)));
        }

        public bool IsUserLoggedIn(string username)
        {
            return RedisRepository.SIsMember(ONLINEUSERS, username.ToLower());
        }

        public void UserLoggedIn(string username)
        {
            RedisRepository.SAdd(ONLINEUSERS, username.ToLower());
            SendLoginLogoutInfo(username, MessageType.LOGIN);
        }

        public void UserLoggedOut(string username)
        {
            RedisRepository.SRem(ONLINEUSERS, username.ToLower());
            SendLoginLogoutInfo(username, MessageType.LOGOUT);
        }

        public string[] GetOnlineUsers()
        {
            return RedisRepository.SMembers(ONLINEUSERS);
        }
    }
}
