namespace Tetris4Dummies.Core.Models;

/// <summary>
/// Represents a single-block Tetris piece (for dummies!)
/// </summary>
public class GamePiece
{
    public int Row { get; set; }
    public int Column { get; set; }
    
    /// <summary>
    /// Color index (1-7) for classic Tetris colors
    /// </summary>
    public int ColorIndex { get; set; }
    
    public GamePiece(int startColumn) : this(startColumn, 1)
    {
    }
    
    public GamePiece(int startColumn, int colorIndex)
    {
        Row = 0;
        Column = startColumn;
        ColorIndex = colorIndex;
    }
    
    /// <summary>
    /// Moves the piece down by one row
    /// </summary>
    public void MoveDown()
    {
        Row++;
    }
    
    /// <summary>
    /// Moves the piece left by one column
    /// </summary>
    public void MoveLeft()
    {
        Column--;
    }
    
    /// <summary>
    /// Moves the piece right by one column
    /// </summary>
    public void MoveRight()
    {
        Column++;
    }
    
    /// <summary>
    /// Resets the piece to the starting position
    /// </summary>
    public void Reset(int startColumn)
    {
        Row = 0;
        Column = startColumn;
    }
}
