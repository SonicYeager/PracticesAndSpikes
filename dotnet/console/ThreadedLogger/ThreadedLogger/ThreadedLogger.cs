using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadedLogger
{
    public class ThreadedLogger : IThreadedLogger
    {
        private EventWaitHandle _waitHandle;
        private Mutex _mutex;
        private TaskFactory _taskFactory;

        private Queue<string> _logMessages;

        public ThreadedLogger()
        {
            _taskFactory = new TaskFactory();
            _waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
            _mutex = new Mutex();
            _logMessages = new Queue<string>();

            _ = _taskFactory.StartNew(() => Log());
        }

        public void QueryLog(string message)
        {
            _mutex.WaitOne();
            _logMessages.Enqueue(message);
            _mutex.ReleaseMutex();
            _waitHandle.Set();
        }

        private void Log()
        {
            while(true)
            {
                if (_logMessages.Count > 0)
                {
                    _mutex.WaitOne();
                    Console.WriteLine(string.Format("ThreadedLogger: {0} | Items in Queue: {1}", _logMessages.Dequeue(), _logMessages.Count));
                    _mutex.ReleaseMutex();
                }
                else
                    _waitHandle.WaitOne();
            }
        }
    }
}
