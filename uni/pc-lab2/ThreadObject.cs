using System.Diagnostics;

class ThreadObject : IDisposable
{
    public Stopwatch? SwSleep { get; }
    public Stopwatch? SwWait { get; }
    public Stopwatch? SwWork { get; }
    public Stopwatch? SwTermination { get; }
    public int TaskCounter { get => _taskCounter; }

    private TaskQueue _queue;
    private bool _showLogs;
    private int _taskCounter = 0;
    private CountdownEvent _cde;
    private ManualResetEvent _mrse;
    private bool _isTerminated = false;
    private bool _isDisposed = false;
    private ThreadStart _emptyTask = () => {};

    public ThreadObject(CountdownEvent cde, ManualResetEvent mrse, bool showLogs = false) 
    {
        _cde = cde;
        _mrse = mrse;
        _showLogs = showLogs;
        _queue = new TaskQueue(showLogs);

        if (showLogs)
        {
            SwSleep = new Stopwatch();
            SwWork = new Stopwatch();
            SwTermination = new Stopwatch();
            SwWait = new Stopwatch();
        }
    }

    public void ThreadProc()
    {
        if (_isDisposed)
        {
            throw new ObjectDisposedException("ThreadObject");
        }
        
        while (!_isTerminated)
        {
            SwWait?.Start();
            var task = _queue.GetTask();
            SwWait?.Stop();

            if (task != _emptyTask)
            {
                _taskCounter += 1;
            }
            else if (_showLogs)
            {
                Console.WriteLine("empty task received by worker");
            }

            SwWork?.Start();
            task();
            SwWork?.Stop();

            SwSleep?.Start();
            _mrse.WaitOne();
            SwSleep?.Stop();

            if (task == _emptyTask)
            {
                Console.WriteLine("accepting new tasks after processing empty task " +
                              "and waiting for resume signal");
            }

            _queue.AcceptTasks();
        }

        _cde.Signal();
        SwTermination?.Stop();

        if (_showLogs)
        {
            Console.WriteLine("thread terminated");
        }
    }

    public bool AddTask(ThreadStart task)
    {
        return _queue.AddTask(task);
    }

    public void SignalQueue()
    {
        _queue.AddTask(_emptyTask);
    }

    public void Terminate()
    {
        if (_isDisposed)
        {
            throw new ObjectDisposedException("ThreadObject");
        }

        if (_isTerminated)
        {
            return;
        }

        SwTermination?.Start();
        _isTerminated = true;
        SignalQueue();
    }

    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        _queue.Dispose();
        _isDisposed = true;
    }
}
