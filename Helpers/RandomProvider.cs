namespace Tetris4Dummies.Helpers;

/// <summary>
/// Default implementation of IRandomProvider using System.Random
/// </summary>
public class RandomProvider : IRandomProvider
{
    private readonly Random _random;
    
    public RandomProvider()
    {
        _random = new Random();
    }
    
    public int Next()
    {
        return _random.Next();
    }
    
    public int Next(int maxValue)
    {
        return _random.Next(maxValue);
    }
    
    public int Next(int minValue, int maxValue)
    {
        return _random.Next(minValue, maxValue);
    }
}
