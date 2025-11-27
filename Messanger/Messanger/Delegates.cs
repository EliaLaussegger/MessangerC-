using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserNamespace;
using DataBank;
namespace Delegates
{
    public interface IServerEvent : IRequest { }
    public interface IDelegate
    {
        void Invoke();
    }
    public interface IRequest
    {
        void Execute();
    }
    class ClientConnectRequest : IRequest
    {
        public void Execute()
        {
            Console.WriteLine("Client Request Executed");
        }
    }
    class ClientLoginRequest : IRequest
    {
        public User user { get; protected set; }
        public void Execute()
        {
            user = UserFunctions.LoginUser();


        }
    }
    class ClientRegisterRequest : IRequest
    {
        public User user { get; protected set; }
        public void Execute()
        {
            user = UserFunctions.CreateUser();
        }
    }
}
