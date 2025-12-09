using ClientNamespace;
using DataBank;
using MessengerClient;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using JsonParser;
using ClientNamespace;
namespace WpfMessanger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CentralUserDB userDb;

        public MainWindow()
        {
            InitializeComponent();
            userDb = CentralUserDB.Instance; // wenn du Singleton machst
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameBox.Text;
            string password = PasswordBox.Password;
            var client = new TcpJsonClient("192.168.178.39", 3000);
            client.SendRequest(new LoginSendModel
            {
                username = username,
                password = password
            });
            if (userDb.Login(username, password))
            {
                MessageBox.Show("Login erfolgreich!");
                ChatWindow chatWindow = new ChatWindow(username, client);
                chatWindow.Show();
                client.SetChatWindow(chatWindow);

                this.Close();
            }
            else
            {
                MessageBox.Show("Login fehlgeschlagen!");
            }
        }
    }
}