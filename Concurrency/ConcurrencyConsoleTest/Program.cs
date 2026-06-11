using MyConcurrency;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        MyThreadPool.QueueUserWorkItem(() =>
        {
            Console.WriteLine("--");
        });

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
}