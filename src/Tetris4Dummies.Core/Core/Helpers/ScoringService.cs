namespace Tetris4Dummies.Core.Helpers;

/// <summary>
/// Implements classic Tetris scoring rules
/// Extracts scoring responsibility from GameState (Anti-Pattern #3)
/// </summary>
public class ScoringService : IScoringService
{
    // Classic Tetris scoring constants
    public const int SingleLineScore = 40;
    public const int DoubleLineScore = 100;
    public const int TripleLineScore = 300;
    public const int TetrisScore = 1200;
    
    // Progression constant
    private const int DefaultLinesPerLevel = 10;
    
    /// <inheritdoc />
    public int LinesPerLevel => DefaultLinesPerLevel;
    
    /// <inheritdoc />
    public int CalculateScore(int linesCleared, int level)
    {
        if (linesCleared <= 0)
            return 0;
        
        int basePoints = linesCleared switch
        {
            1 => SingleLineScore,
            2 => DoubleLineScore,
            3 => TripleLineScore,
            4 => TetrisScore,
            _ => linesCleared * SingleLineScore
        };
        
        return basePoints * level;
    }
    
    /// <inheritdoc />
    public int CalculateLevel(int totalLinesCleared)
    {
        return (totalLinesCleared / LinesPerLevel) + 1;
    }
}
