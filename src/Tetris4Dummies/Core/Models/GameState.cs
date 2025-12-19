using Tetris4Dummies.Core.Helpers;

namespace Tetris4Dummies.Core.Models;

/// <summary>
/// Manages the overall game state
/// </summary>
public class GameState
{
    private readonly GameGrid _grid;
    private GamePiece? _currentPiece;
    private GamePiece? _nextPiece;
    private readonly IRandomProvider _random;
    
    public int Score { get; private set; }
    public int Level { get; private set; }
    public int Lines { get; private set; }
    public bool IsGameOver { get; private set; }
    
    public GameGrid Grid => _grid;
    public GamePiece? CurrentPiece => _currentPiece;
    public GamePiece? NextPiece => _nextPiece;
    
    public GameState() : this(new RandomProvider())
    {
    }
    
    /// <summary>
    /// Constructor with dependency injection for testability
    /// </summary>
    public GameState(IRandomProvider randomProvider)
    {
        _grid = new GameGrid();
        _random = randomProvider;
        Score = 0;
        Level = 1;
        Lines = 0;
        IsGameOver = false;
    }
    
    /// <summary>
    /// Starts a new game
    /// </summary>
    public void StartNewGame()
    {
        // Check if spawn position is blocked before resetting (for game over detection)
        bool spawnBlocked = !_grid.IsEmpty(0, GameGrid.Columns / 2);
        
        _grid.Reset();
        Score = 0;
        Level = 1;
        Lines = 0;
        IsGameOver = false;
        
        // Generate the next piece first
        _nextPiece = CreateNewPiece();
        
        // If spawn was blocked before reset, set game over after spawning
        if (spawnBlocked)
        {
            SpawnNewPiece();
            IsGameOver = true;
        }
        else
        {
            SpawnNewPiece();
        }
    }
    
    /// <summary>
    /// Creates a new piece with a random color
    /// </summary>
    private GamePiece CreateNewPiece()
    {
        int startColumn = GameGrid.Columns / 2;
        int colorIndex = _random.Next(1, 8); // Random color 1-7
        return new GamePiece(startColumn, colorIndex);
    }
    
    /// <summary>
    /// Spawns a new piece at the top center of the grid
    /// </summary>
    private void SpawnNewPiece()
    {
        // Use the next piece if available, otherwise create new
        if (_nextPiece != null)
        {
            _currentPiece = _nextPiece;
            _nextPiece = CreateNewPiece();
        }
        else
        {
            _currentPiece = CreateNewPiece();
            _nextPiece = CreateNewPiece();
        }
        
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
        
        // Store the color index instead of just 1
        _grid[_currentPiece.Row, _currentPiece.Column] = _currentPiece.ColorIndex;
        
        // Clear any full rows and update score
        int linesCleared = _grid.ClearFullRows();
        UpdateScore(linesCleared);
        
        // Spawn new piece
        SpawnNewPiece();
    }
    
    /// <summary>
    /// Updates the score based on lines cleared using classic Tetris scoring
    /// </summary>
    private void UpdateScore(int linesCleared)
    {
        if (linesCleared == 0)
            return;
        
        Lines += linesCleared;
        
        // Classic Tetris scoring: points * level
        int basePoints = linesCleared switch
        {
            1 => 40,
            2 => 100,
            3 => 300,
            4 => 1200, // Tetris!
            _ => linesCleared * 40
        };
        Score += basePoints * Level;
        
        // Level increases every 10 lines
        Level = (Lines / 10) + 1;
    }
}
