using ClientNamespace;
using DataBank;
using JsonParser;
using ServerNamespace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
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
    public interface ITcpClientRequest
    {
        TcpClient client { get; set; }
        ClientTCPConnectedRequest clientTCPConnectedRequest { get; set; }
    }
    class ServerConnectClientRequest : IRequest
    {
        public ClientTCPConnectedRequest clientTCPConnectedRequest { get; set; }

        public string json { get; set; }
        public void Execute()
        {
            Console.WriteLine("Client Request Executed");
        }
    }
    class ServerLoginRequest : IRequest , ITcpClientRequest
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
            for (int i = 0; i < server.connectedClients.Count; i++)
            {
                if (server.connectedClients[i]._client == client)
                {
                    server.connectedClients[i].user = user;
                    UserSerializable jsonUser = new UserSerializable
                    {
                        username = user.username,
                        email = user.email,
                        userId = user.userId,
                        dateOfBirth = user.dateOfBirth
                    };
                    server.connectedClients[i].streamWriter.WriteLine(JsonSerializer.Serialize(new LoginResponseModel { message = "Logged in", status = "sucess", user = jsonUser }));
                    Console.WriteLine(JsonSerializer.Serialize(new LoginResponseModel { message = "Logged in", status = "sucess", user = jsonUser }));
                }

            }



        }
    }
    class ServerRegisterRequest : IRequest , ITcpClientRequest
    {
        public TcpClient client { get; set; }

        public ClientTCPConnectedRequest clientTCPConnectedRequest { get; set; }

        public string json { get; set; }
        public User user { get; protected set; }
        public void Execute()
        {
            user = UserFunctions.CreateUser();
        }
    }
    class ServerMessageRequest : IRequest, ITcpClientRequest
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
            Console.WriteLine($"Message from {username} to {receiver}: {message}");
            

        }
    }
}
