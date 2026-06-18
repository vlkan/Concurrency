using System.Runtime.CompilerServices;

namespace MyConcurrency
{
    class MyTaskResult<TResult> : MyTask
    {
        public TResult? Result { get; private set; }

        public void SetResult(TResult result)
        {
            Result = result;
            base.SetResult();
        }

        public new Awaiter GetAwaiter() => new(this);

        public new struct Awaiter(MyTaskResult<TResult> t) : INotifyCompletion
        {
            public readonly bool IsCompleted => t.IsCompleted;

            public readonly TResult GetResult()
            {
                t.Wait();
                return t.Result!;
            }

            public readonly void OnCompleted(Action continuation) => t.ContinueWith(continuation);
        }
    }
}
