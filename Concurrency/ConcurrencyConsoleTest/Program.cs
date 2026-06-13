using MyConcurrency;

internal class Program
{
    private static void Main(string[] args)
    {
        Example_2();

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
}