using Delegates;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ThreadPoolNamespace;
namespace ServerNamespace
{
    class Server
    {

        private readonly ThreadPoolNamespace.ThreadPool _threadPool;
        public Server(int workerCount)
        {
            _threadPool = new ThreadPoolNamespace.ThreadPool(workerCount);
        }
        public void HandleClientRequest(IDelegate requestHandler)
        {
            _threadPool.QueueWorkItem(requestHandler);
        }
        public void Connect()
        {
            TcpListener listener = new TcpListener(IPAddress.Any, 5000);
            listener.Start();
            TcpClient client = listener.AcceptTcpClient();
        }
    }
}
