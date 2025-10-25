namespace Tetris4Dummies.Core.Helpers;

/// <summary>
/// Abstraction for random number generation to enable testability
/// </summary>
public interface IRandomProvider
{
    /// <summary>
    /// Returns a non-negative random integer
    /// </summary>
    int Next();
    
    /// <summary>
    /// Returns a non-negative random integer that is less than the specified maximum
    /// </summary>
    int Next(int maxValue);
    
    /// <summary>
    /// Returns a random integer that is within a specified range
    /// </summary>
    int Next(int minValue, int maxValue);
}
