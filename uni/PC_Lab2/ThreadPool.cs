class ThreadPool : IDisposable
{
    private const int _threadsCount = 6;

    private ThreadObject[] _threadObjects = new ThreadObject[_threadsCount];
    private Thread[] _threads = new Thread[_threadsCount];
    private Mutex _mutex = new Mutex();
    private CountdownEvent _terminationCountdown = new CountdownEvent(_threadsCount);
    private ManualResetEvent _sleepMrse = new ManualResetEvent(true);
    private bool _isTerminated = false;
    private int _discardedTasks = 0;
    private int _totalTasks = 0;
    private bool _showLogs = false;

    public ThreadPool(bool showLogs = false)
    {
        for (var i = 0; i < _threadsCount; i++) 
        {
            _threadObjects[i] = new ThreadObject(_terminationCountdown, _sleepMrse, showLogs);
            _showLogs = showLogs;
            _threads[i] = new Thread(_threadObjects[i].ThreadProc);
            _threads[i].Start();
        }
    }

    public void AddTask(ThreadStart task)
    {
        try
        {
            _mutex.WaitOne();
        } catch (ObjectDisposedException)
        {
            throw new ObjectDisposedException("ThreadPool");
        }

        if (_isTerminated)
        {
            return;
        }

        var discarded = true;
        for (var i = 0; i < _threadsCount; i++)
        {
            if (_threadObjects[i].AddTask(task)) 
            {
                discarded = false;
                break;
            } 
        }

        if (discarded) {
            _discardedTasks += 1;
        }

        _totalTasks += 1;

        _mutex.ReleaseMutex();
    }

    public void Resume() 
    {
        try
        {
            _mutex.WaitOne();
        } catch (ObjectDisposedException)
        {
            throw new ObjectDisposedException("ThreadPool");
        }

        if (_isTerminated)
        {
            return;
        }

        _sleepMrse.Set();

        _mutex.ReleaseMutex();
    }

    public void Stop()
    {
        try
        {
            _mutex.WaitOne();
        } catch (ObjectDisposedException)
        {
            throw new ObjectDisposedException("ThreadPool");
        }

        if (_isTerminated)
        {
            return;
        }

        _sleepMrse.Reset();
        
        foreach (var threadObject in _threadObjects)
        {
            threadObject.SignalQueue();
        }

        _mutex.ReleaseMutex();
    }

    public void Terminate()
    {
        try
        {
            _mutex.WaitOne();
        } catch (ObjectDisposedException)
        {
            throw new ObjectDisposedException("ThreadPool");
        }

        if (_isTerminated)
        {
            return;
        }

        foreach (var threadObject in _threadObjects)
        {
            threadObject.Terminate();
        }

        _terminationCountdown.Wait();

        if (_showLogs)
        {
            long meanWaitTime = 0;
            long meanSleepTime = 0;
            long meanTerminationTime = 0;
            long meanWorkTime = 0;

            foreach (var threadObject in _threadObjects)
            {
                meanWaitTime += threadObject.SwWait!.ElapsedMilliseconds;
                meanSleepTime += threadObject.SwSleep!.ElapsedMilliseconds;
                meanTerminationTime += threadObject.SwTermination!.ElapsedMilliseconds;
                meanWorkTime += threadObject.SwWork!.ElapsedMilliseconds / threadObject.TaskCounter;
            }

            meanWaitTime /= _threadsCount;
            meanSleepTime /= _threadsCount;
            meanTerminationTime /= _threadsCount;
            meanWorkTime /= _threadsCount;
            
            Console.WriteLine("ThreadPool terminated.");
            Console.WriteLine($"Mean wait time: {meanWaitTime}");
            Console.WriteLine($"Mean sleep time: {meanSleepTime}");
            Console.WriteLine($"Mean termination time: {meanTerminationTime}");
            Console.WriteLine($"Mean work time: {meanWorkTime}");
            Console.WriteLine($"Count of discarded tasks: {_discardedTasks}/{_totalTasks}");
        }

        _isTerminated = true;

        _mutex.ReleaseMutex();
    }

    public void Dispose()
    {
        try
        {
            _mutex.WaitOne();
        } catch (ObjectDisposedException)
        {
            return;
        }

        if (!_isTerminated)
        {
            Terminate();
        }

        _terminationCountdown.Dispose();
        _sleepMrse.Dispose();

        foreach (var threadObject in _threadObjects)
        {
            threadObject.Dispose();
        }

        _mutex.ReleaseMutex();
        _mutex.Dispose();
    }
}
