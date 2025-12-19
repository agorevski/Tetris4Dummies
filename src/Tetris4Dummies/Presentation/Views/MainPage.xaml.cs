using Tetris4Dummies.Presentation.ViewModels;

namespace Tetris4Dummies.Presentation.Views;

public partial class MainPage : ContentPage
{
	private readonly GameViewModel _viewModel;

	public MainPage()
	{
		InitializeComponent();
		
		_viewModel = new GameViewModel();
		_viewModel.PropertyChanged += OnViewModelPropertyChanged;
		_viewModel.GameStateChanged += OnGameStateChanged;
		
		GameCanvas.Drawable = _viewModel.GameDrawable;
		NextPieceCanvas.Drawable = _viewModel.NextPieceDrawable;
	}

	protected override void OnDisappearing()
	{
		base.OnDisappearing();
		_viewModel.Dispose();
	}

	private void OnNewGameClicked(object? sender, EventArgs e)
	{
		_viewModel.NewGameCommand.Execute(null);
	}

	private void OnLeftClicked(object? sender, EventArgs e)
	{
		_viewModel.MoveLeftCommand.Execute(null);
	}

	private void OnRightClicked(object? sender, EventArgs e)
	{
		_viewModel.MoveRightCommand.Execute(null);
	}

	private void OnDropClicked(object? sender, EventArgs e)
	{
		_viewModel.DropCommand.Execute(null);
	}
	
	private void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
	{
		switch (e.PropertyName)
		{
			case nameof(GameViewModel.Score):
				ScoreLabel.Text = _viewModel.Score.ToString();
				break;
			case nameof(GameViewModel.Level):
				LevelLabel.Text = _viewModel.Level.ToString();
				break;
			case nameof(GameViewModel.Lines):
				LinesLabel.Text = _viewModel.Lines.ToString();
				break;
		}
	}
	
	private void OnGameStateChanged(object? sender, EventArgs e)
	{
		GameCanvas.Invalidate();
		NextPieceCanvas.Invalidate();
	}
}
