using Tetris4Dummies.Core.Models;
using Tetris4Dummies.Presentation.Graphics;

namespace Tetris4Dummies.Presentation.Views;

public partial class MainPage : ContentPage
{
	private readonly GameState _gameState;
	private readonly GameDrawable _gameDrawable;
	private readonly NextPieceDrawable _nextPieceDrawable;
	private System.Timers.Timer? _gameTimer;
	private const double BaseGameTickIntervalMs = 500; // Base speed at level 1

	public MainPage()
	{
		InitializeComponent();
		
		_gameState = new GameState();
		_gameDrawable = new GameDrawable(_gameState);
		_nextPieceDrawable = new NextPieceDrawable(_gameState);
		GameCanvas.Drawable = _gameDrawable;
		NextPieceCanvas.Drawable = _nextPieceDrawable;
	}

	protected override void OnDisappearing()
	{
		base.OnDisappearing();
		_gameTimer?.Stop();
		_gameTimer?.Dispose();
	}

	private void OnNewGameClicked(object? sender, EventArgs e)
	{
		StartNewGame();
	}

	private void StartNewGame()
	{
		// Stop existing timer if any
		_gameTimer?.Stop();
		_gameTimer?.Dispose();
		
		// Start new game
		_gameState.StartNewGame();
		UpdateUI();
		
		// Start game loop
		_gameTimer = new System.Timers.Timer(GetTimerInterval());
		_gameTimer.Elapsed += OnGameTimerTick;
		_gameTimer.Start();
	}

	private double GetTimerInterval()
	{
		// Speed increases with level (faster drops)
		return BaseGameTickIntervalMs / (1 + (_gameState.Level - 1) * 0.1);
	}

	private void OnGameTimerTick(object? sender, System.Timers.ElapsedEventArgs e)
	{
		MainThread.BeginInvokeOnMainThread(() =>
		{
			if (!_gameState.IsGameOver)
			{
				_gameState.MoveDown();
				UpdateUI();
				
				// Update timer interval if level changed
				if (_gameTimer != null)
				{
					_gameTimer.Interval = GetTimerInterval();
				}
			}
			else
			{
				_gameTimer?.Stop();
			}
		});
	}

	private void OnLeftClicked(object? sender, EventArgs e)
	{
		_gameState.MoveLeft();
		GameCanvas.Invalidate();
	}

	private void OnRightClicked(object? sender, EventArgs e)
	{
		_gameState.MoveRight();
		GameCanvas.Invalidate();
	}

	private void OnDropClicked(object? sender, EventArgs e)
	{
		_gameState.Drop();
		UpdateUI();
	}

	private void UpdateUI()
	{
		ScoreLabel.Text = _gameState.Score.ToString();
		LevelLabel.Text = _gameState.Level.ToString();
		LinesLabel.Text = _gameState.Lines.ToString();
		GameCanvas.Invalidate();
		NextPieceCanvas.Invalidate();
	}
}
