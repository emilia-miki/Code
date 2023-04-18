class TaskQueue : IDisposable
{
    private ThreadStart? _task = null!;
    private ManualResetEvent _available = new ManualResetEvent(false);
    private bool _busy = false;
    private bool _isDisposed = false;
    private bool _showLogs = false;

    public TaskQueue(bool showLogs = false)
    {
        _showLogs = showLogs;
    }

    public bool AddTask(ThreadStart task)
    {
        if (_isDisposed)
        {
            throw new ObjectDisposedException("TaskQueue");
        }

        if (_busy)
        {
            if (_showLogs)
            {
                Console.WriteLine("task discarded");
            }
            
            return false;
        }

        _task = task;

        if (_showLogs)
        {
            Console.WriteLine("task added");
        }

        _available.Set();
        _available.Reset();
        return true;
    }

    public ThreadStart GetTask()
    {
        if (_isDisposed)
        {
            throw new ObjectDisposedException("TaskQueue");
        }

        _available.WaitOne();
        _busy = true;

        return _task!;
    }

    public void AcceptTasks()
    {
        if (_isDisposed)
        {
            throw new ObjectDisposedException("TaskQueue");
        }

        _busy = false;
    }

    public void Dispose() 
    {
        if (_isDisposed)
        {
            return;
        }

        _isDisposed = true;
        _available.Dispose();
    }
}
