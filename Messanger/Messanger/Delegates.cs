using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates
{

    public interface IDelegate
    {
        void Invoke();
    }
    public interface IRequest
    {
        void Execute();
    }
    class ClientRequest : IRequest
    {
        public void Execute()
        {
            Console.WriteLine("Client Request Executed");
        }
    }
}
