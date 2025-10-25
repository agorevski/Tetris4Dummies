namespace Tetris4Dummies.Core.Models;

/// <summary>
/// Represents the Tetris game grid
/// </summary>
public class GameGrid
{
    public const int Rows = 20;
    public const int Columns = 10;
    
    private readonly int[,] _grid;
    
    public GameGrid()
    {
        _grid = new int[Rows, Columns];
    }
    
    /// <summary>
    /// Gets the value at the specified position (0 = empty, 1 = filled)
    /// </summary>
    public int this[int row, int col]
    {
        get => _grid[row, col];
        set => _grid[row, col] = value;
    }
    
    /// <summary>
    /// Checks if the specified position is within grid bounds
    /// </summary>
    public bool IsInBounds(int row, int col)
    {
        return row >= 0 && row < Rows && col >= 0 && col < Columns;
    }
    
    /// <summary>
    /// Checks if the specified position is empty
    /// </summary>
    public bool IsEmpty(int row, int col)
    {
        return IsInBounds(row, col) && _grid[row, col] == 0;
    }
    
    /// <summary>
    /// Checks if a row is full
    /// </summary>
    public bool IsRowFull(int row)
    {
        for (int col = 0; col < Columns; col++)
        {
            if (_grid[row, col] == 0)
                return false;
        }
        return true;
    }
    
    /// <summary>
    /// Clears a row by setting all cells to 0
    /// </summary>
    public void ClearRow(int row)
    {
        for (int col = 0; col < Columns; col++)
        {
            _grid[row, col] = 0;
        }
    }
    
    /// <summary>
    /// Moves a row down by one
    /// </summary>
    public void MoveRowDown(int row, int numRows)
    {
        for (int col = 0; col < Columns; col++)
        {
            _grid[row + numRows, col] = _grid[row, col];
            _grid[row, col] = 0;
        }
    }
    
    /// <summary>
    /// Clears all full rows and returns the number of rows cleared
    /// </summary>
    public int ClearFullRows()
    {
        int cleared = 0;
        
        for (int row = Rows - 1; row >= 0; row--)
        {
            if (IsRowFull(row))
            {
                ClearRow(row);
                cleared++;
            }
            else if (cleared > 0)
            {
                MoveRowDown(row, cleared);
            }
        }
        
        return cleared;
    }
    
    /// <summary>
    /// Resets the grid to empty state
    /// </summary>
    public void Reset()
    {
        for (int row = 0; row < Rows; row++)
        {
            for (int col = 0; col < Columns; col++)
            {
                _grid[row, col] = 0;
            }
        }
    }
}
