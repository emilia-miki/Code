// a pool of 6 threads
// if all threads are busy the task is discarded
// a task has a random length of 8-12s
// can realize 6 threads of queues of tasks

var threadPool = new ThreadPool(showLogs: true);

var rand = new Random();
var testDuration = 120000;
var countdown = testDuration;
int duration;
while (countdown > testDuration / 2)
{
    threadPool.AddTask(Task);
    duration = rand.Next(4000);
    countdown -= duration;
    Thread.Sleep(duration);
}

Console.WriteLine("Stopping the thread pool");
threadPool.Stop();

while (countdown > testDuration / 4)
{
    threadPool.AddTask(Task);
    duration = rand.Next(4000);
    countdown -= duration;
    Thread.Sleep(duration);
}

Console.WriteLine("Resuming the thread pool");
threadPool.Resume();

while (countdown > testDuration / 10)
{
    threadPool.AddTask(Task);
    duration = rand.Next(4000);
    countdown -= duration;
    Thread.Sleep(duration);
}

Console.WriteLine("Terminating the thread pool");
threadPool.Terminate();
threadPool.Dispose();

static void Task()
{
    var rand = new Random();
    var duration = 8000 + rand.Next(4001);
    Console.WriteLine($"task began ({duration} ms)");
    Thread.Sleep(duration);
    Console.WriteLine("task ended");
}
