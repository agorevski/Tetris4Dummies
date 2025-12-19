using Microsoft.Maui.Graphics;
using Tetris4Dummies.Core.Models;

namespace Tetris4Dummies.Presentation.Graphics;

/// <summary>
/// Handles drawing the next piece preview
/// </summary>
public class NextPieceDrawable : IDrawable
{
    private readonly GameState _gameState;
    private const float BlockSize = 25f;
    private const float BevelSize = 3f;
    
    public NextPieceDrawable(GameState gameState)
    {
        _gameState = gameState;
    }
    
    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.FillColor = Color.FromArgb("#1a1a1a");
        canvas.FillRectangle(dirtyRect);
        
        if (_gameState.NextPiece != null)
        {
            DrawNextPiece(canvas, dirtyRect);
        }
    }
    
    private void DrawNextPiece(ICanvas canvas, RectF dirtyRect)
    {
        if (_gameState.NextPiece == null)
            return;
        
        int colorIndex = _gameState.NextPiece.ColorIndex;
        Color baseColor = TetrisColorPalette.GetColor(colorIndex);
        
        // Center the block in the preview area
        float x = (dirtyRect.Width - BlockSize) / 2;
        float y = (dirtyRect.Height - BlockSize) / 2;
        
        TetrisColorPalette.DrawBeveledBlock(canvas, x, y, BlockSize, baseColor, BevelSize);
    }
}
