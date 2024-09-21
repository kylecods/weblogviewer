using CommunityToolkit.Mvvm.ComponentModel;

namespace WebLog.Dashboard.ViewModels;

public partial class BaseViewModel : ObservableObject, IDisposable
{
    private readonly SemaphoreSlim _isBusyLock = new(1, 1);

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private bool _isNotBusy;

    protected async Task IsBusyFor(Func<Task> work)
    {
        await _isBusyLock.WaitAsync();

        try
        {
            IsBusy = true;
            
            IsNotBusy = false;

            await work();
        }
        finally
        {
            IsBusy = false;
            IsNotBusy = true;
            _isBusyLock.Release();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        // Cleanup
        if (disposing)
        {
            _isBusyLock.Dispose();
        }
    }
}