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
        handler.RegisterObserver(new ClientLogin());


        var server = new Server(4);
        server.ConnectToDatabase();


        UserFunctions userFunction = new UserFunctions();
        CentralUserDB centralUserDB = new CentralUserDB();
        handler.NotifyObservers(new ClientConnectRequest());
        //User user = userFunction.CreateUser();
        //centralUserDB.TestRegistration(user.username);
        ClientLoginRequest request = new ClientLoginRequest();
        List<ObserverNamespace.IObserver<ClientLoginRequest>> updatedObservers = handler.NotifyObservers(request);

        // Beispiel: ersten Observer
        ClientLogin clientLogin = (ClientLogin)updatedObservers[0];
        User loggedInUser = clientLogin.request.user;

    }
}