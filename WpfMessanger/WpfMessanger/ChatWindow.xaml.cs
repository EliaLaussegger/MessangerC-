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
using MessagesNameSpace;
namespace MessengerClient
{
    /// <summary>
    /// Interaktionslogik für ChatWindow.xaml
    /// </summary>
    public partial class ChatWindow : Window
    {
        private string username;
        private TcpJsonClient client;

        // Parameterloser Konstruktor, falls die XAML oder anderer Code eine
        // Instanz ohne Parameter erzeugt.
        public ChatWindow()
        {
            InitializeComponent();
        }

        public ChatWindow(string username, TcpJsonClient client) : this()
        {
            this.username = username;
            this.client = client;
        }

        private void SendMessage_Click(object sender, RoutedEventArgs e)
        {
            // Nachricht aus dem TextBox-Element holen und verarbeiten
            string message = this.MessageBox.Text;
            if (!string.IsNullOrWhiteSpace(message))
            {
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
        public void ReceiveMessage(Message message)
        {
            // Nachricht zur MessageList hinzufügen
            this.MessageList.Items.Add(message);
        }
    }
}
