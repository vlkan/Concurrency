namespace ConcurrencyDeepDive.AsyncAwaitInternals;

public static class AwaitExamples
{
    public static async Task TestSleepVsDelayAsync()
    {
        Console.WriteLine($"[1] Metoda Giren Thread ID: {Environment.CurrentManagedThreadId}");

        // Kötü Yol (Senkron bekleme - Thread'i dondurur)
        Thread.Sleep(2000);
        Console.WriteLine($"[2] Sleep Sonrası Thread ID: {Environment.CurrentManagedThreadId} (Aynı kaldı!)");

        Console.WriteLine("---");

        // İyi Yol (Asenkron bekleme - Thread'i serbest bırakır)
        await Task.Delay(2000);

        Console.WriteLine($"[3] Delay Sonrası Yeni Thread ID: {Environment.CurrentManagedThreadId} (Büyük ihtimalle değişti!)");
    }

    public static async Task TestNetworkIoAsync()
    {
        Console.WriteLine($"[A] İstek Hazırlanıyor... Thread ID: {Environment.CurrentManagedThreadId}");

        using var client = new HttpClient();

        // Metot burada durur, Thread havuza döner. Donanımdan cevap beklenir.
        var sonuc = await client.GetStringAsync("https://jsonplaceholder.typicode.com/posts/1");

        Console.WriteLine($"[B] İstek Tamamlandı! Gelen Veri Uzunluğu: {sonuc.Length}");
        Console.WriteLine($"[C] Metodu Bitiren Thread ID: {Environment.CurrentManagedThreadId}");
    }

    public static async Task TestTaskCompletionSourceAsync()
    {
        var tcs = new TaskCompletionSource<string>();
        var timer = new System.Timers.Timer(3000);

        timer.Elapsed += (sender, args) =>
        {
            Console.WriteLine($"\n[Zamanlayıcı] Dış olay tetiklendi. Veri gönderiliyor...");
            tcs.SetResult("Dış Dünyadan Gelen Gizli Veri");
            timer.Dispose();
        };
        timer.Start();

        Console.WriteLine($"[Metot] Beklemeye geçiliyor... Thread ID: {Environment.CurrentManagedThreadId}");

        // Metot burada donar, CPU %0 kullanımdadır, hiçbir Thread beklemez.
        var gelenVeri = await tcs.Task;

        Console.WriteLine($"[Metot] Uyandı! Veri: {gelenVeri}");
        Console.WriteLine($"[Metot] Bitiren Thread ID: {Environment.CurrentManagedThreadId}");
    }
}
