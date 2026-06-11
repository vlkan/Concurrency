namespace MyConcurrency
{
    public class MyTask
    {
        private bool completed;
        private Exception? exception;
        private Action? continuation;
        private ExecutionContext? executionContext;

        public void SetResult() => Complete(null);

        public void SetException(Exception exception) => Complete(exception);

        public void Complete(Exception? exception)
        {
            if (completed) return;

            completed = true;
            this.exception = exception;

            if (continuation is not null)
            {
                MyThreadPool.QueueUserWorkItem(() =>
                {
                    if (executionContext != null)
                    {
                        ExecutionContext.Run(executionContext, (object? state) => ((Action)state!).Invoke(), continuation);
                    }
                    else
                    {
                        continuation();
                    }
                });
            }
        }

        public void Wait()
        {

        }
    }
}
