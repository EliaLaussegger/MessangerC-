using DataBank;
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
            string username = UsernameBox.Text;
            string password = PasswordBox.Password;

            if (userDb.Login(username, password))
            {
                MessageBox.Show("Login erfolgreich!");
                //new ChatWindow(username).Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Login fehlgeschlagen!");
            }
        }
    }
}