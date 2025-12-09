using DataBank;
using Delegates;
using JsonParser;
using MessagesNameSpace;
using MessengerClient;
using ServerNamespace;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using UserNamespace;
namespace ClientNamespace
{
    public class TcpJsonClient : IDisposable
    {
        public TcpClient _client;
        private NetworkStream _stream;
        private StreamReader _reader;
        private StreamWriter _writer;

        private ChatWindow chat;
        public bool Connected => _client.Connected;

        public TcpJsonClient(string host, int port)
        {
            _client = new TcpClient();
            _client.Connect(host, port);

            _stream = _client.GetStream();
            _reader = new StreamReader(_stream, Encoding.UTF8);
            _writer = new StreamWriter(_stream, Encoding.UTF8) { AutoFlush = true };

            Console.WriteLine("Connected to server: " + host + ":" + port);

            Task.Run(() => ListenLoop());
        }

        // empfängt Daten vom Server
        private async Task ListenLoop()
        {
            try
            {
                while (true)
                {
                    string? line = await _reader.ReadLineAsync();
                    if (line == null)
                        break;
                    IRequest? request = ParseJsonToRequest(line);
                    //Console.WriteLine("SERVER -> CLIENT: " + line);
                }
            }
            catch
            {
                Console.WriteLine("Disconnected.");
            }
        }
        private IRequest? ParseJsonToRequest(string json)
        {
            using var doc = JsonDocument.Parse(json);
            string type = doc.RootElement.GetProperty("type").GetString()!;

            IRequest? request = type switch
            {
                "login" => JsonSerializer.Deserialize<ClientLoginRequest>(json),
                "register" => JsonSerializer.Deserialize<ServerRegisterRequest>(json),
                "connect" => JsonSerializer.Deserialize<ServerConnectClientRequest>(json),
                "message" => JsonSerializer.Deserialize<Delegates.ServerMessageRequest>(json),
                _ => null
            };

            if (request != null)
                request.json = json;

            return request;
        }
        // sendet ein Request-Objekt als JSON
        public void SendRequest(object request)
        {
            string json = JsonSerializer.Serialize(request);
            _writer.WriteLine(json);
            //Console.WriteLine("CLIENT -> SERVER: " + json);
        }

        public void Dispose()
        {
            _stream?.Dispose();
            _client?.Close();
        }
        public void SetChatWindow(ChatWindow chatWindow)
        {
            this.chat = chatWindow;
        }
        class ClientLoginRequest : IRequest, ITcpClientRequest
        {
            public TcpClient client { get; set; }
            public Server server;
            public ClientTCPConnectedRequest clientTCPConnectedRequest { get; set; }

            public string json { get; set; }
            public User user { get; protected set; }
            public void Execute()
            {
                using var doc = JsonDocument.Parse(json);
                string username = doc.RootElement.GetProperty("username").GetString()!;
                string password = doc.RootElement.GetProperty("password").GetString()!;
                user = UserFunctions.LoginUser(username, password);
                client = clientTCPConnectedRequest._client;


            }
        }
        class ClientMessageRequest : IRequest, ITcpClientRequest
        {
            public TcpClient client { get; set; }

            public string json { get; set; }
            public ClientTCPConnectedRequest clientTCPConnectedRequest { get; set; }
            public User user { get; protected set; }
            public Server server;
            public void Execute()
            {
                using var doc = JsonDocument.Parse(json);
                string username = doc.RootElement.GetProperty("senderId").GetString()!;
                string userId = DataBaseHelper.GetUserId(username);
                string message = doc.RootElement.GetProperty("content").GetString()!;
                string receiver = doc.RootElement.GetProperty("receiverId").GetString()!;
                Message message1 = new Message(username, userId, message);
                client.chat.ReceiveMessage(message1);

            }
        }

    }
}
