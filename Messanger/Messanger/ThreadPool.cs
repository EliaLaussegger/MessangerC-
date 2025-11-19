using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadPoolNamespace
{
    public class ThreadPool
    {
        private readonly List<Thread> workers;
        private readonly object _lock = new object();
        private readonly Queue<Action> _taskQueue = new Queue<Action>();
        private bool _running = true;
        public ThreadPool(int workerCount)
        {
            workers = new List<Thread>();

            for (int i = 0; i < workerCount; i++)
            {
                Thread t = new Thread(Work);
                t.IsBackground = true;
                t.Start();
                workers.Add(t);
            }
        }
        public void QueueWorkItem(Action task)
        {
            lock (_lock)
            {
                _taskQueue.Enqueue(task);
                Monitor.Pulse(_lock);
            }
        }
        private void Work()
        {
            while (true)
            {
                Action? task = null;

                lock (_lock)
                {
                    while (_taskQueue.Count == 0 && _running)
                        Monitor.Wait(_lock);

                    if (!_running && _taskQueue.Count == 0)
                        return;

                    task = _taskQueue.Dequeue();
                }

                try
                {
                    task?.Invoke();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Fehler in Task: {ex.Message}");
                }
            }
        }
    }
}
