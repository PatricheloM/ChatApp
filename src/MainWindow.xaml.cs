using StackExchange.Redis;
using System.Windows;
using ChatApp.Util;
using ChatApp.Services;
using System.Windows.Input;

namespace ChatApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private LoginService loginService = new LoginService();

        private void Connect()
        {
            try
            {
                ConnectionTester.Test();
            }
            catch (RedisConnectionException)
            {
                MessageBox.Show("Can not connect to the server! Restart the application", "Server error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
                
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            Connect();
        }

        private void LoginEvent(object sender, RoutedEventArgs e)
        {
            string lowecaseUser = Username.Text.ToLower();
            if (!loginService.ValidateUser(lowecaseUser, Password.Password))
            {
                MessageBox.Show("Wrong username or password", "Wrong credentions", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                if (loginService.IsUserLoggedIn(lowecaseUser))
                {
                    MessageBox.Show("User already logged in!", "User already logged in", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    loginService.UserLoggedIn(lowecaseUser);
                    ChatRoom chatRoom = new ChatRoom(lowecaseUser);
                    chatRoom.Show();
                    Close();
                }
            }
        }

        private void RegistrationFormShowEvent(object sender, RoutedEventArgs e)
        {
            RegisterWindow registerWindow = new RegisterWindow();
            registerWindow.Show();
            Close();
        }

        private void EnterToLogInEvent(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                LoginEvent(sender, e);
            }
        }
    }
}
