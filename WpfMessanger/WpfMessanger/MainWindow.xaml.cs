using ClientNamespace;
using ClientNamespace;
using ClientNamespace;
using DataBank;
using JsonParser;
using MessengerClient;
using ObserverNamespace;
using ServerNamespace;
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
            //string username = UsernameBox.Text;
            //string password = PasswordBox.Password;
            //ClientRequestHandler handler = new ClientRequestHandler();

            //handler.RegisterObserver(new ClientMessageObserver());
            //TcpJsonClient client = new TcpJsonClient("10.91.52.224", 3000, handler);
            //client.SendRequest(new LoginSendModel
            //{
            //    username = username,
            //    password = password
            //});
            //if (userDb.Login(username, password))
            //{
            //    MessageBox.Show("Login erfolgreich!");
            //    ChatWindow chatWindow = new ChatWindow(username, client);
            //    chatWindow.Show();
            //    client.SetChatWindow(chatWindow);

            //    this.Close();
            //}
            //else
            //{
            //    MessageBox.Show("Login fehlgeschlagen!");
            //}
            ClientRequestHandler handler = new ClientRequestHandler();
            handler.RegisterObserver(new ServerClientConnectObserver());
            handler.RegisterObserver(new ServerClientLoginObserver());
            handler.RegisterObserver(new ServerClientRegisterObserver());
            handler.RegisterObserver(new ClientMessageObserver());
            handler.RegisterObserver(new ServerClientMessageObserver());
            Server server = new Server(workerCount: 4, requestHandler: handler);

            server.Start(3000);
            MessageBox.Show("Server Start erfolgreich!");
        }
    }
}