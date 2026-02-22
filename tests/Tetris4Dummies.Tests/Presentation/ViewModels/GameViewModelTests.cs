using FluentAssertions;
using Moq;
using Tetris4Dummies.Core.Helpers;
using Tetris4Dummies.Core.Models;
using Tetris4Dummies.Presentation.ViewModels;

namespace Tetris4Dummies.Tests.Presentation.ViewModels;

/// <summary>
/// Unit tests for GameViewModel class
/// Tests MVVM pattern implementation and timer management
/// </summary>
public class GameViewModelTests : IDisposable
{
    private readonly Mock<IRandomProvider> _mockRandom;
    private GameViewModel? _viewModel;
    
    public GameViewModelTests()
    {
        _mockRandom = new Mock<IRandomProvider>();
        _mockRandom.Setup(r => r.Next(1, 8)).Returns(1);
    }
    
    public void Dispose()
    {
        _viewModel?.Dispose();
    }
    
    private GameViewModel CreateViewModel()
    {
        var gameState = new GameState(_mockRandom.Object);
        _viewModel = new GameViewModel(gameState);
        return _viewModel;
    }
    
    #region Constructor Tests
    
    [Fact]
    public void Constructor_ShouldInitializeProperties()
    {
        // Act
        var viewModel = CreateViewModel();
        
        // Assert
        viewModel.GameState.Should().NotBeNull();
        viewModel.GameDrawable.Should().NotBeNull();
        viewModel.NextPieceDrawable.Should().NotBeNull();
        viewModel.Score.Should().Be(0);
        viewModel.Level.Should().Be(1);
        viewModel.Lines.Should().Be(0);
        viewModel.IsGameOver.Should().BeFalse();
    }
    
    [Fact]
    public void Constructor_ShouldInitializeCommands()
    {
        // Act
        var viewModel = CreateViewModel();
        
        // Assert
        viewModel.NewGameCommand.Should().NotBeNull();
        viewModel.MoveLeftCommand.Should().NotBeNull();
        viewModel.MoveRightCommand.Should().NotBeNull();
        viewModel.DropCommand.Should().NotBeNull();
    }
    
    #endregion
    
    #region Command Tests
    
    [Fact]
    public void NewGameCommand_ShouldStartNewGame()
    {
        // Arrange
        var viewModel = CreateViewModel();
        
        // Act
        viewModel.NewGameCommand.Execute(null);
        
        // Assert
        viewModel.GameState.CurrentPiece.Should().NotBeNull();
        viewModel.GameState.IsGameOver.Should().BeFalse();
    }
    
    [Fact]
    public void MoveLeftCommand_AfterStartNewGame_ShouldMovePiece()
    {
        // Arrange
        var viewModel = CreateViewModel();
        viewModel.StartNewGame();
        int initialColumn = viewModel.GameState.CurrentPiece!.Column;
        
        // Act
        viewModel.MoveLeftCommand.Execute(null);
        
        // Assert
        viewModel.GameState.CurrentPiece!.Column.Should().Be(initialColumn - 1);
    }
    
    [Fact]
    public void MoveRightCommand_AfterStartNewGame_ShouldMovePiece()
    {
        // Arrange
        var viewModel = CreateViewModel();
        viewModel.StartNewGame();
        int initialColumn = viewModel.GameState.CurrentPiece!.Column;
        
        // Act
        viewModel.MoveRightCommand.Execute(null);
        
        // Assert
        viewModel.GameState.CurrentPiece!.Column.Should().Be(initialColumn + 1);
    }
    
    [Fact]
    public void DropCommand_AfterStartNewGame_ShouldDropPiece()
    {
        // Arrange
        var viewModel = CreateViewModel();
        viewModel.StartNewGame();
        
        // Act
        viewModel.DropCommand.Execute(null);
        
        // Assert - piece should have been dropped and new piece spawned
        viewModel.GameState.Grid[GameGrid.Rows - 1, 5].Should().BePositive();
    }
    
    #endregion
    
    #region Property Changed Tests
    
    [Fact]
    public void StartNewGame_ShouldRaisePropertyChanged()
    {
        // Arrange
        var viewModel = CreateViewModel();
        var propertiesChanged = new List<string?>();
        viewModel.PropertyChanged += (s, e) => propertiesChanged.Add(e.PropertyName);
        
        // Act
        viewModel.StartNewGame();
        
        // Assert
        propertiesChanged.Should().Contain("Score");
        propertiesChanged.Should().Contain("Level");
        propertiesChanged.Should().Contain("Lines");
        propertiesChanged.Should().Contain("IsGameOver");
    }
    
    [Fact]
    public void Drop_ShouldRaiseGameStateChanged()
    {
        // Arrange
        var viewModel = CreateViewModel();
        viewModel.StartNewGame();
        bool eventRaised = false;
        viewModel.GameStateChanged += (s, e) => eventRaised = true;
        
        // Act
        viewModel.Drop();
        
        // Assert
        eventRaised.Should().BeTrue();
    }
    
    [Fact]
    public void MoveLeft_ShouldRaiseGameStateChanged()
    {
        // Arrange
        var viewModel = CreateViewModel();
        viewModel.StartNewGame();
        bool eventRaised = false;
        viewModel.GameStateChanged += (s, e) => eventRaised = true;
        
        // Act
        viewModel.MoveLeft();
        
        // Assert
        eventRaised.Should().BeTrue();
    }
    
    [Fact]
    public void MoveRight_ShouldRaiseGameStateChanged()
    {
        // Arrange
        var viewModel = CreateViewModel();
        viewModel.StartNewGame();
        bool eventRaised = false;
        viewModel.GameStateChanged += (s, e) => eventRaised = true;
        
        // Act
        viewModel.MoveRight();
        
        // Assert
        eventRaised.Should().BeTrue();
    }
    
    #endregion
    
    #region Property Binding Tests
    
    [Fact]
    public void Score_ShouldReflectGameState()
    {
        // Arrange
        var viewModel = CreateViewModel();
        viewModel.StartNewGame();
        
        // Fill bottom row except one column to create line clear
        for (int col = 0; col < GameGrid.Columns; col++)
        {
            if (col != 5) viewModel.GameState.Grid[GameGrid.Rows - 1, col] = 1;
        }
        
        // Act
        viewModel.Drop();
        
        // Assert
        viewModel.Score.Should().Be(40); // Single line at level 1
    }
    
    [Fact]
    public void Level_ShouldReflectGameState()
    {
        // Arrange
        var viewModel = CreateViewModel();
        viewModel.StartNewGame();
        
        // Assert
        viewModel.Level.Should().Be(viewModel.GameState.Level);
    }
    
    [Fact]
    public void Lines_ShouldReflectGameState()
    {
        // Arrange
        var viewModel = CreateViewModel();
        viewModel.StartNewGame();
        
        // Assert
        viewModel.Lines.Should().Be(viewModel.GameState.Lines);
    }
    
    [Fact]
    public void IsGameOver_ShouldReflectGameState()
    {
        // Arrange
        var viewModel = CreateViewModel();
        
        // Assert
        viewModel.IsGameOver.Should().Be(viewModel.GameState.IsGameOver);
    }
    
    #endregion
    
    #region Dispose Tests
    
    [Fact]
    public void Dispose_ShouldNotThrow()
    {
        // Arrange
        var viewModel = CreateViewModel();
        viewModel.StartNewGame();
        
        // Act & Assert
        var action = () => viewModel.Dispose();
        action.Should().NotThrow();
    }
    
    [Fact]
    public void Dispose_MultipleTimes_ShouldNotThrow()
    {
        // Arrange
        var viewModel = CreateViewModel();
        viewModel.StartNewGame();
        
        // Act & Assert
        var action = () =>
        {
            viewModel.Dispose();
            viewModel.Dispose();
        };
        action.Should().NotThrow();
    }
    
    #endregion
    
    #region Timer Tick Tests

    [Fact]
    public void OnGameTimerTick_WhenGameNotOver_ShouldMoveDown()
    {
        // Arrange
        var viewModel = CreateViewModel();
        viewModel.StartNewGame();
        int initialRow = viewModel.GameState.CurrentPiece!.Row;
        
        // Act
        viewModel.OnGameTimerTick(null, null!);
        
        // Assert
        viewModel.GameState.CurrentPiece!.Row.Should().Be(initialRow + 1);
    }

    [Fact]
    public void OnGameTimerTick_WhenGameNotOver_ShouldRaisePropertyChanged()
    {
        // Arrange
        var viewModel = CreateViewModel();
        viewModel.StartNewGame();
        var propertiesChanged = new List<string?>();
        viewModel.PropertyChanged += (s, e) => propertiesChanged.Add(e.PropertyName);
        
        // Act
        viewModel.OnGameTimerTick(null, null!);
        
        // Assert
        propertiesChanged.Should().Contain("Score");
        propertiesChanged.Should().Contain("IsGameOver");
    }

    [Fact]
    public void OnGameTimerTick_WhenGameOver_ShouldNotMoveDown()
    {
        // Arrange
        var viewModel = CreateViewModel();
        viewModel.StartNewGame();
        
        // Fill the grid to cause game over
        for (int row = 0; row < GameGrid.Rows; row++)
        {
            for (int col = 0; col < GameGrid.Columns; col++)
            {
                viewModel.GameState.Grid[row, col] = 1;
            }
        }
        // Drop to trigger game over
        viewModel.Drop();
        
        // Act - should not throw or move
        var action = () => viewModel.OnGameTimerTick(null, null!);
        action.Should().NotThrow();
    }

    [Fact]
    public void OnGameTimerTick_WhenDisposed_ShouldReturnEarly()
    {
        // Arrange
        var viewModel = CreateViewModel();
        viewModel.StartNewGame();
        viewModel.Dispose();
        
        // Act & Assert - should not throw
        var action = () => viewModel.OnGameTimerTick(null, null!);
        action.Should().NotThrow();
    }

    [Fact]
    public void GetTimerInterval_AtLevel1_ShouldReturnBaseInterval()
    {
        // Arrange
        var viewModel = CreateViewModel();
        
        // Act
        double interval = viewModel.GetTimerInterval();
        
        // Assert
        interval.Should().Be(500); // BasePieceDropIntervalMs / (1 + 0 * 0.1)
    }

    [Fact]
    public void GetTimerInterval_AtHigherLevel_ShouldReturnFasterInterval()
    {
        // Arrange
        var viewModel = CreateViewModel();
        viewModel.StartNewGame();
        
        // Simulate higher level by clearing lines
        // Fill and clear 10 rows to reach level 2
        for (int i = 0; i < 10; i++)
        {
            for (int col = 0; col < GameGrid.Columns; col++)
            {
                viewModel.GameState.Grid[GameGrid.Rows - 1, col] = 1;
            }
            viewModel.GameState.Grid[GameGrid.Rows - 1, 5] = 0;
            viewModel.Drop();
        }
        
        // Assert - at level > 1, interval should be less than base
        double interval = viewModel.GetTimerInterval();
        interval.Should().BeLessThan(500);
    }

    #endregion
}

/// <summary>
/// Unit tests for RelayCommand class
/// </summary>
public class RelayCommandTests
{
    [Fact]
    public void Execute_ShouldCallAction()
    {
        // Arrange
        bool executed = false;
        var command = new RelayCommand(() => executed = true);
        
        // Act
        command.Execute(null);
        
        // Assert
        executed.Should().BeTrue();
    }
    
    [Fact]
    public void CanExecute_WithoutCanExecuteFunc_ShouldReturnTrue()
    {
        // Arrange
        var command = new RelayCommand(() => { });
        
        // Act
        bool result = command.CanExecute(null);
        
        // Assert
        result.Should().BeTrue();
    }
    
    [Fact]
    public void CanExecute_WithCanExecuteFuncReturningTrue_ShouldReturnTrue()
    {
        // Arrange
        var command = new RelayCommand(() => { }, () => true);
        
        // Act
        bool result = command.CanExecute(null);
        
        // Assert
        result.Should().BeTrue();
    }
    
    [Fact]
    public void CanExecute_WithCanExecuteFuncReturningFalse_ShouldReturnFalse()
    {
        // Arrange
        var command = new RelayCommand(() => { }, () => false);
        
        // Act
        bool result = command.CanExecute(null);
        
        // Assert
        result.Should().BeFalse();
    }
    
    [Fact]
    public void RaiseCanExecuteChanged_ShouldRaiseEvent()
    {
        // Arrange
        var command = new RelayCommand(() => { });
        bool eventRaised = false;
        command.CanExecuteChanged += (s, e) => eventRaised = true;
        
        // Act
        command.RaiseCanExecuteChanged();
        
        // Assert
        eventRaised.Should().BeTrue();
    }
    
    [Fact]
    public void Constructor_WithNullExecute_ShouldThrow()
    {
        // Act & Assert
        var action = () => new RelayCommand(null!);
        action.Should().Throw<ArgumentNullException>();
    }
}
