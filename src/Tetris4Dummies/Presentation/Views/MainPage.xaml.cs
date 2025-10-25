using Tetris4Dummies.Core.Models;
using Tetris4Dummies.Presentation.Graphics;

namespace Tetris4Dummies.Presentation.Views;

public partial class MainPage : ContentPage
{
	private readonly GameState _gameState;
	private readonly GameDrawable _gameDrawable;
	private System.Timers.Timer? _gameTimer;
	private const double GameTickIntervalMs = 500; // Move down every 500ms

	public MainPage()
	{
		InitializeComponent();
		
		_gameState = new GameState();
		_gameDrawable = new GameDrawable(_gameState);
		GameCanvas.Drawable = _gameDrawable;
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
		UpdateScore();
		GameCanvas.Invalidate();
		
		// Start game loop
		_gameTimer = new System.Timers.Timer(GameTickIntervalMs);
		_gameTimer.Elapsed += OnGameTimerTick;
		_gameTimer.Start();
	}

	private void OnGameTimerTick(object? sender, System.Timers.ElapsedEventArgs e)
	{
		MainThread.BeginInvokeOnMainThread(() =>
		{
			if (!_gameState.IsGameOver)
			{
				_gameState.MoveDown();
				UpdateScore();
				GameCanvas.Invalidate();
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
		UpdateScore();
		GameCanvas.Invalidate();
	}

	private void UpdateScore()
	{
		ScoreLabel.Text = $"Score: {_gameState.Score}";
	}
}
