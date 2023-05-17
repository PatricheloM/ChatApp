using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using ChatApp.Redis;
using ChatApp.Services;
using ChatApp.Util;

namespace ChatApp
{
    /// <summary>
    /// Interaction logic for ChatRoom.xaml
    /// </summary>
    public partial class ChatRoom : Window
    {
        private string USER;
        private const string MAINCHANNEL = "MAINCHANNEL";
        private const string INFOCHANNEL = "INFOCHANNEL";

        private MessagingService messagingService = new MessagingService();
        private DatabaseService databaseService = new DatabaseService();
        private LoginService loginService = new LoginService();

        private void ClearTextBox()
        {
            Message.Text = "";
        }

        private void ScrollToBottom()
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                Scroller.ScrollToBottom();
            });
        }

        private void FillRoomWithHistory()
        {
            Dictionary<DateTime, string> messages = databaseService.GetChatHistory();
            var sortedMessages = new SortedDictionary<DateTime, string>(messages);
            foreach (var msg in sortedMessages)
            {
                MessageVisualizer.AddMessage(Room, msg.Key, MessageDeserializer.Serialize(msg.Value), MessageHeadlineType.DEFAULT);
            }
        }

        public ChatRoom(string username)
        {
            InitializeComponent();
            FillRoomWithHistory();
            USER = username;
            RoomWindow.Title = RoomWindow.Title + " (Logged in as " + USER + ')';

            var infoChannel = RedisConnection.Subscribe(INFOCHANNEL);
            infoChannel.OnMessage(message =>
            {
                Message msg = MessageDeserializer.Serialize(message.Message);
                if (msg.Sender != USER)
                {
                    MessageVisualizer.AddMessage(Room, DateTime.Now, msg, MessageHeadlineType.INFO);
                    ScrollToBottom();
                } 

            });

            var privateChannel = RedisConnection.Subscribe(USER);
            privateChannel.OnMessage(message =>
            {
                MessageVisualizer.AddMessage(Room, DateTime.Now, MessageDeserializer.Serialize(message.Message), MessageHeadlineType.PMFROM);
                ScrollToBottom();
            });

            var channel = RedisConnection.Subscribe(MAINCHANNEL);
            channel.OnMessage(message =>
            {
                MessageVisualizer.AddMessage(Room, DateTime.Now, MessageDeserializer.Serialize(message.Message), MessageHeadlineType.DEFAULT);
                ScrollToBottom();
            });

            ScrollToBottom();
        }

        private void SaveMassageToDatabase(string publishable)
        {
            databaseService.SaveMessageToDatabse(DateTime.Now, publishable);
        }

        private void EnterToSendTextMessageEvent(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                SendTextMessageEvent(sender, e);
            }
        }

        private void SendTextMessageEvent(object sender, RoutedEventArgs e)
        {
            const string PMPREFIX = "/pm";
            const string ONLINEUSERSPREFIX = "/onlineusers";

            if (Message.Text.Length == 0)
            {
                MessageBox.Show("You can't send empty messages!", "Empty message", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                if (Message.Text.StartsWith(PMPREFIX))
                {
                    string[] splits = Message.Text.Split(' ');
                    if (splits.Length > 2)
                    {
                        string receiver = splits[1].ToLower();
                        Message msg = new()
                        {
                            Sender = USER,
                            MessageType = MessageType.TEXT,
                            Content = Message.Text.Remove(0, (PMPREFIX + splits[1]).Length + 2)
                        };

                        if (msg.Sender == receiver)
                        {
                            MessageBox.Show("You can't send private message to yourself!", "You played yourself", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        else if (!loginService.GetOnlineUsers().Contains(receiver))
                        {
                            MessageBox.Show("User is not online! Use /onlineusers to see online users", "User not online", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        else
                        {

                            messagingService.SendMessage(receiver, msg);
                            MessageVisualizer.AddMessage(Room, DateTime.Now, msg, MessageHeadlineType.PMTO, receiver);
                            ClearTextBox();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Use /pm <username> <msg> syntax to use private messaging!", "Wrong syntax", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else if (Message.Text.StartsWith(ONLINEUSERSPREFIX)) 
                {
                    var usersStr = new StringBuilder();
                    foreach (var u in loginService.GetOnlineUsers())
                    {
                        usersStr.Append(u + ' ');
                    }
                    Message onlineUsersMsg = new()
                    {
                        Sender = USER,
                        MessageType = MessageType.ONLINEUSERS,
                        Content = usersStr.ToString()
                    };
                    MessageVisualizer.AddMessage(Room, DateTime.Now, onlineUsersMsg, MessageHeadlineType.INFO);
                    ClearTextBox();
                }
                else
                {
                    Message publishable = new()
                    {
                        Sender = USER,
                        MessageType = MessageType.TEXT,
                        Content = Message.Text
                    };
                    messagingService.SendMessage(MAINCHANNEL, publishable);
                    SaveMassageToDatabase(MessageDeserializer.Deserialize(publishable));
                    ClearTextBox();
                }
            }
        }

        private void ChatRoomClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            loginService.UserLoggedOut(USER);
        }

        private void SendFileMessageEvent(object sender, RoutedEventArgs e)
        {

            OpenFileDialog op = new OpenFileDialog
            {
                Title = "Select a picture",
                Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png"
            };
            if (op.ShowDialog() == true)
            {
                byte[] data;
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(new BitmapImage(new Uri(op.FileName))));
                using (MemoryStream ms = new())
                {
                    encoder.Save(ms);
                    data = ms.ToArray();
                }

                Message msg = new()
                {
                    Sender = USER,
                    MessageType = MessageType.IMAGE,
                    Content = Convert.ToBase64String(data, 0, data.Length)
                };
                messagingService.SendMessage(MAINCHANNEL, msg);
                SaveMassageToDatabase(MessageDeserializer.Deserialize(msg));
            }
        }
    }
}
