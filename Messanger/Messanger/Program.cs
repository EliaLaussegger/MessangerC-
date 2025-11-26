using DataBank;
using Delegates;
using ObserverNamespace;
using ServerNamespace;
using UserNamespace;
class Program
{
    static void Main(string[] args)
    {
        var handler = new ClientRequestHandler();
        handler.RegisterObserver(new ClientConnect());
        handler.RegisterObserver(new ClientLoginObserver());
        handler.RegisterObserver(new ClientRegisterObserver());


        var server = new Server(4);
        server.Connect();


        CentralUserDB centralUserDB = new CentralUserDB();
        handler.NotifyObservers(new ClientConnectRequest());
        //User user = userFunction.CreateUser();
        //centralUserDB.TestRegistration(user.username);
        ClientLoginRequest clientLoginRequest = new ClientLoginRequest();
        ClientRegisterRequest clientRegisterRequest = new ClientRegisterRequest();
        List<ObserverNamespace.IObserver<ClientLoginRequest>> updatedObservers = handler.NotifyObservers(clientLoginRequest);

        // Beispiel: ersten Observer
        //ClientRegisterObserver clientLogin = (ClientRegisterObserver)updatedObservers[0];
        ClientLoginObserver clientLogin = (ClientLoginObserver)updatedObservers[0];
        User loggedInUser = clientLogin.request.user;

    }
}