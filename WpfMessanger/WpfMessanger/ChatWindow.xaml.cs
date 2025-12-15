using ClientNamespace;
using DataBank;
using JsonParser;
using MessagesNameSpace;
using Microsoft.Data.Sqlite;
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
using DataBank;
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
            SqliteConnection connection =  DataBaseHelper.GetUserConnection(username);
            List<Message> messages = DataBaseHelper.GetMessageObject(connection);
            foreach (var message in messages)
            {
                ListBoxItem item = new ListBoxItem
                {
                    Content = message.content,
                    HorizontalContentAlignment = message.senderId == username ? HorizontalAlignment.Left : HorizontalAlignment.Right
                };
                this.MessageList.Items.Add(item);
            }

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
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(() => ReceiveMessage(message));
                return;
            }
            // Nachricht zur MessageList hinzufügen
            ListBoxItem item = new ListBoxItem
            {
                Content = message.content,
                HorizontalContentAlignment = HorizontalAlignment.Right
            };
            this.MessageList.Items.Add(item);
        }
    }
}
