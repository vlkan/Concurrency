using System.Runtime.ExceptionServices;

namespace MyConcurrency
{
    public class MyTask
    {
        private bool completed;
        private Exception? exception;
        private Action? continuation;
        private ExecutionContext? executionContext;
        private object lockObject = new();

        public bool IsCompleted => completed;
        public Exception? Exception => exception;

        public void SetResult() => Complete(null);

        public void SetException(Exception exception) => Complete(exception);

        private void Complete(Exception? exception)
        {
            lock (lockObject)
            {
                if (completed) return;

                completed = true;
                this.exception = exception;

                if (continuation is not null)
                {
                    MyThreadPool.QueueUserWorkItem(() =>
                    {
                        if (executionContext is null)
                        {
                            continuation();
                        }
                        else
                        {
                            ExecutionContext.Run(executionContext, (object? state) => ((Action)state!).Invoke(), continuation);
                        }
                    });
                }
            }
        }

        public void Wait()
        {
            ManualResetEventSlim? mre = null;

            if (!completed)
            {
                mre = new();
                //Bekle ve benim SET methodumu çağır.
                ContinueWith(() => mre.Set());
            }

            mre?.Wait();
            //Completed

            if (exception is not null) throw new AggregateException(exception);
            //throw exception tüm exception stack'i unutur ve sadece kendisini atar.
            //throw new AggregateException(exception) bu ise bütün exception stacki alıp üstüne bizikini ekler geçmiş kaybolmaz
            //ExceptionDispatchInfo.Throw(exception); bu da aynısı ama oldschool
        }

        public MyTask ContinueWith(Action action)
        {
            MyTask task = new();
            void callBack()
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    task.SetException(ex);
                    return;
                }

                task.SetResult();
            }
            lock (lockObject)
            {
                if (completed)
                {
                    MyThreadPool.QueueUserWorkItem(callBack);
                }
                else
                {
                    continuation = callBack;
                    executionContext = ExecutionContext.Capture();
                }
            }
            return task;
        }

        #region Helpers
        public static MyTask Run(Action action)
        {
            MyTask task = new();

            MyThreadPool.QueueUserWorkItem(delegate
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    task.SetException(ex);
                    return;
                }
                task.SetResult();
            });
            return task;
        }

        public static MyTask WhenAll(List<MyTask> tasks)
        {
            MyTask t = new();

            var remainingCount = tasks.Count;

            if (remainingCount == 0)
            {
                t.SetResult();
                return t;
            }

            foreach (var task in tasks)
            {
                task.ContinueWith(() =>
                {
                    if (task.Exception is not null)
                    {
                        t.SetException(task.Exception);
                        return;
                    }
                    if (Interlocked.Decrement(ref remainingCount) == 0)
                    {
                        t.SetResult();
                    }
                });
            }

            return t;
        }

        public static MyTask WhenAny(List<MyTask> tasks)
        {
            MyTask t = new();

            if (tasks.Count == 0)
            {
                t.SetResult();
                return t;
            }

            foreach (var task in tasks)
            {
                if (task.IsCompleted)
                {
                    if (task.Exception is not null)
                    {
                        t.SetException(task.Exception);
                        return t;
                    }
                    t.SetResult();
                    break;
                }

                task.ContinueWith(() =>
                {
                    if (task.Exception is not null)
                    {
                        t.SetException(task.Exception);
                        return;
                    }
                    t.SetResult();
                });
            }
            return t;
        }

        public static MyTask Delay(int milliseconds)
        {
            MyTask task = new();

            var timer = new Timer((_) =>
            {
                task.SetResult();
            });

            timer.Change(milliseconds, Timeout.Infinite);
            
            return task;
        }
        #endregion
    }
}
