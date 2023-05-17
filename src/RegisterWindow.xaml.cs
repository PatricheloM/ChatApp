using System.Linq;
using System.Windows;
using ChatApp.Services;

namespace ChatApp
{
    /// <summary>
    /// Interaction logic for RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        private LoginService loginService = new LoginService();

        private readonly string[] DISALLOWEDUSERNAMES = { "INFOCHANNEL", "MAINCHANNEL" };

        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void RegistrationEvent(object sender, RoutedEventArgs e)
        {
            string lowercaseUser = Username.Text.ToLower();
            if (Password1.Password.Length < 8)
            {
                MessageBox.Show("Password should be at least 8 characters long", "Short password", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (lowercaseUser.Any(char.IsWhiteSpace) || Password1.Password.Any(char.IsWhiteSpace))
            {
                MessageBox.Show("Neither the username nor the password should not contain whitespaces!", "No whitespaces", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                if (!loginService.UserNameExists(lowercaseUser) && !DISALLOWEDUSERNAMES.Select(s => s.ToLower()).ToArray().Contains(lowercaseUser))
                {
                    if (Password1.Password != Password2.Password)
                    {
                        MessageBox.Show("Passwords do not match", "Passwords do not match", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        loginService.RegisterUser(lowercaseUser, Password1.Password);

                        MainWindow mainWindow = new MainWindow();
                        mainWindow.Show();
                        Close();
                    }
                }
                else
                {
                    MessageBox.Show("Username already exists", "Username exists", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
