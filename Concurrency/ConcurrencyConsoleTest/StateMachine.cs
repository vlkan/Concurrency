using System.Runtime.CompilerServices;

namespace ConcurrencyConsoleTest;

public struct SimpleAwaiter : INotifyCompletion
{
    private readonly Task task;

    public SimpleAwaiter(Task task)
    {
        this.task = task;
    }

    public bool IsCompleted => task.IsCompleted;

    public void OnCompleted(Action continuation)
    {
        task.ContinueWith(_ => continuation());
    }

    public void GetResult()
    {
        task.Wait();
    }
}


public struct SimpleStateMachine : IAsyncStateMachine
{
    public int state;
    public AsyncTaskMethodBuilder methodBuilder;
    public SimpleAwaiter awaiter;

    /*
     await Task.Delay(1000);
     Console.WriteLine("Task completed inside state machine.");
     Console.WriteLine("Task completed inside state machine.");
     Console.WriteLine("Task completed inside state machine.");
     Console.WriteLine("Task completed inside state machine.");
     */

    public void MoveNext()
    {
        try
        {
            if (state == 0) // not completed yet
            {
                awaiter = new SimpleAwaiter(Task.Delay(1000));
                state = 1;

                methodBuilder.AwaitOnCompleted(ref awaiter, ref this);
                return;
            }

            if (state == 1) // initialized and completed
            {
                awaiter.GetResult();
                // doSomething()
                Console.WriteLine("Task completed inside state machine.");
            }
        }
        catch (Exception ex)
        {
            methodBuilder.SetException(ex);
            return;
        }

        methodBuilder.SetResult(); // set the result
    }

    public void SetStateMachine(IAsyncStateMachine stateMachine)
    {
        methodBuilder.SetStateMachine(stateMachine);
    }
}
