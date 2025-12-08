using ClientNamespace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using JsonParser;
namespace MessengerClient
{
    /// <summary>
    /// Interaktionslogik für ChatWindow.xaml
    /// </summary>
    public partial class ChatWindow : Window
    {
        private string username;

        // Parameterloser Konstruktor, falls die XAML oder anderer Code eine
        // Instanz ohne Parameter erzeugt.
        public ChatWindow()
        {
            InitializeComponent();
        }

        public ChatWindow(string username) : this()
        {
            this.username = username;
        }

        private void SendMessage_Click(object sender, RoutedEventArgs e)
        {
            // Nachricht aus dem TextBox-Element holen und verarbeiten
            string message = this.MessageBox.Text;
            if (!string.IsNullOrWhiteSpace(message))
            {
                var client = new TcpJsonClient("192.168.178.39", 3000);
                client.SendRequest(new MessageSendModel
                {
                    senderId = username,
                    receiverId = "lilo",
                    content = message
                });
                this.MessageList.Items.Add(message);
                this.MessageBox.Clear();    
            }
        }
    }
}
