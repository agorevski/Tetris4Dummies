namespace Tetris4Dummies.Core.Helpers;

/// <summary>
/// Interface for scoring calculation to enable testability and separation of concerns
/// </summary>
public interface IScoringService
{
    /// <summary>
    /// Calculates the score for clearing lines at a given level
    /// </summary>
    /// <param name="linesCleared">Number of lines cleared (1-4)</param>
    /// <param name="level">Current game level</param>
    /// <returns>Points earned</returns>
    int CalculateScore(int linesCleared, int level);
    
    /// <summary>
    /// Calculates the level based on total lines cleared
    /// </summary>
    /// <param name="totalLinesCleared">Total number of lines cleared in the game</param>
    /// <returns>Current level</returns>
    int CalculateLevel(int totalLinesCleared);
    
    /// <summary>
    /// Gets the number of lines required to advance one level
    /// </summary>
    int LinesPerLevel { get; }
}
