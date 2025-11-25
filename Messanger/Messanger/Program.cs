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
        UserFunctions userFunction = new UserFunctions();
        var server = new Server(4);
        server.ConnectToDatabase();
        CentralUserDB centralUserDB = new CentralUserDB();
        handler.NotifyObservers(new ClientRequest());
        //User user = userFunction.CreateUser();
        //centralUserDB.TestRegistration(user.username);
        User loggedInUser = userFunction.LoginUser();

    }
}