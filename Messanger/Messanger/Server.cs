using Delegates;
using Microsoft.Data.Sqlite;
using ObserverNamespace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ThreadPoolNamespace;
using UserNamespace;
using System.Text.Json;
namespace ServerNamespace
{
    class Server
    {

        private readonly ThreadPoolNamespace.ThreadPool _threadPool;
        private TcpListener? _listener;
        private readonly ClientRequestHandler _requestHandler;
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
        private async Task AcceptLoop()
        {
            while (true)
            {
                TcpClient client = await _listener!.AcceptTcpClientAsync();
                Console.WriteLine("Client connected via TCP.");

                _threadPool.QueueWorkItem(new ClientTCPConnectedRequest(client, _requestHandler));
            }
        }

    }
    class ClientTCPConnectedRequest : IRequest
    {
        private readonly TcpClient _client;
        private readonly ClientRequestHandler _handler;
        public string json { get; set; }


        public ClientTCPConnectedRequest(TcpClient client, ClientRequestHandler handler)
        {
            _client = client;
            _handler = handler;
        }

        public void Execute()
        {
            using var stream = _client.GetStream();
            using var reader = new StreamReader(stream, Encoding.UTF8);

            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                //Console.WriteLine("Received JSON: " + line);

                IRequest? request = ParseJsonToRequest(line);

                switch (request)
                {
                    case ClientLoginRequest clr:
                        _handler.NotifyObservers(clr);
                        break;
                    case ClientRegisterRequest crr:
                        _handler.NotifyObservers(crr);
                        break;
                    case ClientConnectRequest ccr:
                        _handler.NotifyObservers(ccr);
                        break;
                    case ClientMessageRequest cmr:
                        _handler.NotifyObservers(cmr);
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
                "login" => JsonSerializer.Deserialize<ClientLoginRequest>(json),
                "register" => JsonSerializer.Deserialize<ClientRegisterRequest>(json),
                "connect" => JsonSerializer.Deserialize<ClientConnectRequest>(json),
                "message" => JsonSerializer.Deserialize<ClientMessageRequest>(json),
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
}
