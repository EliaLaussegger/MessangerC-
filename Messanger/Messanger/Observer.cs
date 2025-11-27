using Delegates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserNamespace;
namespace ObserverNamespace
{
    public interface IObserver<T> where T : IRequest
    {
        IObserver<T> Update(T request);
    }
    public class Observer<T> where T : IRequest
    {
        public T request { get; protected set; }
    }
    class ClientRequestHandler
    {
        private readonly List<object> _observers = new List<object>();
        public void RegisterObserver<T>(IObserver<T> observer) where T : IRequest
        {
            _observers.Add(observer);
        }
        public void UnregisterObserver<T>(IObserver<T> observer) where T : IRequest
        {
            _observers.Remove(observer);
        }
        public List<IObserver<T>> NotifyObservers<T>(T request) where T : IRequest
        {
            List<IObserver<T>> notifiedObservers = new List<IObserver<T>>();
            foreach (var observer in _observers.OfType<IObserver<T>>())
            {
                IObserver<T> o = observer.Update(request);
                if (o != null)
                {
                    notifiedObservers.Add(o);
                }
            }
            return notifiedObservers;
        }
    }
    class ClientConnect : Observer<ClientConnectRequest>, IObserver<ClientConnectRequest>
    {
        public IObserver<ClientConnectRequest> Update(ClientConnectRequest request)
        {
            Console.WriteLine("Client connected.");
            return this;
        }
    }
    class ClientLoginObserver : Observer<ClientLoginRequest>, IObserver<ClientLoginRequest>
    {
        public IObserver<ClientLoginRequest> Update(ClientLoginRequest request)
        {
            request.Execute();
            Console.WriteLine(request.user.username + " logged in.");
            this.request = request;
            return this;
        }
    }
    class ClientRegisterObserver : Observer<ClientRegisterRequest>, IObserver<ClientRegisterRequest>
    {

        public IObserver<ClientRegisterRequest> Update(ClientRegisterRequest request)
        {
            request.Execute();
            this.request = request;
            return this;
        }
    }

}
