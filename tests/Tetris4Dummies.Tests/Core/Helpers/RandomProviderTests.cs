using FluentAssertions;
using Tetris4Dummies.Core.Helpers;

namespace Tetris4Dummies.Tests.Core.Helpers;

/// <summary>
/// Unit tests for RandomProvider class
/// </summary>
public class RandomProviderTests
{
    [Fact]
    public void Next_ShouldReturnNonNegativeNumber()
    {
        // Arrange
        var provider = new RandomProvider();
        
        // Act
        int result = provider.Next();
        
        // Assert
        result.Should().BeGreaterThanOrEqualTo(0);
    }
    
    [Fact]
    public void Next_WithMaxValue_ShouldReturnNumberLessThanMax()
    {
        // Arrange
        var provider = new RandomProvider();
        const int maxValue = 100;
        
        // Act
        int result = provider.Next(maxValue);
        
        // Assert
        result.Should().BeGreaterThanOrEqualTo(0);
        result.Should().BeLessThan(maxValue);
    }
    
    [Fact]
    public void Next_WithMinAndMaxValue_ShouldReturnNumberInRange()
    {
        // Arrange
        var provider = new RandomProvider();
        const int minValue = 10;
        const int maxValue = 20;
        
        // Act
        int result = provider.Next(minValue, maxValue);
        
        // Assert
        result.Should().BeGreaterThanOrEqualTo(minValue);
        result.Should().BeLessThan(maxValue);
    }
    
    [Fact]
    public void Next_WithMinAndMaxValue_MultipleCallsShouldStayInRange()
    {
        // Arrange
        var provider = new RandomProvider();
        const int minValue = 1;
        const int maxValue = 8;
        
        // Act & Assert
        for (int i = 0; i < 100; i++)
        {
            int result = provider.Next(minValue, maxValue);
            result.Should().BeGreaterThanOrEqualTo(minValue);
            result.Should().BeLessThan(maxValue);
        }
    }
    
    [Fact]
    public void Constructor_ShouldCreateValidInstance()
    {
        // Act
        var provider = new RandomProvider();
        
        // Assert
        provider.Should().NotBeNull();
        provider.Should().BeAssignableTo<IRandomProvider>();
    }
}
