using DataBank;
using Delegates;
using ObserverNamespace;
using ServerNamespace;
using UserNamespace;
using ClientNamespace;
using Helper;
class Program
{
    static void Main(string[] args)
    {
        ClientRequestHandler handler = new ClientRequestHandler();
        handler.RegisterObserver(new ClientLoginObserver());
        handler.RegisterObserver(new ClientRegisterObserver());
        handler.RegisterObserver(new ClientConnect());
        handler.RegisterObserver(new ClientMessageObserver());
        Server server = new Server(workerCount: 4, requestHandler: handler);

        server.Start(3000);

        //var client = new TcpJsonClient("127.0.0.1", 3000);

        //client.SendRequest(new LoginSendModel
        //{
        //    username = "asdf",
        //    password = "penasdfis"
        //});
        Console.ReadLine();

        //handler.NotifyObservers(new ClientConnectRequest());
        ////User user = userFunction.CreateUser();
        ////centralUserDB.TestRegistration(user.username);
        //ClientLoginRequest clientLoginRequest = new ClientLoginRequest();
        //ClientRegisterRequest clientRegisterRequest = new ClientRegisterRequest();
        ////List<ObserverNamespace.IObserver<ClientLoginRequest>> updatedObservers = handler.NotifyObservers(clientLoginRequest);
        //List<ObserverNamespace.IObserver<ClientRegisterRequest>> updatedObservers = handler.NotifyObservers(clientRegisterRequest);

        //// Beispiel: ersten Observer
        //ClientRegisterObserver clientLogin = (ClientRegisterObserver)updatedObservers[0];
        ////ClientLoginObserver clientLogin = (ClientLoginObserver)updatedObservers[0];
        //User loggedInUser = clientLogin.request.user;

    }
}