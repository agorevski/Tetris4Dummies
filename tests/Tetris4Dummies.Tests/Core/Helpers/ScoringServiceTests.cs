using FluentAssertions;
using Tetris4Dummies.Core.Helpers;

namespace Tetris4Dummies.Tests.Core.Helpers;

/// <summary>
/// Unit tests for ScoringService class
/// </summary>
public class ScoringServiceTests
{
    private readonly ScoringService _scoringService = new();
    
    #region CalculateScore Tests
    
    [Fact]
    public void CalculateScore_WithZeroLines_ShouldReturnZero()
    {
        // Act
        int score = _scoringService.CalculateScore(0, 1);
        
        // Assert
        score.Should().Be(0, "no lines cleared means no points");
    }
    
    [Fact]
    public void CalculateScore_WithNegativeLines_ShouldReturnZero()
    {
        // Act
        int score = _scoringService.CalculateScore(-1, 1);
        
        // Assert
        score.Should().Be(0, "negative lines should return zero");
    }
    
    [Fact]
    public void CalculateScore_SingleLine_Level1_ShouldReturn40()
    {
        // Act
        int score = _scoringService.CalculateScore(1, 1);
        
        // Assert
        score.Should().Be(40, "single line at level 1 gives 40 points");
    }
    
    [Fact]
    public void CalculateScore_DoubleLine_Level1_ShouldReturn100()
    {
        // Act
        int score = _scoringService.CalculateScore(2, 1);
        
        // Assert
        score.Should().Be(100, "double line at level 1 gives 100 points");
    }
    
    [Fact]
    public void CalculateScore_TripleLine_Level1_ShouldReturn300()
    {
        // Act
        int score = _scoringService.CalculateScore(3, 1);
        
        // Assert
        score.Should().Be(300, "triple line at level 1 gives 300 points");
    }
    
    [Fact]
    public void CalculateScore_Tetris_Level1_ShouldReturn1200()
    {
        // Act
        int score = _scoringService.CalculateScore(4, 1);
        
        // Assert
        score.Should().Be(1200, "Tetris at level 1 gives 1200 points");
    }
    
    [Fact]
    public void CalculateScore_SingleLine_Level2_ShouldReturn80()
    {
        // Act
        int score = _scoringService.CalculateScore(1, 2);
        
        // Assert
        score.Should().Be(80, "single line at level 2 gives 80 points (40 × 2)");
    }
    
    [Fact]
    public void CalculateScore_Tetris_Level5_ShouldReturn6000()
    {
        // Act
        int score = _scoringService.CalculateScore(4, 5);
        
        // Assert
        score.Should().Be(6000, "Tetris at level 5 gives 6000 points (1200 × 5)");
    }
    
    [Fact]
    public void CalculateScore_MoreThanFourLines_ShouldUseFallback()
    {
        // For edge case: more than 4 lines uses linesCleared * SingleLineScore
        // Act
        int score = _scoringService.CalculateScore(5, 1);
        
        // Assert
        score.Should().Be(200, "5 lines at level 1 uses fallback: 5 × 40 = 200");
    }
    
    #endregion
    
    #region CalculateLevel Tests
    
    [Fact]
    public void CalculateLevel_WithZeroLines_ShouldReturnLevel1()
    {
        // Act
        int level = _scoringService.CalculateLevel(0);
        
        // Assert
        level.Should().Be(1, "0 lines cleared is level 1");
    }
    
    [Fact]
    public void CalculateLevel_With9Lines_ShouldReturnLevel1()
    {
        // Act
        int level = _scoringService.CalculateLevel(9);
        
        // Assert
        level.Should().Be(1, "9 lines cleared is still level 1");
    }
    
    [Fact]
    public void CalculateLevel_With10Lines_ShouldReturnLevel2()
    {
        // Act
        int level = _scoringService.CalculateLevel(10);
        
        // Assert
        level.Should().Be(2, "10 lines cleared is level 2");
    }
    
    [Fact]
    public void CalculateLevel_With25Lines_ShouldReturnLevel3()
    {
        // Act
        int level = _scoringService.CalculateLevel(25);
        
        // Assert
        level.Should().Be(3, "25 lines cleared is level 3 (25/10 + 1 = 3)");
    }
    
    [Fact]
    public void CalculateLevel_With100Lines_ShouldReturnLevel11()
    {
        // Act
        int level = _scoringService.CalculateLevel(100);
        
        // Assert
        level.Should().Be(11, "100 lines cleared is level 11");
    }
    
    #endregion
    
    #region LinesPerLevel Property Tests
    
    [Fact]
    public void LinesPerLevel_ShouldReturn10()
    {
        // Assert
        _scoringService.LinesPerLevel.Should().Be(10, "default lines per level is 10");
    }
    
    #endregion
    
    #region Scoring Constants Tests
    
    [Fact]
    public void ScoringConstants_ShouldMatchClassicTetris()
    {
        // Assert
        ScoringService.SingleLineScore.Should().Be(40);
        ScoringService.DoubleLineScore.Should().Be(100);
        ScoringService.TripleLineScore.Should().Be(300);
        ScoringService.TetrisScore.Should().Be(1200);
    }
    
    #endregion
}
