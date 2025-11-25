using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Delegates;
namespace ObserverNamespace
{
    public interface IObserver<T> where T : IRequest
    {
        void Update(T request);
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
        public void NotifyObservers<T>(T request) where T : IRequest
        {
            foreach (var observer in _observers.OfType<IObserver<T>>())
            {
                observer.Update(request);
            }
        }
    }
    class ClientConnect : IObserver<ClientConnectRequest>
    {
        public void Update(ClientConnectRequest request)
        {
            Console.WriteLine("Client connected.");
        }
    }
}
