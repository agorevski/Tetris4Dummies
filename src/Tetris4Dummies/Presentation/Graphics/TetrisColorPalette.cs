using Microsoft.Maui.Graphics;

namespace Tetris4Dummies.Presentation.Graphics;

/// <summary>
/// Shared color palette for Tetris pieces
/// </summary>
public static class TetrisColorPalette
{
    /// <summary>
    /// Classic Tetris color palette indexed 1-7
    /// </summary>
    public static readonly Color[] Colors = new[]
    {
        Color.FromArgb("#00F0F0"), // 1: Cyan (I-piece)
        Color.FromArgb("#F0F000"), // 2: Yellow (O-piece)
        Color.FromArgb("#A000F0"), // 3: Purple (T-piece)
        Color.FromArgb("#00F000"), // 4: Green (S-piece)
        Color.FromArgb("#F00000"), // 5: Red (Z-piece)
        Color.FromArgb("#0000F0"), // 6: Blue (J-piece)
        Color.FromArgb("#F0A000")  // 7: Orange (L-piece)
    };

    /// <summary>
    /// Gets the color for a given color index (1-7)
    /// </summary>
    public static Color GetColor(int colorIndex)
    {
        if (colorIndex >= 1 && colorIndex <= Colors.Length)
        {
            return Colors[colorIndex - 1];
        }
        return Microsoft.Maui.Graphics.Colors.Red; // Fallback color
    }

    /// <summary>
    /// Draws a 3D beveled block at the specified position
    /// </summary>
    public static void DrawBeveledBlock(ICanvas canvas, float x, float y, float size, Color baseColor, float bevelSize = 3f)
    {
        // Draw main block
        canvas.FillColor = baseColor;
        canvas.FillRectangle(x, y, size, size);

        // Top-left highlight (lighter)
        canvas.FillColor = baseColor.AddLuminosity(0.3f);
        canvas.FillRectangle(x, y, size, bevelSize); // Top edge
        canvas.FillRectangle(x, y, bevelSize, size); // Left edge

        // Bottom-right shadow (darker)
        canvas.FillColor = baseColor.AddLuminosity(-0.3f);
        canvas.FillRectangle(x, y + size - bevelSize, size, bevelSize); // Bottom edge
        canvas.FillRectangle(x + size - bevelSize, y, bevelSize, size); // Right edge

        // Inner border for definition
        canvas.StrokeColor = baseColor.AddLuminosity(-0.1f);
        canvas.StrokeSize = 1f;
        canvas.DrawRectangle(x + 1, y + 1, size - 2, size - 2);
    }
}
