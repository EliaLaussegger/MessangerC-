using System;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ClientNamespace
{
    public class TcpJsonClient : IDisposable
    {
        private TcpClient _client;
        private NetworkStream _stream;
        private StreamReader _reader;
        private StreamWriter _writer;

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

                    //Console.WriteLine("SERVER -> CLIENT: " + line);
                }
            }
            catch
            {
                Console.WriteLine("Disconnected.");
            }
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
    }
}
