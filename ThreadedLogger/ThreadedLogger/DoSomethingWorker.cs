using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadedLogger
{
    public class DoSomethingWorker : IDoSomethingWorker
    {
        private CancellationTokenSource _cancellationTokenSource;
        private EventWaitHandle _waitHandle;
        private TaskFactory _taskFactory;

        private IThreadedLogger _threadedLogger;

        private int _calls;

        public DoSomethingWorker(IThreadedLogger logger)
        {
            _cancellationTokenSource = new ();
            _waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
            _taskFactory = new TaskFactory();
            _threadedLogger = logger;
            _calls = 0;
        }

        public void Begin()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _ = _taskFactory.StartNew(() => LogSomething(), _cancellationTokenSource.Token);
        }

        public void End()
        {
            _cancellationTokenSource.Cancel();
        }

        private void LogSomething()
        {
            while (true)
            {
                Thread.Sleep(Random.Shared.Next(100, 2000));
                ++_calls;
                _threadedLogger.QueryLog(string.Format("A Log Message From {0} the {1} time!", this.GetHashCode(), _calls));
            }
        }
    }
}
