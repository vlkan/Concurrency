using MyConcurrency;
using System.Threading.Tasks;

internal class Program
{
    private static async Task Main(string[] args)
    {
        await Example_6();
        //Console.ReadLine();
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
}