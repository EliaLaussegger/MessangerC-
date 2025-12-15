using ClientNamespace;
using DataBank;
using Delegates;
using Microsoft.Data.Sqlite;
using ObserverNamespace;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ThreadPoolNamespace;
using UserNamespace;
using MessagesNameSpace;
namespace ServerNamespace
{
    public class Server
    {

        public ThreadPoolNamespace.ThreadPool _threadPool;
        private TcpListener? _listener;
        private readonly ClientRequestHandler _requestHandler;
        private StreamWriter _writer;
        private NetworkStream _stream;
        public List<ClientTCPConnectedRequest> connectedClients = new List<ClientTCPConnectedRequest>();
        public List<TcpJsonClient> clients = new List<TcpJsonClient>();
        public Server(int workerCount, ClientRequestHandler requestHandler)
        {
            _threadPool = new ThreadPoolNamespace.ThreadPool(workerCount);
            _requestHandler = requestHandler;
        }
        public void HandleClientRequest(IRequest requestHandler)
        {
            _threadPool.QueueWorkItem(requestHandler);
        }
        public void Start(int port)
        {
            _listener = new TcpListener(IPAddress.Parse("0.0.0.0"), port);
            //_writer = new StreamWriter(_stream, Encoding.UTF8) { AutoFlush = true };
            try
            {
                _listener.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Listener konnte nicht gestartet werden: " + ex.Message);
                throw;
            }

            Console.WriteLine("Server started on port " + port);
            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var addr in host.AddressList.Where(a => a.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork))
                {
                    Console.WriteLine($"Listening on {addr}:{port}  (use this IP from remote clients)");
                }
            }
            catch
            {
                Console.WriteLine("Konnte lokale IP-Adressen nicht ermitteln. Nutze 'ipconfig' oder 'Get-NetIPAddress'.");
            }

            Task.Run(() => AcceptLoop());
        }
        public void DisconnectClient(ClientTCPConnectedRequest client)
        {
            for (int i = 0; i < connectedClients.Count; i++)
            {
                if (connectedClients[i] == client)
                {
                    connectedClients.RemoveAt(i);
                    Console.WriteLine("Client disconnected.");
                    break;
                }
            }
            Console.WriteLine("Server stopped.");
        }
        private async Task AcceptLoop()
        {
            while (true)
            {
                TcpClient clientTCP = await _listener!.AcceptTcpClientAsync();
                _stream = clientTCP.GetStream();

                Console.WriteLine("Client connected via TCP.");
                ClientTCPConnectedRequest clientTCPConnectedRequest = new ClientTCPConnectedRequest(clientTCP, _requestHandler);
                clientTCPConnectedRequest.server = this;
                clientTCPConnectedRequest._client.clientTCPConnectedRequest = clientTCPConnectedRequest;
                connectedClients.Add(clientTCPConnectedRequest);
                _threadPool.QueueWorkItem(clientTCPConnectedRequest);
            }
        }
        public void SendRequest(object request)
        {
            string json = JsonSerializer.Serialize(request);
            _writer.WriteLine(json);
        }

    }
    public class ClientTCPConnectedRequest : IRequest
    {

        public TcpJsonClient _client;
        public TcpClient _clientTCP;
        private readonly ClientRequestHandler _handler;
        public Server server;
        public NetworkStream stream;
        public StreamWriter streamWriter;
        public User user;
        public string json { get; set; }


        public ClientTCPConnectedRequest(TcpClient client, ClientRequestHandler handler)
        {
            _clientTCP = client;
            _handler = handler;
        }

        public void Execute()
        {
            stream = _clientTCP.GetStream();
            using var reader = new StreamReader(stream, Encoding.UTF8);
            streamWriter = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                //Console.WriteLine("Received JSON: " + line);

                IRequest? request = ParseJsonToRequest(line);

                switch (request)
                {
                    case ServerLoginRequest clr:
                        clr.clientTCPConnectedRequest = this;
                        clr.server = server;
                        _handler.NotifyObservers(clr);
                        break;
                    case ServerRegisterRequest crr:
                        _handler.NotifyObservers(crr);
                        break;
                    case ServerConnectClientRequest ccr:
                        _handler.NotifyObservers(ccr);
                        break;
                    case Delegates.ServerMessageRequest cmr:
                        cmr.clientTCPConnectedRequest = this;
                        _handler.NotifyObservers(cmr);
                        ServerMessageRequest serverMessageRequest = new ServerMessageRequest();
                        serverMessageRequest.clientTCPConnectedRequest = this;
                        serverMessageRequest.clientMessageRequest = cmr;
                        server._threadPool.QueueWorkItem(serverMessageRequest);
                        break;
                    default:
                        Console.WriteLine("Unknown request type: " + request.GetType());
                        break;
                }
            }
        }

        // JSON → IRequest Mapping
        private IRequest? ParseJsonToRequest(string json)
        {
            using var doc = JsonDocument.Parse(json);
            string type = doc.RootElement.GetProperty("type").GetString()!;

            IRequest? request = type switch
            {
                "login" => JsonSerializer.Deserialize<ServerLoginRequest>(json),
                "register" => JsonSerializer.Deserialize<ServerRegisterRequest>(json),
                "connect" => JsonSerializer.Deserialize<ServerConnectClientRequest>(json),
                "message" => JsonSerializer.Deserialize<Delegates.ServerMessageRequest>(json),
                _ => null
            };

            if (request != null)
                request.json = json;

            return request;
        }
    }
    class ServerConnectRequest : IRequest
    {

        public string json { get; set; }

        public void Execute()
        {
            Console.WriteLine("Server connected.");
        }
    }
    class ServerMessageRequest : IRequest, ITcpServerRequest
    {
        public TcpClient client { get; set; }

        public string json { get; set; }
        public ClientTCPConnectedRequest clientTCPConnectedRequest { get; set; }
        public Delegates.ServerMessageRequest clientMessageRequest { get; set; }
        public void Execute()
        {

            //DataBaseHelper.GetUserId(clientMessageRequest.username);
            try
            {
                for (int i = 0; i < clientTCPConnectedRequest.server.connectedClients.Count; i++)
                {
                    using var doc = JsonDocument.Parse(clientMessageRequest.json);
                    string username = doc.RootElement.GetProperty("receiverId").GetString()!;
                    string userId = DataBaseHelper.GetUserId(username);
                    if (userId == clientTCPConnectedRequest.server.connectedClients[i].user.userId)
                    {
                        ClientTCPConnectedRequest targetClient = clientTCPConnectedRequest.server.connectedClients[i];
                        targetClient.streamWriter.WriteLine(clientMessageRequest.json);
                        Message message = JsonSerializer.Deserialize<Message>(clientMessageRequest.json)!;
                        MessageDataBase messageDataBase = new MessageDataBase(message);
                    }


                }
                //User user = clientTCPConnectedRequest.server.connectedClients[i].user;


                //if (targetClient != clientTCPConnectedRequest)
                //{
                //    targetClient.streamWriter.WriteLine(clientMessageRequest.json);

                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fehler beim Senden der Nachricht: " + ex.Message);

            }
        }
    }

}
