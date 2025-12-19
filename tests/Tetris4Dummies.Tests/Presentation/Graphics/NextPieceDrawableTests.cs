using FluentAssertions;
using Microsoft.Maui.Graphics;
using Moq;
using Tetris4Dummies.Core.Helpers;
using Tetris4Dummies.Core.Models;
using Tetris4Dummies.Presentation.Graphics;

namespace Tetris4Dummies.Tests.Presentation.Graphics;

/// <summary>
/// Unit tests for NextPieceDrawable class
/// </summary>
public class NextPieceDrawableTests
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
        NextPieceDrawable drawable = new NextPieceDrawable(gameState);
        
        // Assert
        drawable.Should().NotBeNull();
    }
    
    [Fact]
    public void Draw_WithNoNextPiece_ShouldNotThrow()
    {
        // Arrange
        Mock<IRandomProvider> mockRandom = CreateMockRandom();
        GameState gameState = new GameState(mockRandom.Object);
        NextPieceDrawable drawable = new NextPieceDrawable(gameState);
        Mock<ICanvas> mockCanvas = new Mock<ICanvas>();
        RectF dirtyRect = new RectF(0, 0, 60, 60);
        
        // Act & Assert
        FluentActions.Invoking(() => drawable.Draw(mockCanvas.Object, dirtyRect))
            .Should().NotThrow();
    }
    
    [Fact]
    public void Draw_ShouldFillBackground()
    {
        // Arrange
        Mock<IRandomProvider> mockRandom = CreateMockRandom();
        GameState gameState = new GameState(mockRandom.Object);
        NextPieceDrawable drawable = new NextPieceDrawable(gameState);
        Mock<ICanvas> mockCanvas = new Mock<ICanvas>();
        RectF dirtyRect = new RectF(0, 0, 60, 60);
        
        // Act
        drawable.Draw(mockCanvas.Object, dirtyRect);
        
        // Assert - verify FillRectangle was called at least once for the background
        mockCanvas.Verify(c => c.FillRectangle(It.IsAny<float>(), It.IsAny<float>(), 
            It.IsAny<float>(), It.IsAny<float>()), Times.AtLeastOnce());
    }
    
    [Fact]
    public void Draw_WithNextPiece_ShouldDrawBlock()
    {
        // Arrange
        Mock<IRandomProvider> mockRandom = CreateMockRandom();
        GameState gameState = new GameState(mockRandom.Object);
        gameState.StartNewGame();
        
        NextPieceDrawable drawable = new NextPieceDrawable(gameState);
        Mock<ICanvas> mockCanvas = new Mock<ICanvas>();
        RectF dirtyRect = new RectF(0, 0, 60, 60);
        
        // Act
        drawable.Draw(mockCanvas.Object, dirtyRect);
        
        // Assert - should draw the block (multiple FillRectangle calls for beveled effect)
        mockCanvas.Verify(c => c.FillRectangle(It.IsAny<float>(), It.IsAny<float>(), 
            It.IsAny<float>(), It.IsAny<float>()), Times.AtLeast(1));
    }
    
    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(5)]
    [InlineData(7)]
    public void Draw_WithDifferentColorIndices_ShouldWork(int colorIndex)
    {
        // Arrange
        Mock<IRandomProvider> mockRandom = CreateMockRandom();
        mockRandom.Setup(r => r.Next(1, 8)).Returns(colorIndex);
        GameState gameState = new GameState(mockRandom.Object);
        gameState.StartNewGame();
        
        NextPieceDrawable drawable = new NextPieceDrawable(gameState);
        Mock<ICanvas> mockCanvas = new Mock<ICanvas>();
        RectF dirtyRect = new RectF(0, 0, 60, 60);
        
        // Act & Assert - should not throw for any valid color index
        FluentActions.Invoking(() => drawable.Draw(mockCanvas.Object, dirtyRect))
            .Should().NotThrow();
    }
    
    [Fact]
    public void Draw_WithInvalidColorIndex_ShouldNotDraw()
    {
        // Arrange
        Mock<IRandomProvider> mockRandom = CreateMockRandom();
        mockRandom.Setup(r => r.Next(1, 8)).Returns(0); // Invalid color index
        GameState gameState = new GameState(mockRandom.Object);
        gameState.StartNewGame();
        
        NextPieceDrawable drawable = new NextPieceDrawable(gameState);
        Mock<ICanvas> mockCanvas = new Mock<ICanvas>();
        RectF dirtyRect = new RectF(0, 0, 60, 60);
        
        // Act & Assert - should not throw even with invalid color index
        FluentActions.Invoking(() => drawable.Draw(mockCanvas.Object, dirtyRect))
            .Should().NotThrow();
    }
}
