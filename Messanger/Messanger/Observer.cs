using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Delegates;
namespace ObserverNamespace
{
    interface IObserver
    {
        void Update(IRequest request);
    }
    class ClientRequestHandler
    {
        private readonly List<IObserver> _observers = new List<IObserver>();
        public void RegisterObserver(IObserver observer)
        {
            _observers.Add(observer);
        }
        public void UnregisterObserver(IObserver observer)
        {
            _observers.Remove(observer);
        }
        public void NotifyObservers(IRequest request)
        {
            foreach (var observer in _observers)
            {
                observer.Update(request);
            }
        }
    }
    class ClientConnect : IObserver
    {
        public void Update(IRequest request)
        {
            Console.WriteLine("Client tries Connect");
        }
    }
}
