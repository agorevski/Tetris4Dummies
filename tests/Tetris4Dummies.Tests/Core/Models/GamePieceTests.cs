using FluentAssertions;
using Tetris4Dummies.Core.Models;

namespace Tetris4Dummies.Tests.Core.Models;

/// <summary>
/// Comprehensive unit tests for GamePiece class
/// Target: 100% code coverage
/// </summary>
public class GamePieceTests
{
    [Fact]
    public void Constructor_ShouldInitializeAtTopWithSpecifiedColumn()
    {
        // Arrange & Act
        GamePiece piece = new GamePiece(5);
        
        // Assert
        piece.Row.Should().Be(0, "piece should start at the top row");
        piece.Column.Should().Be(5, "piece should be at the specified column");
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(5)]
    [InlineData(9)]
    public void Constructor_ShouldAcceptVariousColumnValues(int column)
    {
        // Arrange & Act
        GamePiece piece = new GamePiece(column);
        
        // Assert
        piece.Column.Should().Be(column);
        piece.Row.Should().Be(0);
    }
    
    [Fact]
    public void MoveDown_ShouldIncrementRow()
    {
        // Arrange
        GamePiece piece = new GamePiece(5);
        
        // Act
        piece.MoveDown();
        
        // Assert
        piece.Row.Should().Be(1, "row should increment by 1");
        piece.Column.Should().Be(5, "column should remain unchanged");
    }
    
    [Fact]
    public void MoveDown_MultipleTimesIncrementsRowEachTime()
    {
        // Arrange
        GamePiece piece = new GamePiece(5);
        
        // Act
        piece.MoveDown();
        piece.MoveDown();
        piece.MoveDown();
        
        // Assert
        piece.Row.Should().Be(3, "row should increment with each call");
    }
    
    [Fact]
    public void MoveLeft_ShouldDecrementColumn()
    {
        // Arrange
        GamePiece piece = new GamePiece(5);
        
        // Act
        piece.MoveLeft();
        
        // Assert
        piece.Column.Should().Be(4, "column should decrement by 1");
        piece.Row.Should().Be(0, "row should remain unchanged");
    }
    
    [Fact]
    public void MoveLeft_MultipleTimesDecrementsColumnEachTime()
    {
        // Arrange
        GamePiece piece = new GamePiece(5);
        
        // Act
        piece.MoveLeft();
        piece.MoveLeft();
        piece.MoveLeft();
        
        // Assert
        piece.Column.Should().Be(2, "column should decrement with each call");
    }
    
    [Fact]
    public void MoveLeft_CanMoveIntoBegativeColumns()
    {
        // Arrange
        GamePiece piece = new GamePiece(0);
        
        // Act
        piece.MoveLeft();
        
        // Assert
        piece.Column.Should().Be(-1, "GamePiece allows negative columns (boundary checking is GameState's responsibility)");
    }
    
    [Fact]
    public void MoveRight_ShouldIncrementColumn()
    {
        // Arrange
        GamePiece piece = new GamePiece(5);
        
        // Act
        piece.MoveRight();
        
        // Assert
        piece.Column.Should().Be(6, "column should increment by 1");
        piece.Row.Should().Be(0, "row should remain unchanged");
    }
    
    [Fact]
    public void MoveRight_MultipleTimesIncrementsColumnEachTime()
    {
        // Arrange
        GamePiece piece = new GamePiece(5);
        
        // Act
        piece.MoveRight();
        piece.MoveRight();
        piece.MoveRight();
        
        // Assert
        piece.Column.Should().Be(8, "column should increment with each call");
    }
    
    [Fact]
    public void MoveRight_CanMoveBeyondGridBounds()
    {
        // Arrange
        GamePiece piece = new GamePiece(9);
        
        // Act
        piece.MoveRight();
        
        // Assert
        piece.Column.Should().Be(10, "GamePiece allows out-of-bounds columns (boundary checking is GameState's responsibility)");
    }
    
    [Fact]
    public void Reset_ShouldResetToTopWithNewColumn()
    {
        // Arrange
        GamePiece piece = new GamePiece(5);
        piece.MoveDown();
        piece.MoveDown();
        piece.MoveLeft();
        
        // Act
        piece.Reset(7);
        
        // Assert
        piece.Row.Should().Be(0, "row should be reset to 0");
        piece.Column.Should().Be(7, "column should be set to new value");
    }
    
    [Fact]
    public void Reset_ToSameColumn_ShouldStillResetRow()
    {
        // Arrange
        GamePiece piece = new GamePiece(5);
        piece.MoveDown();
        piece.MoveDown();
        
        // Act
        piece.Reset(5);
        
        // Assert
        piece.Row.Should().Be(0, "row should be reset");
        piece.Column.Should().Be(5, "column should remain the same");
    }
    
    [Theory]
    [InlineData(0, 5, 10, 10)]  // Start 5, left 5 = 0, right 10 = 10
    [InlineData(3, 2, 7, 10)]   // Start 5, left 2 = 3, right 7 = 10
    public void CombinedMovements_ShouldUpdatePositionCorrectly(int downs, int lefts, int rights, int expectedColumn)
    {
        // Arrange
        GamePiece piece = new GamePiece(5);
        
        // Act
        for (int i = 0; i < downs; i++) piece.MoveDown();
        for (int i = 0; i < lefts; i++) piece.MoveLeft();
        for (int i = 0; i < rights; i++) piece.MoveRight();
        
        // Assert
        piece.Row.Should().Be(downs);
        piece.Column.Should().Be(expectedColumn);
    }
    
    [Fact]
    public void RowProperty_CanBeSetDirectly()
    {
        // Arrange
        GamePiece piece = new GamePiece(5);
        
        // Act
        piece.Row = 10;
        
        // Assert
        piece.Row.Should().Be(10, "Row property should be settable");
    }
    
    [Fact]
    public void ColumnProperty_CanBeSetDirectly()
    {
        // Arrange
        GamePiece piece = new GamePiece(5);
        
        // Act
        piece.Column = 7;
        
        // Assert
        piece.Column.Should().Be(7, "Column property should be settable");
    }
    
    #region ColorIndex Tests
    
    [Fact]
    public void Constructor_WithColorIndex_ShouldSetColorIndex()
    {
        // Arrange & Act
        GamePiece piece = new GamePiece(5, 3);
        
        // Assert
        piece.ColorIndex.Should().Be(3, "color index should be set to provided value");
        piece.Column.Should().Be(5, "column should be set correctly");
        piece.Row.Should().Be(0, "row should start at 0");
    }
    
    [Theory]
    [InlineData(1)]
    [InlineData(4)]
    [InlineData(7)]
    public void Constructor_WithDifferentColorIndices_ShouldWork(int colorIndex)
    {
        // Arrange & Act
        GamePiece piece = new GamePiece(5, colorIndex);
        
        // Assert
        piece.ColorIndex.Should().Be(colorIndex);
    }
    
    [Fact]
    public void Constructor_WithDefaultColorIndex_ShouldBeOne()
    {
        // Arrange & Act
        GamePiece piece = new GamePiece(5);
        
        // Assert
        piece.ColorIndex.Should().Be(1, "default color index should be 1");
    }
    
    [Fact]
    public void ColorIndex_CanBeSetDirectly()
    {
        // Arrange
        GamePiece piece = new GamePiece(5, 1);
        
        // Act
        piece.ColorIndex = 5;
        
        // Assert
        piece.ColorIndex.Should().Be(5, "ColorIndex should be settable");
    }
    
    #endregion
}
