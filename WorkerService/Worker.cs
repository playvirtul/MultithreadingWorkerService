namespace WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        private object _lock = new object();
        private AutoResetEvent _waitHandler = new AutoResetEvent(true);
        private Mutex _mutexObj = new();
        private Semaphore _semaphore = new Semaphore(1, 1);

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                File.WriteAllText("ThreadLockTextFile.txt", string.Empty);
                File.WriteAllText("TaskLockTextFile.txt", string.Empty);
                File.WriteAllText("ThreadAutoResetEventTextFile.txt", string.Empty);
                File.WriteAllText("TaskAutoResetEventTextFile.txt", string.Empty);
                File.WriteAllText("ThreadMutexTextFile.txt", string.Empty);
                File.WriteAllText("TaskMutexTextFile.txt", string.Empty);
                File.WriteAllText("ThreadSemaphoreTextFile.txt", string.Empty);
                File.WriteAllText("TaskSemaphoreTextFile.txt", string.Empty);

                StartThreadLock();
                StartTaskLock();
                StartThreadAutoResetEvent();
                StartTaskAutoResetEvent();
                StartThreadMutex();
                StartTaskMutex();
                StartThreadSemaphore();
                StartTaskSemaphore();

                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(5000, stoppingToken);
            }
        }

        private void StartThreadLock()
        {
            for (int i = 0; i < 5; i++)
            {
                ThreadPool.QueueUserWorkItem(_ => ThreadLockPrintContent(_lock));
            }
        }

        private void ThreadLockPrintContent(object obj)
        {
            lock (obj)
            {
                for (int i = 0; i < 5; i++)
                {
                    File.AppendAllText("ThreadLockTextFile.txt", $"Поток - {Thread.CurrentThread.ManagedThreadId}\n");
                }
            }
        }

        private void StartTaskLock()
        {
            for (int i = 0; i < 5; i++)
            {
                Task.Run(TaskLockPrintContent);
            }
        }

        private void TaskLockPrintContent()
        {
            lock (_lock)
            {
                for (int i = 0; i < 5; i++)
                {
                    File.AppendAllText("TaskLockTextFile.txt", $"Поток - {Task.CurrentId}\n");
                }
            }
        }

        private void StartThreadMutex()
        {
            for (int i = 0; i < 5; i++)
            {
                ThreadPool.QueueUserWorkItem(_ => ThreadLockPrintMutex(_lock));
            }
        }

        private void ThreadLockPrintMutex(object obj)
        {
            _mutexObj.WaitOne();

            for (int i = 0; i < 5; i++)
            {
                File.AppendAllText("ThreadMutexTextFile.txt", $"Поток - {Thread.CurrentThread.ManagedThreadId}\n");
            }

            _mutexObj.ReleaseMutex();
        }

        private void StartTaskMutex()
        {
            for (int i = 0; i < 5; i++)
            {
                Task.Run(TaskLockPrintMutex);
            }
        }

        private void TaskLockPrintMutex()
        {
            _mutexObj.WaitOne();

            for (int i = 0; i < 5; i++)
            {
                File.AppendAllText("TaskMutexTextFile.txt", $"Поток - {Task.CurrentId}\n");
            }

            _mutexObj.ReleaseMutex();
        }

        private void StartThreadAutoResetEvent()
        {
            for (int i = 0; i < 5; i++)
            {
                ThreadPool.QueueUserWorkItem(_ => ThreadAutoResetEventPrintContent(_lock));
            }
        }

        private void ThreadAutoResetEventPrintContent(object obj)
        {
            _waitHandler.WaitOne();

            for (int i = 0; i < 5; i++)
            {
                File.AppendAllText("ThreadAutoResetEventTextFile.txt", $"Поток - {Thread.CurrentThread.ManagedThreadId}\n");
            }

            _waitHandler.Set();
        }

        private void StartTaskAutoResetEvent()
        {
            for (int i = 0; i < 5; i++)
            {
                Task.Run(TaskAutoResetEventPrintContent);
            }
        }

        private void TaskAutoResetEventPrintContent()
        {
            _waitHandler.WaitOne();

            for (int i = 0; i < 5; i++)
            {
                File.AppendAllText("TaskAutoResetEventTextFile.txt", $"Поток - {Task.CurrentId}\n");
            }

            _waitHandler.Set();
        }

        private void StartThreadSemaphore()
        {
            for (int i = 0; i < 5; i++)
            {
                ThreadPool.QueueUserWorkItem(_ => ThreadSemaphorePrintContent(_lock));
            }
        }

        private void ThreadSemaphorePrintContent(object obj)
        {
            _semaphore.WaitOne();

            for (int i = 0; i < 5; i++)
            {
                File.AppendAllText("ThreadSemaphoreTextFile.txt", $"Поток - {Thread.CurrentThread.ManagedThreadId}\n");
            }

            _semaphore.Release();
        }

        private void StartTaskSemaphore()
        {
            for (int i = 0; i < 5; i++)
            {
                Task.Run(TaskSemaphorePrintContent);
            }
        }

        private void TaskSemaphorePrintContent()
        {
            _semaphore.WaitOne();

            for (int i = 0; i < 5; i++)
            {
                File.AppendAllText("TaskSemaphoreTextFile.txt", $"Поток - {Task.CurrentId}\n");
            }

            _semaphore.Release();
        }
    }
}