using FluentAssertions;
using Moq;
using Tetris4Dummies.Helpers;
using Tetris4Dummies.Models;

namespace Tetris4Dummies.Tests.Models;

/// <summary>
/// Comprehensive unit tests for GameState class with mocked randomness
/// Target: 100% code coverage
/// </summary>
public class GameStateTests
{
    private Mock<IRandomProvider> CreateMockRandom()
    {
        return new Mock<IRandomProvider>();
    }
    
    [Fact]
    public void Constructor_WithDefaultConstructor_ShouldInitializeCorrectly()
    {
        // Arrange & Act
        GameState gameState = new GameState();
        
        // Assert
        gameState.Score.Should().Be(0, "initial score should be 0");
        gameState.IsGameOver.Should().BeFalse("game should not be over initially");
        gameState.Grid.Should().NotBeNull("grid should be initialized");
        gameState.CurrentPiece.Should().BeNull("no piece should be spawned initially");
    }
    
    [Fact]
    public void Constructor_WithRandomProvider_ShouldInitializeCorrectly()
    {
        // Arrange
        Mock<IRandomProvider> mockRandom = CreateMockRandom();
        
        // Act
        GameState gameState = new GameState(mockRandom.Object);
        
        // Assert
        gameState.Score.Should().Be(0);
        gameState.IsGameOver.Should().BeFalse();
        gameState.Grid.Should().NotBeNull();
        gameState.CurrentPiece.Should().BeNull();
    }
    
    [Fact]
    public void StartNewGame_ShouldResetState()
    {
        // Arrange
        Mock<IRandomProvider> mockRandom = CreateMockRandom();
        GameState gameState = new GameState(mockRandom.Object);
        
        // Act
        gameState.StartNewGame();
        
        // Assert
        gameState.Score.Should().Be(0, "score should be reset");
        gameState.IsGameOver.Should().BeFalse("game should not be over");
        gameState.CurrentPiece.Should().NotBeNull("a piece should be spawned");
        gameState.CurrentPiece!.Row.Should().Be(0, "piece should start at top");
        gameState.CurrentPiece.Column.Should().Be(5, "piece should start at center column (10/2)");
    }
    
    [Fact]
    public void StartNewGame_ShouldClearGrid()
    {
        // Arrange
        Mock<IRandomProvider> mockRandom = CreateMockRandom();
        GameState gameState = new GameState(mockRandom.Object);
        gameState.StartNewGame();
        // Manually add some blocks
        gameState.Grid[5, 5] = 1;
        gameState.Grid[10, 3] = 1;
        
        // Act
        gameState.StartNewGame();
        
        // Assert
        gameState.Grid[5, 5].Should().Be(0, "grid should be cleared");
        gameState.Grid[10, 3].Should().Be(0, "grid should be cleared");
    }
    
    [Fact]
    public void StartNewGame_WhenSpawnPositionBlocked_ShouldSetGameOver()
    {
        // Arrange
        Mock<IRandomProvider> mockRandom = CreateMockRandom();
        GameState gameState = new GameState(mockRandom.Object);
        // Block the spawn position before starting
        gameState.Grid[0, 5] = 1;
        
        // Act
        gameState.StartNewGame();
        
        // Assert
        gameState.IsGameOver.Should().BeTrue("game should be over when spawn position is blocked");
        gameState.CurrentPiece.Should().NotBeNull("piece still exists but can't be placed");
    }
    
    [Fact]
    public void MoveDown_WithNoPiece_ShouldReturnFalse()
    {
        // Arrange
        Mock<IRandomProvider> mockRandom = CreateMockRandom();
        GameState gameState = new GameState(mockRandom.Object);
        
        // Act
        bool result = gameState.MoveDown();
        
        // Assert
        result.Should().BeFalse("cannot move down without a piece");
    }
    
    [Fact]
    public void MoveDown_WhenGameOver_ShouldReturnFalse()
    {
        // Arrange
        Mock<IRandomProvider> mockRandom = CreateMockRandom();
        GameState gameState = new GameState(mockRandom.Object);
        gameState.Grid[0, 5] = 1; // Block spawn position
        gameState.StartNewGame(); // Trigger game over
        
        // Act
        bool result = gameState.MoveDown();
        
        // Assert
        result.Should().BeFalse("cannot move when game is over");
    }
    
    [Fact]
    public void MoveDown_WhenPieceCanMove_ShouldReturnTrue()
    {
        // Arrange
        Mock<IRandomProvider> mockRandom = CreateMockRandom();
        GameState gameState = new GameState(mockRandom.Object);
        gameState.StartNewGame();
        int initialRow = gameState.CurrentPiece!.Row;
        
        // Act
        bool result = gameState.MoveDown();
        
        // Assert
        result.Should().BeTrue("piece should be able to move down");
        gameState.CurrentPiece!.Row.Should().Be(initialRow + 1, "piece should have moved down one row");
    }
    
    [Fact]
    public void MoveDown_WhenReachingBottom_ShouldLockAndSpawnNew()
    {
        // Arrange
        Mock<IRandomProvider> mockRandom = CreateMockRandom();
        GameState gameState = new GameState(mockRandom.Object);
        gameState.StartNewGame();
        
        // Move piece to bottom
        while (gameState.CurrentPiece!.Row < GameGrid.Rows - 1)
        {
            gameState.MoveDown();
        }
        
        // Act - final move that lands the piece
        bool result = gameState.MoveDown();
        
        // Assert
        result.Should().BeFalse("piece should have landed");
        gameState.CurrentPiece!.Row.Should().Be(0, "new piece should be spawned at top");
        gameState.Grid[GameGrid.Rows - 1, 5].Should().Be(1, "old piece should be locked in grid");
    }
    
    [Fact]
    public void MoveDown_WhenCollidingWithLockedPiece_ShouldLockAndSpawnNew()
    {
        // Arrange
        Mock<IRandomProvider> mockRandom = CreateMockRandom();
        GameState gameState = new GameState(mockRandom.Object);
        gameState.StartNewGame();
        // Place a block below spawn position
        gameState.Grid[2, 5] = 1;
        
        // Act - move down twice (row 0 -> 1 -> 2, but 2 is blocked)
        gameState.MoveDown(); // Row 0 -> 1 (succeeds)
        bool result = gameState.MoveDown(); // Row 1 -> 2 (blocked, locks at row 1)
        
        // Assert
        result.Should().BeFalse("piece should have locked");
        gameState.Grid[1, 5].Should().Be(1, "piece should be locked at row 1");
        gameState.CurrentPiece!.Row.Should().Be(0, "new piece should be spawned");
    }
    
    [Fact]
    public void MoveLeft_WithNoPiece_ShouldDoNothing()
    {
        // Arrange
        Mock<IRandomProvider> mockRandom = CreateMockRandom();
        GameState gameState = new GameState(mockRandom.Object);
        
        // Act
        gameState.MoveLeft();
        
        // Assert - should not throw
        gameState.CurrentPiece.Should().BeNull();
    }
    
    [Fact]
    public void MoveLeft_WhenGameOver_ShouldDoNothing()
    {
        // Arrange
        Mock<IRandomProvider> mockRandom = CreateMockRandom();
        GameState gameState = new GameState(mockRandom.Object);
        gameState.Grid[0, 5] = 1;
        gameState.StartNewGame(); // Trigger game over
        int initialColumn = gameState.CurrentPiece!.Column;
        
        // Act
        gameState.MoveLeft();
        
        // Assert
        gameState.CurrentPiece.Column.Should().Be(initialColumn, "piece should not move when game is over");
    }
    
    [Fact]
    public void MoveLeft_WhenSpaceIsEmpty_ShouldMove()
    {
        // Arrange
        Mock<IRandomProvider> mockRandom = CreateMockRandom();
        GameState gameState = new GameState(mockRandom.Object);
        gameState.StartNewGame();
        int initialColumn = gameState.CurrentPiece!.Column;
        
        // Act
        gameState.MoveLeft();
        
        // Assert
        gameState.CurrentPiece!.Column.Should().Be(initialColumn - 1, "piece should move left");
    }
    
    [Fact]
    public void MoveLeft_WhenAtLeftBoundary_ShouldNotMove()
    {
        // Arrange
        Mock<IRandomProvider> mockRandom = CreateMockRandom();
        GameState gameState = new GameState(mockRandom.Object);
        gameState.StartNewGame();
        // Move piece to left edge
        gameState.CurrentPiece!.Column = 0;
        
        // Act
        gameState.MoveLeft();
        
        // Assert
        gameState.CurrentPiece!.Column.Should().Be(0, "piece should not move past left boundary");
    }
    
    [Fact]
    public void MoveLeft_WhenBlockedByLockedPiece_ShouldNotMove()
    {
        // Arrange
        Mock<IRandomProvider> mockRandom = CreateMockRandom();
        GameState gameState = new GameState(mockRandom.Object);
        gameState.StartNewGame();
        int initialColumn = gameState.CurrentPiece!.Column;
        // Block left position
        gameState.Grid[0, initialColumn - 1] = 1;
        
        // Act
        gameState.MoveLeft();
        
        // Assert
        gameState.CurrentPiece!.Column.Should().Be(initialColumn, "piece should not move into blocked space");
    }
    
    [Fact]
    public void MoveRight_WithNoPiece_ShouldDoNothing()
    {
        // Arrange
        Mock<IRandomProvider> mockRandom = CreateMockRandom();
        GameState gameState = new GameState(mockRandom.Object);
        
        // Act
        gameState.MoveRight();
        
        // Assert
        gameState.CurrentPiece.Should().BeNull();
    }
    
    [Fact]
    public void MoveRight_WhenGameOver_ShouldDoNothing()
    {
        // Arrange
        Mock<IRandomProvider> mockRandom = CreateMockRandom();
        GameState gameState = new GameState(mockRandom.Object);
        gameState.Grid[0, 5] = 1;
        gameState.StartNewGame(); // Trigger game over
        int initialColumn = gameState.CurrentPiece!.Column;
        
        // Act
        gameState.MoveRight();
        
        // Assert
        gameState.CurrentPiece.Column.Should().Be(initialColumn, "piece should not move when game is over");
    }
    
    [Fact]
    public void MoveRight_WhenSpaceIsEmpty_ShouldMove()
    {
        // Arrange
        Mock<IRandomProvider> mockRandom = CreateMockRandom();
        GameState gameState = new GameState(mockRandom.Object);
        gameState.StartNewGame();
        int initialColumn = gameState.CurrentPiece!.Column;
        
        // Act
        gameState.MoveRight();
        
        // Assert
        gameState.CurrentPiece!.Column.Should().Be(initialColumn + 1, "piece should move right");
    }
    
    [Fact]
    public void MoveRight_WhenAtRightBoundary_ShouldNotMove()
    {
        // Arrange
        Mock<IRandomProvider> mockRandom = CreateMockRandom();
        GameState gameState = new GameState(mockRandom.Object);
        gameState.StartNewGame();
        // Move piece to right edge
        gameState.CurrentPiece!.Column = GameGrid.Columns - 1;
        
        // Act
        gameState.MoveRight();
        
        // Assert
        gameState.CurrentPiece!.Column.Should().Be(GameGrid.Columns - 1, "piece should not move past right boundary");
    }
    
    [Fact]
    public void MoveRight_WhenBlockedByLockedPiece_ShouldNotMove()
    {
        // Arrange
        Mock<IRandomProvider> mockRandom = CreateMockRandom();
        GameState gameState = new GameState(mockRandom.Object);
        gameState.StartNewGame();
        int initialColumn = gameState.CurrentPiece!.Column;
        // Block right position
        gameState.Grid[0, initialColumn + 1] = 1;
        
        // Act
        gameState.MoveRight();
        
        // Assert
        gameState.CurrentPiece!.Column.Should().Be(initialColumn, "piece should not move into blocked space");
    }
    
    [Fact]
    public void Drop_WithNoPiece_ShouldDoNothing()
    {
        // Arrange
        Mock<IRandomProvider> mockRandom = CreateMockRandom();
        GameState gameState = new GameState(mockRandom.Object);
        
        // Act
        gameState.Drop();
        
        // Assert
        gameState.CurrentPiece.Should().BeNull();
    }
    
    [Fact]
    public void Drop_WhenGameOver_ShouldDoNothing()
    {
        // Arrange
        Mock<IRandomProvider> mockRandom = CreateMockRandom();
        GameState gameState = new GameState(mockRandom.Object);
        gameState.Grid[0, 5] = 1;
        gameState.StartNewGame(); // Trigger game over
        
        // Act
        gameState.Drop();
        
        // Assert
        gameState.CurrentPiece!.Row.Should().Be(0, "piece should not drop when game is over");
    }
    
    [Fact]
    public void Drop_ShouldMoveToBottom()
    {
        // Arrange
        Mock<IRandomProvider> mockRandom = CreateMockRandom();
        GameState gameState = new GameState(mockRandom.Object);
        gameState.StartNewGame();
        
        // Act
        gameState.Drop();
        
        // Assert
        gameState.Grid[GameGrid.Rows - 1, 5].Should().Be(1, "piece should be at bottom");
        gameState.CurrentPiece!.Row.Should().Be(0, "new piece should be spawned");
    }
    
    [Fact]
    public void Drop_WithObstacle_ShouldLandAboveObstacle()
    {
        // Arrange
        Mock<IRandomProvider> mockRandom = CreateMockRandom();
        GameState gameState = new GameState(mockRandom.Object);
        gameState.StartNewGame();
        // Place obstacle
        gameState.Grid[10, 5] = 1;
        
        // Act
        gameState.Drop();
        
        // Assert
        gameState.Grid[9, 5].Should().Be(1, "piece should land above obstacle");
        gameState.Grid[10, 5].Should().Be(1, "obstacle should remain");
    }
    
    [Fact]
    public void ClearingOneLine_ShouldAdd100Points()
    {
        // Arrange
        Mock<IRandomProvider> mockRandom = CreateMockRandom();
        GameState gameState = new GameState(mockRandom.Object);
        gameState.StartNewGame();
        // Fill bottom row except one column
        for (int col = 0; col < GameGrid.Columns; col++)
        {
            if (col != 5) gameState.Grid[GameGrid.Rows - 1, col] = 1;
        }
        
        // Act - drop piece to complete the row
        gameState.Drop();
        
        // Assert
        gameState.Score.Should().Be(100, "clearing one line should give 100 points");
    }
    
    [Fact]
    public void ClearingMultipleLines_ShouldAddCorrectPoints()
    {
        // Arrange
        Mock<IRandomProvider> mockRandom = CreateMockRandom();
        GameState gameState = new GameState(mockRandom.Object);
        gameState.StartNewGame();
        // Fill two bottom rows except column 5
        for (int row = GameGrid.Rows - 2; row < GameGrid.Rows; row++)
        {
            for (int col = 0; col < GameGrid.Columns; col++)
            {
                if (col != 5) gameState.Grid[row, col] = 1;
            }
        }
        gameState.CurrentPiece!.Column = 5;
        
        // Act - drop to fill column 5 in both rows
        gameState.Drop();
        gameState.CurrentPiece!.Column = 5;
        gameState.Drop();
        
        // Assert
        gameState.Score.Should().Be(200, "clearing two lines should give 200 points");
    }
    
    [Fact]
    public void GridProperty_ShouldReturnGridInstance()
    {
        // Arrange
        Mock<IRandomProvider> mockRandom = CreateMockRandom();
        GameState gameState = new GameState(mockRandom.Object);
        
        // Act
        GameGrid grid = gameState.Grid;
        
        // Assert
        grid.Should().NotBeNull();
        grid.Should().BeSameAs(gameState.Grid, "should return same instance");
    }
    
    [Fact]
    public void CurrentPieceProperty_ShouldReturnCurrentPiece()
    {
        // Arrange
        Mock<IRandomProvider> mockRandom = CreateMockRandom();
        GameState gameState = new GameState(mockRandom.Object);
        
        // Act & Assert
        gameState.CurrentPiece.Should().BeNull("no piece before starting game");
        
        gameState.StartNewGame();
        gameState.CurrentPiece.Should().NotBeNull("piece should exist after starting");
    }
    
    [Fact]
    public void ScoreProperty_ShouldTrackScore()
    {
        // Arrange
        Mock<IRandomProvider> mockRandom = CreateMockRandom();
        GameState gameState = new GameState(mockRandom.Object);
        gameState.StartNewGame();
        
        // Assert
        gameState.Score.Should().Be(0, "initial score");
        
        // Create a line clear scenario
        for (int col = 0; col < GameGrid.Columns; col++)
        {
            if (col != 5) gameState.Grid[GameGrid.Rows - 1, col] = 1;
        }
        gameState.Drop();
        
        gameState.Score.Should().Be(100, "score should update after line clear");
    }
    
    [Fact]
    public void IsGameOverProperty_ShouldReflectGameState()
    {
        // Arrange
        Mock<IRandomProvider> mockRandom = CreateMockRandom();
        GameState gameState = new GameState(mockRandom.Object);
        
        // Act & Assert
        gameState.IsGameOver.Should().BeFalse("game not over initially");
        
        gameState.StartNewGame();
        gameState.IsGameOver.Should().BeFalse("game not over after start");
        
        // Create new game state with blocked spawn
        GameState gameState2 = new GameState(mockRandom.Object);
        gameState2.Grid[0, 5] = 1;
        gameState2.StartNewGame();
        gameState2.IsGameOver.Should().BeTrue("game should be over when spawn is blocked");
    }
    
    [Fact]
    public void CompleteGameFlow_ShouldWorkCorrectly()
    {
        // Arrange
        Mock<IRandomProvider> mockRandom = CreateMockRandom();
        GameState gameState = new GameState(mockRandom.Object);
        
        // Act - full game flow
        gameState.StartNewGame();
        gameState.MoveLeft();
        gameState.MoveRight();
        gameState.MoveRight();
        gameState.MoveDown();
        gameState.Drop();
        
        // Assert
        gameState.IsGameOver.Should().BeFalse("game should still be playable");
        gameState.CurrentPiece.Should().NotBeNull("new piece should be spawned");
    }
    
    [Fact]
    public void MultipleNewGames_ShouldResetProperly()
    {
        // Arrange
        Mock<IRandomProvider> mockRandom = CreateMockRandom();
        GameState gameState = new GameState(mockRandom.Object);
        
        // Act - play multiple games
        gameState.StartNewGame();
        gameState.Drop();
        int scoreAfterFirstDrop = gameState.Score;
        
        gameState.StartNewGame();
        
        // Assert
        gameState.Score.Should().Be(0, "score should reset on new game");
        gameState.IsGameOver.Should().BeFalse("game over should reset");
        gameState.CurrentPiece!.Row.Should().Be(0, "new piece should be at top");
    }
}
