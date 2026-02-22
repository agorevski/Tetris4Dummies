using Tetris4Dummies.Core.Helpers;

namespace Tetris4Dummies.Presentation.Services;

/// <summary>
/// MAUI implementation of IMainThreadDispatcher
/// </summary>
public class MainThreadDispatcher : IMainThreadDispatcher
{
    public void BeginInvokeOnMainThread(Action action)
    {
        Microsoft.Maui.ApplicationModel.MainThread.BeginInvokeOnMainThread(action);
    }
}
