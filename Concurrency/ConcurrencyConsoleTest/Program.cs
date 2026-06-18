using MyConcurrency;

internal class Program
{
    private static void Main(string[] args)
    {
        Example_8();
        Console.ReadLine();
    }
    void Example_1()
    {
        List<Task> tasks = new();
        AsyncLocal<int> local = new();
        for (int i = 0; i < 10; i++)
        {
            local.Value = i;
            var t = Task.Run(() =>
            {
                Console.WriteLine("Value: {0}", local.Value);
                Task.Delay(1000).Wait();
            });

            tasks.Add(t);
        }

        Task.WhenAll(tasks).Wait();
    }

    static void Example_2()
    {
        var t = MyTask.Run(() =>
        {
            Console.WriteLine("Hello from MyTask!");
            Thread.Sleep(3000);
            Console.WriteLine("MyTask completed.");
        });

        Console.WriteLine("Before");
        t.Wait();
        Console.WriteLine("After");
    }

    static void Example_3()
    {
        var tasks = new List<MyTask>();

        tasks.Add(MyTask.Run(() =>
        {
            Console.WriteLine("Task 1 started.");
            Thread.Sleep(1000);
            Console.WriteLine("Task 1 completed.");
        }));

        tasks.Add(MyTask.Run(() =>
        {
            Console.WriteLine("Task 2 started.");
            Thread.Sleep(2000);
            Console.WriteLine("Task 2 completed.");
        }));

        tasks.Add(MyTask.Run(() =>
        {
            Console.WriteLine("Task 3 started.");
            Thread.Sleep(5000);
            Console.WriteLine("Task 3 completed.");
        }));

        var t = MyTask.WhenAll(tasks);

        Console.WriteLine("Before");
        t.Wait();
        Console.WriteLine("After");

    }

    static void Example_4()
    {
        var t = MyTask.Delay(5000);
        Console.WriteLine("Before");
        t.Wait();
        Console.WriteLine("After");
    }

    static void Example_5()
    {
        var tasks = new List<MyTask>();

        tasks.Add(MyTask.Run(() =>
        {
            Console.WriteLine("Task 1 started.");
            Thread.Sleep(1000);
            Console.WriteLine("Task 1 completed.");
        }));

        tasks.Add(MyTask.Run(() =>
        {
            Console.WriteLine("Task 2 started.");
            Thread.Sleep(2000);
            Console.WriteLine("Task 2 completed.");
        }));

        tasks.Add(MyTask.Run(() =>
        {
            Console.WriteLine("Task 3 started.");
            Thread.Sleep(5000);
            Console.WriteLine("Task 3 completed.");
        }));

        var t = MyTask.WhenAny(tasks);

        Console.WriteLine("Before");
        t.Wait();
        Console.WriteLine("After");
    }

    static async Task Example_6()
    {
        Console.WriteLine("Before");
        await MyTask.Delay(2000);
        Console.WriteLine("After");
        //awaitable because of GetAwaiter() method in MyTask class
    }

    static async Task Example_7()
    {
        var tasks = new List<MyTask>();

        tasks.Add(MyTask.Run(() => { Thread.Sleep(3000); }));
        tasks.Add(MyTask.Run(() => { Thread.Sleep(1000); }));
        tasks.Add(MyTask.Run(() => { Thread.Sleep(5500); }));

        await foreach (var item in MyTask.WhenEach(tasks))
        {
            Console.WriteLine("Task tamamlandi");
        }
    }

    static void Example_8()
    {
        _ = MyTask.Run(() =>
        {
            Thread.Sleep(1000);
            Console.WriteLine("Main Method ");
        })
        .ContinueWith(() =>
        {
            Thread.Sleep(1000);
            Console.WriteLine("1. ContinueWith Method");
        })
        .ContinueWith(() =>
        {
            Thread.Sleep(1000);
            Console.WriteLine("2. ContinueWith Method");
        })
        .ContinueWith(() =>
         {
             Thread.Sleep(1000);
             Console.WriteLine("3. ContinueWith Method");
         })
        .ContinueWith(() =>
         {
             Thread.Sleep(1000);
             Console.WriteLine("4. ContinueWith Method");
         });
    }

    void Example_9()
    {
        List<Task> tasks = new();
        AsyncLocal<int> local = new();
        for (int i = 0; i < 10; i++)
        {
            local.Value = i;
            var t = Task.Run(() =>
            {
                Console.WriteLine("Value: {0}", local.Value);
                Task.Delay(1000).Wait();
            });

            tasks.Add(t);
        }

        Task.WhenAll(tasks).Wait();
    }

    void Example_10()
    {
        void Print(int val)
        {
            Console.WriteLine(val);
            var randomValue = Random.Shared.Next(500, 1500);
            Task.Delay(randomValue).Wait();
        }

        Task.Run(() =>
        {
            Thread.Sleep(1000);
            Console.WriteLine("Main tasks completed");
        })
        .ContinueWith((_) => Print(1))
        .ContinueWith((_) => Print(2))
        .ContinueWith((_) => Print(3))
        .ContinueWith((_) => Print(4));
    }

    async void Example_11()
    {
        var task = Task.Run(() =>
        {
            Task.Delay(1000).Wait();
        });

        await task;

        _ = task.ContinueWith((_) =>
        {
            Console.WriteLine("Continued with...");
        });



    }

    async Task Example_12()
    {
        void PrintLog(string logMessage) => Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] - {logMessage}");

        var task = Task.Run(() =>
        {
            Task.Delay(1000).Wait();
        })
        .ContinueWith((_) =>
        {
            return Task.Delay(2000);
        });

        PrintLog("Before");
        var ct = task.Result;

        await task;
        await ct;
        PrintLog("After");
    }
}