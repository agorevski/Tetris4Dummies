using Microsoft.Maui.Graphics;
using Tetris4Dummies.Core.Models;

namespace Tetris4Dummies.Presentation.Graphics;

/// <summary>
/// Handles drawing the Tetris game on a graphics canvas
/// </summary>
public class GameDrawable : IDrawable
{
    private readonly GameState _gameState;
    private const float CellSize = 30f;
    private const float GridLineWidth = 1f;
    
    public GameDrawable(GameState gameState)
    {
        _gameState = gameState;
    }
    
    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.FillColor = Colors.Black;
        canvas.FillRectangle(dirtyRect);
        
        DrawGrid(canvas);
        DrawLockedPieces(canvas);
        DrawCurrentPiece(canvas);
        
        if (_gameState.IsGameOver)
        {
            DrawGameOver(canvas, dirtyRect);
        }
    }
    
    private void DrawGrid(ICanvas canvas)
    {
        canvas.StrokeColor = Colors.Gray;
        canvas.StrokeSize = GridLineWidth;
        
        // Draw vertical lines
        for (int col = 0; col <= GameGrid.Columns; col++)
        {
            float x = col * CellSize;
            canvas.DrawLine(x, 0, x, GameGrid.Rows * CellSize);
        }
        
        // Draw horizontal lines
        for (int row = 0; row <= GameGrid.Rows; row++)
        {
            float y = row * CellSize;
            canvas.DrawLine(0, y, GameGrid.Columns * CellSize, y);
        }
    }
    
    private void DrawLockedPieces(ICanvas canvas)
    {
        canvas.FillColor = Colors.Blue;
        
        for (int row = 0; row < GameGrid.Rows; row++)
        {
            for (int col = 0; col < GameGrid.Columns; col++)
            {
                if (_gameState.Grid[row, col] == 1)
                {
                    DrawCell(canvas, row, col);
                }
            }
        }
    }
    
    private void DrawCurrentPiece(ICanvas canvas)
    {
        if (_gameState.CurrentPiece == null)
            return;
        
        canvas.FillColor = Colors.Red;
        DrawCell(canvas, _gameState.CurrentPiece.Row, _gameState.CurrentPiece.Column);
    }
    
    private void DrawCell(ICanvas canvas, int row, int col)
    {
        float x = col * CellSize + GridLineWidth;
        float y = row * CellSize + GridLineWidth;
        float size = CellSize - 2 * GridLineWidth;
        
        canvas.FillRectangle(x, y, size, size);
    }
    
    private void DrawGameOver(ICanvas canvas, RectF dirtyRect)
    {
        // Semi-transparent overlay
        canvas.FillColor = Colors.Black.WithAlpha(0.7f);
        canvas.FillRectangle(dirtyRect);
        
        // Game Over text
        canvas.FontColor = Colors.White;
        canvas.FontSize = 24;
        canvas.DrawString("GAME OVER", 
            dirtyRect.Width / 2, 
            dirtyRect.Height / 2 - 20,
            HorizontalAlignment.Center);
        
        canvas.FontSize = 16;
        canvas.DrawString($"Score: {_gameState.Score}", 
            dirtyRect.Width / 2, 
            dirtyRect.Height / 2 + 20,
            HorizontalAlignment.Center);
    }
}
