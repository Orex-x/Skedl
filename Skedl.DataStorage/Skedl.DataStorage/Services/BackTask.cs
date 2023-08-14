namespace Skedl.DataStorage.Services;

public class BackTask
{
    private readonly CancellationTokenSource _cancellationTokenSource;
    private Task? _backgroundTask;

    public BackTask()
    {
        _cancellationTokenSource = new CancellationTokenSource();
    }

    public void Start(Action action)
    {
        _backgroundTask = Task.Run(action);
    }

    public void Stop()
    {
        if(_backgroundTask == null) return;
        
        _cancellationTokenSource.Cancel();
        try
        {
            _backgroundTask.Wait();
        }
        catch (AggregateException)
        {
        }
    }
}