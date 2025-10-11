namespace Tetris4Dummies.Models;

/// <summary>
/// Manages the overall game state
/// </summary>
public class GameState
{
    private readonly GameGrid _grid;
    private GamePiece? _currentPiece;
    private readonly Random _random;
    
    public int Score { get; private set; }
    public bool IsGameOver { get; private set; }
    
    public GameGrid Grid => _grid;
    public GamePiece? CurrentPiece => _currentPiece;
    
    public GameState()
    {
        _grid = new GameGrid();
        _random = new Random();
        Score = 0;
        IsGameOver = false;
    }
    
    /// <summary>
    /// Starts a new game
    /// </summary>
    public void StartNewGame()
    {
        _grid.Reset();
        Score = 0;
        IsGameOver = false;
        SpawnNewPiece();
    }
    
    /// <summary>
    /// Spawns a new piece at the top center of the grid
    /// </summary>
    private void SpawnNewPiece()
    {
        int startColumn = GameGrid.Columns / 2;
        _currentPiece = new GamePiece(startColumn);
        
        // Check if the spawn position is already occupied (game over)
        if (!CanPlacePiece(_currentPiece))
        {
            IsGameOver = true;
        }
    }
    
    /// <summary>
    /// Checks if a piece can be placed at its current position
    /// </summary>
    private bool CanPlacePiece(GamePiece piece)
    {
        return _grid.IsEmpty(piece.Row, piece.Column);
    }
    
    /// <summary>
    /// Attempts to move the current piece down
    /// Returns true if successful, false if the piece has landed
    /// </summary>
    public bool MoveDown()
    {
        if (_currentPiece == null || IsGameOver)
            return false;
        
        _currentPiece.MoveDown();
        
        if (!CanPlacePiece(_currentPiece))
        {
            // Can't move down, so move back up and lock the piece
            _currentPiece.Row--;
            LockCurrentPiece();
            return false;
        }
        
        return true;
    }
    
    /// <summary>
    /// Attempts to move the current piece left
    /// </summary>
    public void MoveLeft()
    {
        if (_currentPiece == null || IsGameOver)
            return;
        
        _currentPiece.MoveLeft();
        
        if (!CanPlacePiece(_currentPiece))
        {
            _currentPiece.MoveRight(); // Move back
        }
    }
    
    /// <summary>
    /// Attempts to move the current piece right
    /// </summary>
    public void MoveRight()
    {
        if (_currentPiece == null || IsGameOver)
            return;
        
        _currentPiece.MoveRight();
        
        if (!CanPlacePiece(_currentPiece))
        {
            _currentPiece.MoveLeft(); // Move back
        }
    }
    
    /// <summary>
    /// Drops the current piece instantly to the bottom
    /// </summary>
    public void Drop()
    {
        if (_currentPiece == null || IsGameOver)
            return;
        
        while (MoveDown())
        {
            // Keep moving down until it can't
        }
    }
    
    /// <summary>
    /// Locks the current piece in place and spawns a new one
    /// </summary>
    private void LockCurrentPiece()
    {
        if (_currentPiece == null)
            return;
        
        _grid[_currentPiece.Row, _currentPiece.Column] = 1;
        
        // Clear any full rows and update score
        int linesCleared = _grid.ClearFullRows();
        UpdateScore(linesCleared);
        
        // Spawn new piece
        SpawnNewPiece();
    }
    
    /// <summary>
    /// Updates the score based on lines cleared
    /// </summary>
    private void UpdateScore(int linesCleared)
    {
        // Simple scoring: 100 points per line
        Score += linesCleared * 100;
    }
}
