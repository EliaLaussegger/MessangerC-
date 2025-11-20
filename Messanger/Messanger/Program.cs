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
        userFunction.CreateUser();
        User max2 = new User("MaxMustermann2", "ajflajl", new DateTime(1990, 1, 1), "password");
        centralUserDB.RegisterUser(max2);

    }
}