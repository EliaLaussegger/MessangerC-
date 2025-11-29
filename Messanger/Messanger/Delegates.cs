using DataBank;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using UserNamespace;
namespace Delegates
{
    public interface IServerEvent : IRequest { }
    public interface IDelegate
    {
        void Invoke();
    }
    public interface IRequest
    {
        string json { get; set; }
        void Execute();
    }
    class ClientConnectRequest : IRequest
    {
        public string json { get; set; }
        public void Execute()
        {
            Console.WriteLine("Client Request Executed");
        }
    }
    class ClientLoginRequest : IRequest
    {
        public string json { get; set; }
        public User user { get; protected set; }
        public void Execute()
        {
            using var doc = JsonDocument.Parse(json);
            string username = doc.RootElement.GetProperty("username").GetString()!;
            string password = doc.RootElement.GetProperty("password").GetString()!;
            user = UserFunctions.LoginUser(username, password);


        }
    }
    class ClientRegisterRequest : IRequest
    {
        public string json { get; set; }
        public User user { get; protected set; }
        public void Execute()
        {
            user = UserFunctions.CreateUser();
        }
    }
}
