using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Tetris4Dummies.Core.Helpers;
using Tetris4Dummies.Core.Models;
using Tetris4Dummies.Presentation.Graphics;

namespace Tetris4Dummies.Presentation.ViewModels;

/// <summary>
/// ViewModel for the main game page implementing MVVM pattern.
/// Addresses Anti-Pattern #1 (View-Logic coupling) and #2 (Timer management)
/// </summary>
public class GameViewModel : INotifyPropertyChanged, IDisposable
{
    private readonly GameState _gameState;
    private readonly GameDrawable _gameDrawable;
    private readonly NextPieceDrawable _nextPieceDrawable;
    private readonly IMainThreadDispatcher _dispatcher;
    private System.Timers.Timer? _gameTimer;
    private bool _isDisposed;
    
    // Game timing constants
    private const double BasePieceDropIntervalMs = 500;
    private const double LevelSpeedMultiplier = 0.1;
    
    public event PropertyChangedEventHandler? PropertyChanged;
    
    /// <summary>
    /// Event raised when UI needs to be refreshed
    /// </summary>
    public event EventHandler? GameStateChanged;
    
    public GameState GameState => _gameState;
    public GameDrawable GameDrawable => _gameDrawable;
    public NextPieceDrawable NextPieceDrawable => _nextPieceDrawable;
    
    public int Score => _gameState.Score;
    public int Level => _gameState.Level;
    public int Lines => _gameState.Lines;
    public bool IsGameOver => _gameState.IsGameOver;
    
    public ICommand NewGameCommand { get; }
    public ICommand MoveLeftCommand { get; }
    public ICommand MoveRightCommand { get; }
    public ICommand DropCommand { get; }
    
    public GameViewModel() : this(new GameState())
    {
    }
    
    public GameViewModel(GameState gameState) : this(gameState, new SynchronousDispatcher())
    {
    }
    
    public GameViewModel(GameState gameState, IMainThreadDispatcher dispatcher)
    {
        _gameState = gameState;
        _dispatcher = dispatcher;
        _gameDrawable = new GameDrawable(_gameState);
        _nextPieceDrawable = new NextPieceDrawable(_gameState);
        
        NewGameCommand = new RelayCommand(StartNewGame);
        MoveLeftCommand = new RelayCommand(MoveLeft);
        MoveRightCommand = new RelayCommand(MoveRight);
        DropCommand = new RelayCommand(Drop);
    }
    
    /// <summary>
    /// Starts a new game, properly managing timer lifecycle (Anti-Pattern #2 fix)
    /// </summary>
    public void StartNewGame()
    {
        StopTimer();
        
        _gameState.StartNewGame();
        NotifyStateChanged();
        
        StartTimer();
    }
    
    private void StartTimer()
    {
        _gameTimer = new System.Timers.Timer(GetTimerInterval());
        _gameTimer.Elapsed += OnGameTimerTick;
        _gameTimer.AutoReset = true;
        _gameTimer.Start();
    }
    
    private void StopTimer()
    {
        if (_gameTimer != null)
        {
            _gameTimer.Stop();
            _gameTimer.Elapsed -= OnGameTimerTick;
            _gameTimer.Dispose();
            _gameTimer = null;
        }
    }
    
    internal double GetTimerInterval()
    {
        return BasePieceDropIntervalMs / (1 + (_gameState.Level - 1) * LevelSpeedMultiplier);
    }
    
    internal void OnGameTimerTick(object? sender, System.Timers.ElapsedEventArgs e)
    {
        if (_isDisposed) return;
        
        _dispatcher.BeginInvokeOnMainThread(() =>
        {
            if (!_gameState.IsGameOver)
            {
                _gameState.MoveDown();
                NotifyStateChanged();
                
                // Update timer interval if level changed
                if (_gameTimer != null)
                {
                    _gameTimer.Interval = GetTimerInterval();
                }
            }
            else
            {
                StopTimer();
            }
        });
    }
    
    public void MoveLeft()
    {
        _gameState.MoveLeft();
        GameStateChanged?.Invoke(this, EventArgs.Empty);
    }
    
    public void MoveRight()
    {
        _gameState.MoveRight();
        GameStateChanged?.Invoke(this, EventArgs.Empty);
    }
    
    public void Drop()
    {
        _gameState.Drop();
        NotifyStateChanged();
    }
    
    private void NotifyStateChanged()
    {
        OnPropertyChanged(nameof(Score));
        OnPropertyChanged(nameof(Level));
        OnPropertyChanged(nameof(Lines));
        OnPropertyChanged(nameof(IsGameOver));
        GameStateChanged?.Invoke(this, EventArgs.Empty);
    }
    
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    
    protected virtual void Dispose(bool disposing)
    {
        if (_isDisposed) return;
        
        if (disposing)
        {
            StopTimer();
        }
        
        _isDisposed = true;
    }
    
    /// <summary>
    /// Default synchronous dispatcher for non-UI contexts (testing, etc.)
    /// </summary>
    private class SynchronousDispatcher : IMainThreadDispatcher
    {
        public void BeginInvokeOnMainThread(Action action) => action();
    }
}

/// <summary>
/// Simple relay command implementation for MVVM
/// </summary>
public class RelayCommand : ICommand
{
    private readonly Action _execute;
    private readonly Func<bool>? _canExecute;
    
    public event EventHandler? CanExecuteChanged;
    
    public RelayCommand(Action execute, Func<bool>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }
    
    public bool CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;
    
    public void Execute(object? parameter) => _execute();
    
    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
