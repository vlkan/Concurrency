using System.Collections.Concurrent;

namespace MyConcurrency
{
    public class MyThreadPool
    {
        private static readonly BlockingCollection<(Action, ExecutionContext?)> actions = [];

        //shallow copy yapar, yani action içindeki referans tipler aynı kalır, sadece action'ın kendisi kopyalanır.
        public static void QueueUserWorkItem(Action action) => actions.Add((action, ExecutionContext.Capture()));

        static MyThreadPool()
        {
            for (int i = 0; i < Environment.ProcessorCount; i++)
            {
                var thread = new Thread(() =>
                {
                    while (true)
                    {
                        //BlockingCollection a yeni bi action eklenene kadar burada bekler.
                        var (action, context) = actions.Take();
                        if (context != null)
                        {
                            action();
                        }
                        else
                            ExecutionContext.Run(context, (object? state) => ((Action)state!).Invoke(), action);
                    }
                });
                thread.IsBackground = true;
                thread.Start();
            }
        }
    }
}
