using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThreadPoolNamespace;
using Microsoft.Data.Sqlite;

namespace ServerNamespace
{
    class Server
    {

        private readonly ThreadPoolNamespace.ThreadPool _threadPool;
        public Server(int workerCount)
        {
            _threadPool = new ThreadPoolNamespace.ThreadPool(workerCount);
        }
        public void HandleClientRequest(Action requestHandler)
        {
            _threadPool.QueueWorkItem(requestHandler);
        }
        public void ConnectToDatabase()
        {
           
        }

    }
}
