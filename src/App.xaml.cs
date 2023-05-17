using ChatApp.Util;
using StackExchange.Redis;
using System.Windows;

namespace ChatApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static void Connect()
        {
            try
            {
                ConnectionTester.Test();
            }
            catch (RedisConnectionException)
            {
                MessageBox.Show("Can not connect to the server! Restart the application", "Server error", MessageBoxButton.OK, MessageBoxImage.Error);
                Current.Shutdown();

            }
        }

        public App()
        {
            Connect();
        }
    }
}
