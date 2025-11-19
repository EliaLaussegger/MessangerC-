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

        handler.NotifyObservers(() => Console.WriteLine("Request ausgeführt"));
        var server = new Server(4);
        server.ConnectToDatabase();
        CentralUserDB centralUserDB = new CentralUserDB();
        User max = new User("MaxMustermann", "ajflajl", new DateTime(1990, 1, 1), "password");
        User max2 = new User("MaxMustermann2", "ajflajl", new DateTime(1990, 1, 1), "password");
        centralUserDB.RegisterUser(max);
        centralUserDB.RegisterUser(max2);

    }
}