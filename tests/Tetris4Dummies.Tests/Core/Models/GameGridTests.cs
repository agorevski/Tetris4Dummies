using FluentAssertions;
using Tetris4Dummies.Core.Models;

namespace Tetris4Dummies.Tests.Core.Models;

/// <summary>
/// Comprehensive unit tests for GameGrid class
/// Target: 100% code coverage
/// </summary>
public class GameGridTests
{
    [Fact]
    public void Constructor_ShouldCreateEmptyGrid()
    {
        // Arrange & Act
        GameGrid grid = new GameGrid();
        
        // Assert
        for (int row = 0; row < GameGrid.Rows; row++)
        {
            for (int col = 0; col < GameGrid.Columns; col++)
            {
                grid[row, col].Should().Be(0, $"cell at ({row}, {col}) should be empty");
            }
        }
    }
    
    [Fact]
    public void Constants_ShouldHaveCorrectValues()
    {
        // Assert
        GameGrid.Rows.Should().Be(20, "standard Tetris grid has 20 rows");
        GameGrid.Columns.Should().Be(10, "standard Tetris grid has 10 columns");
    }
    
    [Fact]
    public void Indexer_ShouldGetAndSetValues()
    {
        // Arrange
        GameGrid grid = new GameGrid();
        
        // Act
        grid[5, 3] = 1;
        
        // Assert
        grid[5, 3].Should().Be(1, "value should be set and retrievable");
        grid[5, 4].Should().Be(0, "adjacent cell should remain empty");
    }
    
    [Theory]
    [InlineData(0, 0)]
    [InlineData(19, 9)]
    [InlineData(10, 5)]
    public void Indexer_ShouldWorkForValidBounds(int row, int col)
    {
        // Arrange
        GameGrid grid = new GameGrid();
        
        // Act
        grid[row, col] = 1;
        int value = grid[row, col];
        
        // Assert
        value.Should().Be(1);
    }
    
    [Theory]
    [InlineData(0, 0, true)]
    [InlineData(19, 9, true)]
    [InlineData(10, 5, true)]
    [InlineData(-1, 0, false)]
    [InlineData(0, -1, false)]
    [InlineData(20, 0, false)]
    [InlineData(0, 10, false)]
    [InlineData(-1, -1, false)]
    [InlineData(20, 10, false)]
    public void IsInBounds_ShouldReturnCorrectValue(int row, int col, bool expected)
    {
        // Arrange
        GameGrid grid = new GameGrid();
        
        // Act
        bool result = grid.IsInBounds(row, col);
        
        // Assert
        result.Should().Be(expected);
    }
    
    [Fact]
    public void IsEmpty_ShouldReturnTrueForEmptyCell()
    {
        // Arrange
        GameGrid grid = new GameGrid();
        
        // Act & Assert
        grid.IsEmpty(5, 5).Should().BeTrue("cell should be empty initially");
    }
    
    [Fact]
    public void IsEmpty_ShouldReturnFalseForFilledCell()
    {
        // Arrange
        GameGrid grid = new GameGrid();
        grid[5, 5] = 1;
        
        // Act & Assert
        grid.IsEmpty(5, 5).Should().BeFalse("cell is filled");
    }
    
    [Theory]
    [InlineData(-1, 0)]
    [InlineData(0, -1)]
    [InlineData(20, 0)]
    [InlineData(0, 10)]
    public void IsEmpty_ShouldReturnFalseForOutOfBounds(int row, int col)
    {
        // Arrange
        GameGrid grid = new GameGrid();
        
        // Act & Assert
        grid.IsEmpty(row, col).Should().BeFalse("out of bounds should be considered not empty");
    }
    
    [Fact]
    public void IsRowFull_ShouldReturnFalseForEmptyRow()
    {
        // Arrange
        GameGrid grid = new GameGrid();
        
        // Act & Assert
        grid.IsRowFull(0).Should().BeFalse("empty row should not be full");
    }
    
    [Fact]
    public void IsRowFull_ShouldReturnFalseForPartiallyFilledRow()
    {
        // Arrange
        GameGrid grid = new GameGrid();
        for (int col = 0; col < 5; col++)
        {
            grid[0, col] = 1;
        }
        
        // Act & Assert
        grid.IsRowFull(0).Should().BeFalse("partially filled row should not be full");
    }
    
    [Fact]
    public void IsRowFull_ShouldReturnTrueForCompletelyFilledRow()
    {
        // Arrange
        GameGrid grid = new GameGrid();
        for (int col = 0; col < GameGrid.Columns; col++)
        {
            grid[0, col] = 1;
        }
        
        // Act & Assert
        grid.IsRowFull(0).Should().BeTrue("completely filled row should be full");
    }
    
    [Fact]
    public void ClearRow_ShouldSetAllCellsToZero()
    {
        // Arrange
        GameGrid grid = new GameGrid();
        for (int col = 0; col < GameGrid.Columns; col++)
        {
            grid[5, col] = 1;
        }
        
        // Act
        grid.ClearRow(5);
        
        // Assert
        for (int col = 0; col < GameGrid.Columns; col++)
        {
            grid[5, col].Should().Be(0, $"column {col} should be cleared");
        }
    }
    
    [Fact]
    public void ClearRow_ShouldNotAffectOtherRows()
    {
        // Arrange
        GameGrid grid = new GameGrid();
        grid[4, 0] = 1;
        grid[5, 0] = 1;
        grid[6, 0] = 1;
        
        // Act
        grid.ClearRow(5);
        
        // Assert
        grid[4, 0].Should().Be(1, "row above should not be affected");
        grid[5, 0].Should().Be(0, "cleared row should be empty");
        grid[6, 0].Should().Be(1, "row below should not be affected");
    }
    
    [Fact]
    public void MoveRowDown_ShouldMoveRowContentDown()
    {
        // Arrange
        GameGrid grid = new GameGrid();
        grid[5, 0] = 1;
        grid[5, 1] = 1;
        grid[5, 2] = 1;
        
        // Act
        grid.MoveRowDown(5, 1);
        
        // Assert
        grid[5, 0].Should().Be(0, "source row should be cleared");
        grid[5, 1].Should().Be(0, "source row should be cleared");
        grid[5, 2].Should().Be(0, "source row should be cleared");
        grid[6, 0].Should().Be(1, "content should move to row 6");
        grid[6, 1].Should().Be(1, "content should move to row 6");
        grid[6, 2].Should().Be(1, "content should move to row 6");
    }
    
    [Fact]
    public void MoveRowDown_WithMultipleRows_ShouldMoveCorrectly()
    {
        // Arrange
        GameGrid grid = new GameGrid();
        grid[5, 0] = 1;
        
        // Act
        grid.MoveRowDown(5, 3);
        
        // Assert
        grid[5, 0].Should().Be(0, "source row should be cleared");
        grid[8, 0].Should().Be(1, "content should move down 3 rows");
    }
    
    [Fact]
    public void ClearFullRows_WithNoFullRows_ShouldReturnZero()
    {
        // Arrange
        GameGrid grid = new GameGrid();
        grid[19, 0] = 1;
        grid[19, 1] = 1;
        
        // Act
        int cleared = grid.ClearFullRows();
        
        // Assert
        cleared.Should().Be(0, "no full rows to clear");
    }
    
    [Fact]
    public void ClearFullRows_WithOneFullRow_ShouldReturnOne()
    {
        // Arrange
        GameGrid grid = new GameGrid();
        for (int col = 0; col < GameGrid.Columns; col++)
        {
            grid[19, col] = 1;
        }
        
        // Act
        int cleared = grid.ClearFullRows();
        
        // Assert
        cleared.Should().Be(1, "one row should be cleared");
        grid.IsRowFull(19).Should().BeFalse("row should no longer be full");
    }
    
    [Fact]
    public void ClearFullRows_WithMultipleFullRows_ShouldClearAll()
    {
        // Arrange
        GameGrid grid = new GameGrid();
        // Fill rows 18 and 19
        for (int col = 0; col < GameGrid.Columns; col++)
        {
            grid[18, col] = 1;
            grid[19, col] = 1;
        }
        
        // Act
        int cleared = grid.ClearFullRows();
        
        // Assert
        cleared.Should().Be(2, "two rows should be cleared");
    }
    
    [Fact]
    public void ClearFullRows_ShouldApplyGravity()
    {
        // Arrange
        GameGrid grid = new GameGrid();
        grid[17, 0] = 1; // Block above full row
        for (int col = 0; col < GameGrid.Columns; col++)
        {
            grid[19, col] = 1; // Full row at bottom
        }
        
        // Act
        int cleared = grid.ClearFullRows();
        
        // Assert
        cleared.Should().Be(1);
        grid[17, 0].Should().Be(0, "block should have moved down");
        grid[18, 0].Should().Be(1, "block should now be one row lower");
    }
    
    [Fact]
    public void ClearFullRows_WithNonConsecutiveFullRows_ShouldClearCorrectly()
    {
        // Arrange
        GameGrid grid = new GameGrid();
        // Fill row 19 (bottom)
        for (int col = 0; col < GameGrid.Columns; col++)
        {
            grid[19, col] = 1;
        }
        // Fill row 17 (leave 18 empty)
        for (int col = 0; col < GameGrid.Columns; col++)
        {
            grid[17, col] = 1;
        }
        // Add a block above
        grid[16, 5] = 1;
        
        // Act
        int cleared = grid.ClearFullRows();
        
        // Assert
        cleared.Should().Be(2, "two non-consecutive rows should be cleared");
        grid[18, 5].Should().Be(1, "block should fall down 2 rows");
    }
    
    [Fact]
    public void ClearFullRows_ComplexScenario_ShouldHandleCorrectly()
    {
        // Arrange
        GameGrid grid = new GameGrid();
        // Create pattern: block at row 15, full row at 19
        grid[15, 0] = 1;
        for (int col = 0; col < GameGrid.Columns; col++)
        {
            grid[19, col] = 1;
        }
        
        // Act
        int cleared = grid.ClearFullRows();
        
        // Assert
        cleared.Should().Be(1);
        grid[15, 0].Should().Be(0, "block should have moved");
        grid[16, 0].Should().Be(1, "block should be one row lower");
        grid[19, 0].Should().Be(0, "bottom row should be empty");
    }
    
    [Fact]
    public void Reset_ShouldClearAllCells()
    {
        // Arrange
        GameGrid grid = new GameGrid();
        for (int row = 0; row < GameGrid.Rows; row++)
        {
            for (int col = 0; col < GameGrid.Columns; col++)
            {
                grid[row, col] = 1;
            }
        }
        
        // Act
        grid.Reset();
        
        // Assert
        for (int row = 0; row < GameGrid.Rows; row++)
        {
            for (int col = 0; col < GameGrid.Columns; col++)
            {
                grid[row, col].Should().Be(0, $"cell ({row}, {col}) should be empty after reset");
            }
        }
    }
    
    [Fact]
    public void Reset_OnAlreadyEmptyGrid_ShouldHaveNoEffect()
    {
        // Arrange
        GameGrid grid = new GameGrid();
        
        // Act
        grid.Reset();
        
        // Assert
        for (int row = 0; row < GameGrid.Rows; row++)
        {
            for (int col = 0; col < GameGrid.Columns; col++)
            {
                grid[row, col].Should().Be(0);
            }
        }
    }
    
    [Fact]
    public void ClearFullRows_WithFourConsecutiveFullRows_ShouldClearAll()
    {
        // Arrange
        GameGrid grid = new GameGrid();
        // Fill bottom 4 rows (Tetris!)
        for (int row = 16; row < 20; row++)
        {
            for (int col = 0; col < GameGrid.Columns; col++)
            {
                grid[row, col] = 1;
            }
        }
        
        // Act
        int cleared = grid.ClearFullRows();
        
        // Assert
        cleared.Should().Be(4, "all four rows should be cleared");
        for (int row = 16; row < 20; row++)
        {
            grid.IsRowFull(row).Should().BeFalse($"row {row} should be empty");
        }
    }
    
    [Fact]
    public void ClearFullRows_WithBlocksAboveFullRows_ShouldMoveThemDown()
    {
        // Arrange
        GameGrid grid = new GameGrid();
        // Add blocks at various heights
        grid[10, 3] = 1;
        grid[12, 5] = 1;
        grid[15, 7] = 1;
        // Fill bottom row
        for (int col = 0; col < GameGrid.Columns; col++)
        {
            grid[19, col] = 1;
        }
        
        // Act
        int cleared = grid.ClearFullRows();
        
        // Assert
        cleared.Should().Be(1);
        grid[11, 3].Should().Be(1, "block should move down 1 row");
        grid[13, 5].Should().Be(1, "block should move down 1 row");
        grid[16, 7].Should().Be(1, "block should move down 1 row");
    }
}
