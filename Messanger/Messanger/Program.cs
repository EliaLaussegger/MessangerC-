using ObserverNamespace;
using ServerNamespace;
using DataBank;
using UserNamespace;
class Program
{
    static void Main(string[] args)
    {
        var handler = new ClientRequestHandler();
        handler.RegisterObserver(new ClientConnect());
        UserFunctions userFunction = new UserFunctions();
        handler.NotifyObservers(() => Console.WriteLine("Request ausgeführt"));
        var server = new Server(4);
        server.ConnectToDatabase();
        CentralUserDB centralUserDB = new CentralUserDB();
        User user = userFunction.CreateUser();
        centralUserDB.TestRegistration(user.username);

    }
}