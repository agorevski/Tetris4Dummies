namespace Tetris4Dummies.Core.Helpers;

/// <summary>
/// Abstraction for main thread dispatching to enable testability
/// </summary>
public interface IMainThreadDispatcher
{
    void BeginInvokeOnMainThread(Action action);
}
