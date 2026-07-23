using ConcurrencyDeepDive.AsyncAwaitInternals;

internal class Program
{
    private static async Task Main(string[] args)
    {
        Console.WriteLine($"[Main] Program Başladı. Ana Thread ID: {Environment.CurrentManagedThreadId}\n");

        //Console.WriteLine("=== 1. SEVİYE: Sleep vs Delay ===");
        //await AwaitExamples.TestSleepVsDelayAsync();

        //Console.WriteLine("\n=== 2. SEVİYE: Gerçek Ağ İsteği (I/O) ===");
        //await AwaitExamples.TestNetworkIoAsync();

        Console.WriteLine("\n=== 3. SEVİYE: TaskCompletionSource ===");
        await AwaitExamples.TestTaskCompletionSourceAsync();
    }
}