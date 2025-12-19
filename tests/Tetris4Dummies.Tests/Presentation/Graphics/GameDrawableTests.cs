using FluentAssertions;
using Microsoft.Maui.Graphics;
using Moq;
using Tetris4Dummies.Core.Helpers;
using Tetris4Dummies.Core.Models;
using Tetris4Dummies.Presentation.Graphics;

namespace Tetris4Dummies.Tests.Presentation.Graphics;

/// <summary>
/// Unit tests for GameDrawable class
/// </summary>
public class GameDrawableTests
{
    private Mock<IRandomProvider> CreateMockRandom()
    {
        var mock = new Mock<IRandomProvider>();
        mock.Setup(r => r.Next(1, 8)).Returns(1);
        return mock;
    }
    
    [Fact]
    public void Constructor_ShouldAcceptGameState()
    {
        // Arrange
        Mock<IRandomProvider> mockRandom = CreateMockRandom();
        GameState gameState = new GameState(mockRandom.Object);
        
        // Act
        GameDrawable drawable = new GameDrawable(gameState);
        
        // Assert
        drawable.Should().NotBeNull();
    }
    
    [Fact]
    public void Draw_ShouldNotThrowWhenNoPiece()
    {
        // Arrange
        Mock<IRandomProvider> mockRandom = CreateMockRandom();
        GameState gameState = new GameState(mockRandom.Object);
        GameDrawable drawable = new GameDrawable(gameState);
        Mock<ICanvas> mockCanvas = new Mock<ICanvas>();
        RectF dirtyRect = new RectF(0, 0, 300, 600);
        
        // Act & Assert
        FluentActions.Invoking(() => drawable.Draw(mockCanvas.Object, dirtyRect))
            .Should().NotThrow();
    }
    
    [Fact]
    public void Draw_ShouldFillBlackBackground()
    {
        // Arrange
        Mock<IRandomProvider> mockRandom = CreateMockRandom();
        GameState gameState = new GameState(mockRandom.Object);
        GameDrawable drawable = new GameDrawable(gameState);
        Mock<ICanvas> mockCanvas = new Mock<ICanvas>();
        RectF dirtyRect = new RectF(0, 0, 300, 600);
        
        // Act
        drawable.Draw(mockCanvas.Object, dirtyRect);
        
        // Assert
        mockCanvas.VerifySet(c => c.FillColor = Colors.Black, Times.AtLeastOnce());
    }
    
    [Fact]
    public void Draw_WithGameStarted_ShouldDrawCurrentPiece()
    {
        // Arrange
        Mock<IRandomProvider> mockRandom = CreateMockRandom();
        GameState gameState = new GameState(mockRandom.Object);
        gameState.StartNewGame();
        
        GameDrawable drawable = new GameDrawable(gameState);
        Mock<ICanvas> mockCanvas = new Mock<ICanvas>();
        RectF dirtyRect = new RectF(0, 0, 300, 600);
        
        // Act
        drawable.Draw(mockCanvas.Object, dirtyRect);
        
        // Assert - verify FillRectangle was called for the piece
        mockCanvas.Verify(c => c.FillRectangle(It.IsAny<float>(), It.IsAny<float>(), 
            It.IsAny<float>(), It.IsAny<float>()), Times.AtLeastOnce());
    }
    
    [Fact]
    public void Draw_WithLockedPieces_ShouldDrawThem()
    {
        // Arrange
        Mock<IRandomProvider> mockRandom = CreateMockRandom();
        GameState gameState = new GameState(mockRandom.Object);
        gameState.StartNewGame();
        // Lock a piece
        gameState.Grid[19, 5] = 1;
        
        GameDrawable drawable = new GameDrawable(gameState);
        Mock<ICanvas> mockCanvas = new Mock<ICanvas>();
        RectF dirtyRect = new RectF(0, 0, 300, 600);
        
        // Act
        drawable.Draw(mockCanvas.Object, dirtyRect);
        
        // Assert
        mockCanvas.Verify(c => c.FillRectangle(It.IsAny<float>(), It.IsAny<float>(), 
            It.IsAny<float>(), It.IsAny<float>()), Times.AtLeast(2));
    }
    
    [Fact]
    public void Draw_WithGameOver_ShouldDrawOverlay()
    {
        // Arrange
        Mock<IRandomProvider> mockRandom = CreateMockRandom();
        GameState gameState = new GameState(mockRandom.Object);
        // Block spawn position to trigger game over
        gameState.Grid[0, 5] = 1;
        gameState.StartNewGame();
        
        GameDrawable drawable = new GameDrawable(gameState);
        Mock<ICanvas> mockCanvas = new Mock<ICanvas>();
        RectF dirtyRect = new RectF(0, 0, 300, 600);
        
        // Act
        drawable.Draw(mockCanvas.Object, dirtyRect);
        
        // Assert - should draw game over text
        mockCanvas.Verify(c => c.DrawString(It.Is<string>(s => s.Contains("GAME OVER")), 
            It.IsAny<float>(), It.IsAny<float>(), It.IsAny<HorizontalAlignment>()), Times.Once());
    }
    
    [Fact]
    public void Draw_ShouldDrawGridLines()
    {
        // Arrange
        Mock<IRandomProvider> mockRandom = CreateMockRandom();
        GameState gameState = new GameState(mockRandom.Object);
        GameDrawable drawable = new GameDrawable(gameState);
        Mock<ICanvas> mockCanvas = new Mock<ICanvas>();
        RectF dirtyRect = new RectF(0, 0, 300, 600);
        
        // Act
        drawable.Draw(mockCanvas.Object, dirtyRect);
        
        // Assert - should draw grid lines (11 vertical + 21 horizontal = 32 lines)
        mockCanvas.Verify(c => c.DrawLine(It.IsAny<float>(), It.IsAny<float>(), 
            It.IsAny<float>(), It.IsAny<float>()), Times.AtLeast(30));
    }
    
    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(7)]
    public void Draw_WithDifferentColorIndices_ShouldWork(int colorIndex)
    {
        // Arrange
        Mock<IRandomProvider> mockRandom = CreateMockRandom();
        mockRandom.Setup(r => r.Next(1, 8)).Returns(colorIndex);
        GameState gameState = new GameState(mockRandom.Object);
        gameState.StartNewGame();
        
        GameDrawable drawable = new GameDrawable(gameState);
        Mock<ICanvas> mockCanvas = new Mock<ICanvas>();
        RectF dirtyRect = new RectF(0, 0, 300, 600);
        
        // Act & Assert - should not throw for any valid color index
        FluentActions.Invoking(() => drawable.Draw(mockCanvas.Object, dirtyRect))
            .Should().NotThrow();
    }
}
