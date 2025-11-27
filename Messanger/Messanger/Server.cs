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
    }
    class ServerConnectRequest : IRequest
    {
         
        public void Execute()
        {
            Console.WriteLine("Server connected.");
        }
    }
}
