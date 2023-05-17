using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ChatApp.Util
{
    static class MessageVisualizer
    {
        public static void AddMessage(StackPanel room, DateTime timeStamp, Message message, MessageHeadlineType headlineType, string? receiver=null)
        {
            if (message.MessageType != MessageType.IMAGE)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    TextBlock msgBlock = new()
                    {
                        Width = 400,
                        Padding = new Thickness(5),
                        TextWrapping = TextWrapping.Wrap
                    };
                    if (headlineType == MessageHeadlineType.DEFAULT) msgBlock.Inlines.Add(new Bold(new Run(message.Sender + " (" + timeStamp + "):\n")));
                    if (headlineType == MessageHeadlineType.PMFROM) msgBlock.Inlines.Add(new Bold(new Run("PM from " + message.Sender + " (" + timeStamp + "):\n")));
                    if (headlineType == MessageHeadlineType.PMTO) msgBlock.Inlines.Add(new Bold(new Run("PM to " + receiver + " (" + timeStamp + "):\n")));
                    if (headlineType == MessageHeadlineType.INFO && message.MessageType == MessageType.LOGIN) msgBlock.Inlines.Add(new Bold(new Run("User " + message.Sender + " logged in (" + timeStamp + ")")));
                    if (headlineType == MessageHeadlineType.INFO && message.MessageType == MessageType.LOGOUT) msgBlock.Inlines.Add(new Bold(new Run("User " + message.Sender + " logged out (" + timeStamp + ")")));
                    if (headlineType == MessageHeadlineType.INFO && message.MessageType == MessageType.ONLINEUSERS) msgBlock.Inlines.Add(new Bold(new Run("Online users: " + message.Content)));
                    if (headlineType != MessageHeadlineType.INFO) msgBlock.Inlines.Add(new Run(message.Content));

                    Border border = new()
                    {
                        BorderThickness = new Thickness(1),
                        BorderBrush = headlineType == MessageHeadlineType.DEFAULT ? Brushes.Black : headlineType == MessageHeadlineType.INFO ? Brushes.Red : Brushes.Yellow,
                        Child = msgBlock
                    };

                    border.Child = msgBlock;

                    room.Children.Add(border);
                });
            }
            else
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    TextBlock msgBlock = new()
                    {
                        Width = 400,
                        Padding = new Thickness(5)
                    };
                    msgBlock.Inlines.Add(new Bold(new Run(message.Sender + " (" + timeStamp + "):\n")));
                    byte[] binaryData = Convert.FromBase64String(message.Content);

                    BitmapImage bi = new();
                    bi.BeginInit();
                    bi.StreamSource = new MemoryStream(binaryData);
                    bi.EndInit();

                    Image img = new()
                    {
                        Source = bi,
                        Margin = new Thickness(10, 10, 10, 10)
                    };

                    msgBlock.Inlines.Add(img);

                    Border border = new()
                    {
                        BorderThickness = new Thickness(1),
                        BorderBrush = Brushes.Black,
                        Child = msgBlock
                    };

                    room.Children.Add(border);
                });
            }
        }
    }
}
