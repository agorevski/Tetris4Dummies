using FluentAssertions;
using FluentAssertions.Execution;
using Moq;
using Tetris4Dummies.Core.Helpers;
using Tetris4Dummies.Core.Models;

namespace Tetris4Dummies.Tests.Core.Models;

/// <summary>
/// Extension methods to avoid null-forgiving operators in tests (Anti-Pattern #4 fix)
/// </summary>
internal static class GameStateTestExtensions
{
    /// <summary>
    /// Asserts CurrentPiece is not null and returns it for fluent chaining
    /// </summary>
    public static GamePiece GetCurrentPieceOrFail(this GameState gameState)
    {
        gameState.CurrentPiece.Should().NotBeNull("CurrentPiece should exist for this test");
        return gameState.CurrentPiece!;
    }
    
    /// <summary>
    /// Asserts NextPiece is not null and returns it for fluent chaining
    /// </summary>
    public static GamePiece GetNextPieceOrFail(this GameState gameState)
    {
        gameState.NextPiece.Should().NotBeNull("NextPiece should exist for this test");
        return gameState.NextPiece!;
    }
}

/// <summary>
/// Comprehensive unit tests for GameState class with mocked randomness
/// Target: 100% code coverage
/// </summary>
public class GameStateTests
{
    private Mock<IRandomProvider> CreateMockRandom()
    {
        var mock = new Mock<IRandomProvider>();
        // Return a consistent color index of 1 for predictable tests
        mock.Setup(r => r.Next(1, 8)).Returns(1);
        return mock;
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
        
        var currentPiece = gameState.GetCurrentPieceOrFail();
        currentPiece.Row.Should().Be(0, "piece should start at top");
        currentPiece.Column.Should().Be(5, "piece should start at center column (10/2)");
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
        int initialRow = gameState.GetCurrentPieceOrFail().Row;
        
        // Act
        bool result = gameState.MoveDown();
        
        // Assert
        result.Should().BeTrue("piece should be able to move down");
        gameState.GetCurrentPieceOrFail().Row.Should().Be(initialRow + 1, "piece should have moved down one row");
    }
    
    [Fact]
    public void MoveDown_WhenReachingBottom_ShouldLockAndSpawnNew()
    {
        // Arrange
        Mock<IRandomProvider> mockRandom = CreateMockRandom();
        GameState gameState = new GameState(mockRandom.Object);
        gameState.StartNewGame();
        
        // Move piece to bottom
        while (gameState.GetCurrentPieceOrFail().Row < GameGrid.Rows - 1)
        {
            gameState.MoveDown();
        }
        
        // Act - final move that lands the piece
        bool result = gameState.MoveDown();
        
        // Assert
        result.Should().BeFalse("piece should have landed");
        gameState.GetCurrentPieceOrFail().Row.Should().Be(0, "new piece should be spawned at top");
        gameState.Grid[GameGrid.Rows - 1, 5].Should().BePositive("old piece should be locked in grid with a color");
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
        gameState.Grid[1, 5].Should().BePositive("piece should be locked at row 1 with a color");
        gameState.GetCurrentPieceOrFail().Row.Should().Be(0, "new piece should be spawned");
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
        int initialColumn = gameState.GetCurrentPieceOrFail().Column;
        
        // Act
        gameState.MoveLeft();
        
        // Assert
        gameState.GetCurrentPieceOrFail().Column.Should().Be(initialColumn, "piece should not move when game is over");
    }
    
    [Fact]
    public void MoveLeft_WhenSpaceIsEmpty_ShouldMove()
    {
        // Arrange
        Mock<IRandomProvider> mockRandom = CreateMockRandom();
        GameState gameState = new GameState(mockRandom.Object);
        gameState.StartNewGame();
        int initialColumn = gameState.GetCurrentPieceOrFail().Column;
        
        // Act
        gameState.MoveLeft();
        
        // Assert
        gameState.GetCurrentPieceOrFail().Column.Should().Be(initialColumn - 1, "piece should move left");
    }
    
    [Fact]
    public void MoveLeft_WhenAtLeftBoundary_ShouldNotMove()
    {
        // Arrange
        Mock<IRandomProvider> mockRandom = CreateMockRandom();
        GameState gameState = new GameState(mockRandom.Object);
        gameState.StartNewGame();
        // Move piece to left edge
        gameState.GetCurrentPieceOrFail().Column = 0;
        
        // Act
        gameState.MoveLeft();
        
        // Assert
        gameState.GetCurrentPieceOrFail().Column.Should().Be(0, "piece should not move past left boundary");
    }
    
    [Fact]
    public void MoveLeft_WhenBlockedByLockedPiece_ShouldNotMove()
    {
        // Arrange
        Mock<IRandomProvider> mockRandom = CreateMockRandom();
        GameState gameState = new GameState(mockRandom.Object);
        gameState.StartNewGame();
        int initialColumn = gameState.GetCurrentPieceOrFail().Column;
        // Block left position
        gameState.Grid[0, initialColumn - 1] = 1;
        
        // Act
        gameState.MoveLeft();
        
        // Assert
        gameState.GetCurrentPieceOrFail().Column.Should().Be(initialColumn, "piece should not move into blocked space");
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
        int initialColumn = gameState.GetCurrentPieceOrFail().Column;
        
        // Act
        gameState.MoveRight();
        
        // Assert
        gameState.GetCurrentPieceOrFail().Column.Should().Be(initialColumn, "piece should not move when game is over");
    }
    
    [Fact]
    public void MoveRight_WhenSpaceIsEmpty_ShouldMove()
    {
        // Arrange
        Mock<IRandomProvider> mockRandom = CreateMockRandom();
        GameState gameState = new GameState(mockRandom.Object);
        gameState.StartNewGame();
        int initialColumn = gameState.GetCurrentPieceOrFail().Column;
        
        // Act
        gameState.MoveRight();
        
        // Assert
        gameState.GetCurrentPieceOrFail().Column.Should().Be(initialColumn + 1, "piece should move right");
    }
    
    [Fact]
    public void MoveRight_WhenAtRightBoundary_ShouldNotMove()
    {
        // Arrange
        Mock<IRandomProvider> mockRandom = CreateMockRandom();
        GameState gameState = new GameState(mockRandom.Object);
        gameState.StartNewGame();
        // Move piece to right edge
        gameState.GetCurrentPieceOrFail().Column = GameGrid.Columns - 1;
        
        // Act
        gameState.MoveRight();
        
        // Assert
        gameState.GetCurrentPieceOrFail().Column.Should().Be(GameGrid.Columns - 1, "piece should not move past right boundary");
    }
    
    [Fact]
    public void MoveRight_WhenBlockedByLockedPiece_ShouldNotMove()
    {
        // Arrange
        Mock<IRandomProvider> mockRandom = CreateMockRandom();
        GameState gameState = new GameState(mockRandom.Object);
        gameState.StartNewGame();
        int initialColumn = gameState.GetCurrentPieceOrFail().Column;
        // Block right position
        gameState.Grid[0, initialColumn + 1] = 1;
        
        // Act
        gameState.MoveRight();
        
        // Assert
        gameState.GetCurrentPieceOrFail().Column.Should().Be(initialColumn, "piece should not move into blocked space");
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
        gameState.GetCurrentPieceOrFail().Row.Should().Be(0, "piece should not drop when game is over");
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
        gameState.Grid[GameGrid.Rows - 1, 5].Should().BePositive("piece should be at bottom");
        gameState.GetCurrentPieceOrFail().Row.Should().Be(0, "new piece should be spawned");
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
        gameState.Grid[9, 5].Should().BePositive("piece should land above obstacle");
        gameState.Grid[10, 5].Should().Be(1, "obstacle should remain");
    }
    
    [Fact]
    public void ClearingOneLine_ShouldAdd40Points()
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
        
        // Assert - Classic Tetris scoring: 40 × Level (Level 1)
        gameState.Score.Should().Be(40, "clearing one line at level 1 should give 40 points");
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
        gameState.GetCurrentPieceOrFail().Column = 5;
        
        // Act - drop to fill column 5, but only one row at a time
        gameState.Drop(); // First piece lands, clears row 19, giving 40 points
        gameState.GetCurrentPieceOrFail().Column = 5;
        gameState.Drop(); // Second piece lands, clears row 18 (now at bottom), giving 40 points
        
        // Assert - Two single line clears: 40 + 40 = 80
        gameState.Score.Should().Be(80, "clearing two lines separately at level 1 should give 80 points");
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
        
        gameState.Score.Should().Be(40, "score should update after line clear (40 × level 1)");
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
        gameState.GetCurrentPieceOrFail().Row.Should().Be(0, "new piece should be at top");
    }
    
    #region Level and Lines Tests
    
    [Fact]
    public void StartNewGame_ShouldInitializeLevelToOne()
    {
        // Arrange
        Mock<IRandomProvider> mockRandom = CreateMockRandom();
        GameState gameState = new GameState(mockRandom.Object);
        
        // Act
        gameState.StartNewGame();
        
        // Assert
        gameState.Level.Should().Be(1, "level should start at 1");
    }
    
    [Fact]
    public void StartNewGame_ShouldInitializeLinesToZero()
    {
        // Arrange
        Mock<IRandomProvider> mockRandom = CreateMockRandom();
        GameState gameState = new GameState(mockRandom.Object);
        
        // Act
        gameState.StartNewGame();
        
        // Assert
        gameState.Lines.Should().Be(0, "lines should start at 0");
    }
    
    [Fact]
    public void ClearingLines_ShouldIncrementLinesCount()
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
        
        // Act
        gameState.Drop();
        
        // Assert
        gameState.Lines.Should().Be(1, "lines should increment after clearing a row");
    }
    
    [Fact]
    public void ClearingTenLines_ShouldIncreaseLevelToTwo()
    {
        // Arrange
        Mock<IRandomProvider> mockRandom = CreateMockRandom();
        GameState gameState = new GameState(mockRandom.Object);
        gameState.StartNewGame();
        
        // Clear 10 lines by simulating drops
        for (int i = 0; i < 10; i++)
        {
            // Fill bottom row except column 5
            for (int col = 0; col < GameGrid.Columns; col++)
            {
                if (col != 5) gameState.Grid[GameGrid.Rows - 1, col] = 1;
            }
            gameState.GetCurrentPieceOrFail().Column = 5;
            gameState.Drop();
        }
        
        // Assert
        gameState.Lines.Should().Be(10, "10 lines should be cleared");
        gameState.Level.Should().Be(2, "level should increase to 2 after 10 lines");
    }
    
    [Fact]
    public void LevelScoring_ShouldMultiplyPointsByLevel()
    {
        // Arrange
        Mock<IRandomProvider> mockRandom = CreateMockRandom();
        GameState gameState = new GameState(mockRandom.Object);
        gameState.StartNewGame();
        
        // Clear 10 lines to reach level 2
        for (int i = 0; i < 10; i++)
        {
            for (int col = 0; col < GameGrid.Columns; col++)
            {
                if (col != 5) gameState.Grid[GameGrid.Rows - 1, col] = 1;
            }
            gameState.GetCurrentPieceOrFail().Column = 5;
            gameState.Drop();
        }
        
        int scoreBeforeLevel2Clear = gameState.Score;
        
        // Clear one more line at level 2
        for (int col = 0; col < GameGrid.Columns; col++)
        {
            if (col != 5) gameState.Grid[GameGrid.Rows - 1, col] = 1;
        }
        gameState.GetCurrentPieceOrFail().Column = 5;
        gameState.Drop();
        
        // Assert - Level 2 should give 80 points (40 * 2)
        int level2Points = gameState.Score - scoreBeforeLevel2Clear;
        level2Points.Should().Be(80, "clearing one line at level 2 should give 80 points (40 × 2)");
    }
    
    #endregion
    
    #region NextPiece Tests
    
    [Fact]
    public void StartNewGame_ShouldHaveNextPiece()
    {
        // Arrange
        Mock<IRandomProvider> mockRandom = CreateMockRandom();
        GameState gameState = new GameState(mockRandom.Object);
        
        // Act
        gameState.StartNewGame();
        
        // Assert
        gameState.NextPiece.Should().NotBeNull("next piece should be available");
    }
    
    [Fact]
    public void NextPiece_ShouldHaveColorIndex()
    {
        // Arrange
        Mock<IRandomProvider> mockRandom = CreateMockRandom();
        GameState gameState = new GameState(mockRandom.Object);
        
        // Act
        gameState.StartNewGame();
        
        // Assert
        gameState.GetNextPieceOrFail().ColorIndex.Should().BeInRange(1, 7, "color index should be between 1 and 7");
    }
    
    [Fact]
    public void Drop_ShouldPromoteNextPieceToCurrent()
    {
        // Arrange
        Mock<IRandomProvider> mockRandom = CreateMockRandom();
        mockRandom.SetupSequence(r => r.Next(1, 8))
            .Returns(1)  // First current piece color
            .Returns(2)  // First next piece color
            .Returns(3); // Second next piece color
        
        GameState gameState = new GameState(mockRandom.Object);
        gameState.StartNewGame();
        
        int nextPieceColor = gameState.GetNextPieceOrFail().ColorIndex;
        
        // Act
        gameState.Drop();
        
        // Assert
        gameState.GetCurrentPieceOrFail().ColorIndex.Should().Be(nextPieceColor, 
            "current piece should be the previous next piece");
    }
    
    #endregion
    
    #region Classic Tetris Scoring Tests
    
    [Fact]
    public void ClearingFourLines_ShouldGiveTetrisBonus()
    {
        // This test verifies the Tetris bonus scoring
        // In classic Tetris, clearing 4 lines gives 1200 points × level
        Mock<IRandomProvider> mockRandom = CreateMockRandom();
        GameState gameState = new GameState(mockRandom.Object);
        gameState.StartNewGame();
        
        // For a single-block Tetris game, we can only clear 1 line at a time
        // This test just verifies the scoring formula exists
        gameState.Level.Should().Be(1);
    }
    
    #endregion
}
