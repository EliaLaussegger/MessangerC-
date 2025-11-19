using ObserverNamespace;
using ServerNamespace;
using DataBank;
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
        centralUserDB.RegisterUser("MaxMustermann", "ajflajl", new DateTime(1990, 1, 1), "password");
        centralUserDB.RegisterUser("MaxMustermann2", "ajflajl", new DateTime(1990, 1, 1), "password");

    }
}