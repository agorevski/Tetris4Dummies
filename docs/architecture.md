# Project Architecture

This document describes the architecture, structure, and design patterns used in Tetris4Dummies.

## Technology Stack

- **Framework**: .NET MAUI (Multi-platform App UI)
- **Language**: C# 12 with .NET 9.0
- **UI Pattern**: XAML with code-behind
- **Graphics**: Microsoft.Maui.Graphics (IDrawable pattern)
- **Target Platforms**: Android (primary), iOS, Windows, macOS, Tizen

## Project Structure

```text
Tetris4Dummies/
├── Models/              # Core game logic
│   ├── GameGrid.cs      # 2D grid data structure
│   ├── GamePiece.cs     # Falling block representation
│   └── GameState.cs     # Game state management
├── Graphics/            # Custom rendering
│   └── GameDrawable.cs  # IDrawable implementation
├── Platforms/           # Platform-specific code
│   ├── Android/
│   ├── iOS/
│   ├── Windows/
│   ├── MacCatalyst/
│   └── Tizen/
├── Resources/           # Assets and resources
│   ├── Fonts/
│   ├── Images/
│   ├── Splash/
│   └── Styles/
├── MainPage.xaml[.cs]   # Main UI and game controls
├── App.xaml[.cs]        # Application lifecycle
└── AppShell.xaml[.cs]   # App navigation shell
```

## Core Components

### Models Layer

#### GameGrid

Manages the 2D game grid (20 rows × 10 columns).

**Key Methods**:

- `IsInBounds(row, col)` - Validates grid coordinates
- `IsEmpty(row, col)` - Checks if a cell is unoccupied
- `IsRowFull(row)` - Determines if a row is complete
- `ClearRow(row)` - Removes a completed row
- `MoveRowDown(row, numRows)` - Shifts rows down after clearing
- `ClearFullRows()` - Clears all full rows and returns count
- `Reset()` - Resets the grid to empty state

#### GamePiece

Represents a single falling block (1x1).

**Properties**:

- `Row` - Current row position
- `Column` - Current column position
- `Color` - Block color

**Key Methods**:

- `MoveDown()` - Moves piece down one row
- `MoveLeft()` - Moves piece left one column
- `MoveRight()` - Moves piece right one column
- `Reset(startColumn)` - Resets to top of grid

#### GameState

Orchestrates the overall game logic.

**Properties**:

- `Grid` - Reference to GameGrid
- `CurrentPiece` - The active falling piece
- `Score` - Current game score
- `IsGameOver` - Game over flag

**Key Methods**:

- `StartNewGame()` - Initializes new game
- `SpawnNewPiece()` - Creates new piece at top
- `CanPlacePiece(piece)` - Validates piece placement
- `MoveDown()` - Moves current piece down
- `MoveLeft()` - Moves current piece left
- `MoveRight()` - Moves current piece right
- `Drop()` - Instantly drops piece to bottom
- `LockCurrentPiece()` - Fixes piece in grid
- `UpdateScore(linesCleared)` - Updates score (100 points/line)

### Graphics Layer

#### GameDrawable

Implements `IDrawable` interface for custom rendering.

**Responsibilities**:

- Renders the game grid
- Draws placed blocks
- Draws the current falling piece
- Uses `Microsoft.Maui.Graphics` API for cross-platform drawing

### UI Layer

#### MainPage

Main game interface containing:

- GraphicsView for game rendering
- Control buttons (Left, Right, Drop)
- New Game button
- Score display
- Game loop timer management

**Key Features**:

- Timer-based game loop
- Touch/button controls
- Main thread synchronization for UI updates
- Proper lifecycle management (OnAppearing/OnDisappearing)

## Design Patterns

### Separation of Concerns

- **Models**: Pure game logic, no UI dependencies
- **Graphics**: Rendering logic using platform-agnostic APIs
- **UI**: Event handling and user interaction

### Code-Behind Pattern

Uses XAML for UI layout with C# code-behind for logic. This is appropriate for the game's relatively simple UI requirements.

### IDrawable Pattern

Leverages MAUI's `IDrawable` interface for custom graphics, ensuring cross-platform compatibility without platform-specific rendering code.

### Dependency Injection

Services can be registered in `MauiProgram.cs` for dependency injection throughout the app.

## Game Logic Flow

1. **Game Start**: User taps "New Game"
   - `GameState.StartNewGame()` initializes grid and spawns first piece
   - Timer starts game loop

2. **Game Loop**: Timer tick event
   - `GameState.MoveDown()` attempts to move piece down
   - If piece can't move, it's locked and a new piece spawns
   - Grid checks for full rows and clears them
   - Score updates based on cleared lines
   - UI refreshes via `GraphicsView.Invalidate()`

3. **User Input**: Button taps
   - Left/Right buttons call `GameState.MoveLeft/Right()`
   - Drop button calls `GameState.Drop()` for instant fall
   - UI refreshes after each action

4. **Game Over**: Blocks reach top
   - `GameState.IsGameOver` is set to true
   - Timer stops
   - UI displays game over state

## Performance Considerations

### Mobile Optimization

- Minimize allocations in game loop
- Reuse objects where possible
- Use efficient data structures (2D array for grid)

### Threading

- Game loop runs on timer thread
- UI updates marshaled to main thread via `MainThread.BeginInvokeOnMainThread()`

### Resource Management

- Timer properly disposed in `OnDisappearing()`
- Event handlers cleaned up to prevent memory leaks

## MAUI-Specific Features

### Cross-Platform Graphics

Uses `Microsoft.Maui.Graphics` instead of platform-specific APIs (e.g., Android Canvas, iOS CoreGraphics).

### Platform Folders

Platform-specific code goes in `Platforms/{PlatformName}/` directories, using conditional compilation when needed.

### Resource System

All assets (fonts, images, colors, styles) managed through MAUI's unified resource system.

## Coding Conventions

- **PascalCase**: Classes, methods, properties, public fields
- **camelCase with underscore**: Private fields (`_gameState`)
- **camelCase**: Local variables, parameters
- **XML Documentation**: All public APIs documented
- **Nullable Reference Types**: Enabled project-wide
- **Partial Classes**: Used for XAML code-behind

## Extension Points

### Adding New Features

- **Power-ups**: Extend `GamePiece` with special types
- **Levels**: Add difficulty scaling to `GameState`
- **High Scores**: Implement persistence layer
- **Sound Effects**: Add audio through MAUI's audio APIs

### Adding Platforms

See [Porting Guide](porting.md) for details on enabling additional platforms.

## References

- [.NET MAUI Documentation](https://learn.microsoft.com/dotnet/maui/)
- [Microsoft.Maui.Graphics](https://learn.microsoft.com/dotnet/maui/user-interface/graphics/)
- [C# Coding Conventions](https://learn.microsoft.com/dotnet/csharp/fundamentals/coding-style/coding-conventions)
