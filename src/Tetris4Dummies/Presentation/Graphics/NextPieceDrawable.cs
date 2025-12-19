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
    
    // Tetris color palette matching Colors.xaml
    private static readonly Color[] TetrisColors = new[]
    {
        Color.FromArgb("#00F0F0"), // 1: Cyan (I-piece)
        Color.FromArgb("#F0F000"), // 2: Yellow (O-piece)
        Color.FromArgb("#A000F0"), // 3: Purple (T-piece)
        Color.FromArgb("#00F000"), // 4: Green (S-piece)
        Color.FromArgb("#F00000"), // 5: Red (Z-piece)
        Color.FromArgb("#0000F0"), // 6: Blue (J-piece)
        Color.FromArgb("#F0A000")  // 7: Orange (L-piece)
    };
    
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
        if (colorIndex < 1 || colorIndex > TetrisColors.Length)
            return;
        
        Color baseColor = TetrisColors[colorIndex - 1];
        
        // Center the block in the preview area
        float x = (dirtyRect.Width - BlockSize) / 2;
        float y = (dirtyRect.Height - BlockSize) / 2;
        
        DrawBeveledBlock(canvas, x, y, baseColor);
    }
    
    private void DrawBeveledBlock(ICanvas canvas, float x, float y, Color baseColor)
    {
        // Draw main block
        canvas.FillColor = baseColor;
        canvas.FillRectangle(x, y, BlockSize, BlockSize);
        
        // Draw 3D bevel effect
        float bevelSize = 3f;
        
        // Top-left highlight (lighter)
        canvas.FillColor = baseColor.WithAlpha(1f).AddLuminosity(0.3f);
        
        // Top edge
        canvas.FillRectangle(x, y, BlockSize, bevelSize);
        
        // Left edge
        canvas.FillRectangle(x, y, bevelSize, BlockSize);
        
        // Bottom-right shadow (darker)
        canvas.FillColor = baseColor.WithAlpha(1f).AddLuminosity(-0.3f);
        
        // Bottom edge
        canvas.FillRectangle(x, y + BlockSize - bevelSize, BlockSize, bevelSize);
        
        // Right edge
        canvas.FillRectangle(x + BlockSize - bevelSize, y, bevelSize, BlockSize);
        
        // Inner border for more definition
        canvas.StrokeColor = baseColor.AddLuminosity(-0.1f);
        canvas.StrokeSize = 1f;
        canvas.DrawRectangle(x + 1, y + 1, BlockSize - 2, BlockSize - 2);
    }
}
