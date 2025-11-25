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


        var server = new Server(4);
        server.ConnectToDatabase();


        UserFunctions userFunction = new UserFunctions();
        CentralUserDB centralUserDB = new CentralUserDB();
        handler.NotifyObservers(new ClientConnectRequest());
        //User user = userFunction.CreateUser();
        //centralUserDB.TestRegistration(user.username);
        User loggedInUser = userFunction.LoginUser();

    }
}