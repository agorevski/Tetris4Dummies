# Project Architecture

This document describes the architecture, structure, and design patterns used in Tetris4Dummies.

## Technology Stack

- **Framework**: .NET MAUI (Multi-platform App UI)
- **Language**: C# 12 with .NET 9.0
- **UI Pattern**: MVVM (Model-View-ViewModel)
- **Graphics**: Microsoft.Maui.Graphics (IDrawable pattern)
- **Target Platforms**: Android (primary), iOS, Windows, macOS, Tizen

## Project Structure

```text
src/
├── Tetris4Dummies.Core/           # Shared Core library (net9.0, no MAUI dependency)
│   └── Core/
│       ├── Models/
│       │   ├── GameGrid.cs        # 2D grid data structure (20×10)
│       │   ├── GamePiece.cs       # Falling block representation
│       │   └── GameState.cs       # Game state management and orchestration
│       └── Helpers/
│           ├── IRandomProvider.cs  # Random number abstraction for testability
│           ├── RandomProvider.cs   # Default System.Random implementation
│           ├── IScoringService.cs  # Scoring calculation interface
│           ├── ScoringService.cs   # Classic Tetris scoring implementation
│           └── IMainThreadDispatcher.cs  # Main thread dispatch abstraction
├── Tetris4Dummies/                 # .NET MAUI app (net9.0-android)
│   ├── Presentation/
│   │   ├── Views/
│   │   │   ├── MainPage.xaml      # Main game UI
│   │   │   └── MainPage.xaml.cs   # Code-behind with DI constructor
│   │   ├── ViewModels/
│   │   │   └── GameViewModel.cs   # MVVM ViewModel + RelayCommand
│   │   ├── Graphics/
│   │   │   ├── GameDrawable.cs    # Main game grid renderer
│   │   │   ├── NextPieceDrawable.cs  # Next piece preview renderer
│   │   │   └── TetrisColorPalette.cs # Shared color definitions
│   │   ├── Services/
│   │   │   └── MainThreadDispatcher.cs # MAUI MainThread implementation
│   │   └── Styles/
│   ├── Platforms/                  # Platform-specific code
│   ├── Resources/                  # Assets and resources
│   ├── MauiProgram.cs             # DI container registration
│   ├── App.xaml[.cs]              # Application lifecycle
│   └── AppShell.xaml[.cs]         # App navigation shell
tests/
└── Tetris4Dummies.Tests/          # Unit test project (net9.0)
    ├── Core/
    │   ├── Models/
    │   │   ├── GameGridTests.cs
    │   │   ├── GamePieceTests.cs
    │   │   └── GameStateTests.cs
    │   └── Helpers/
    │       ├── ScoringServiceTests.cs
    │       └── RandomProviderTests.cs
    └── Presentation/
        ├── ViewModels/
        │   └── GameViewModelTests.cs
        └── Graphics/
            ├── GameDrawableTests.cs
            └── NextPieceDrawableTests.cs
```

### Core Library

`Tetris4Dummies.Core` is a shared .NET 9.0 class library with no MAUI dependency. It contains all game logic (models) and service interfaces/implementations (helpers), allowing it to be referenced by both the MAUI app and the unit test project without requiring MAUI workloads.

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

### Helpers Layer

#### IRandomProvider / RandomProvider

Abstraction over `System.Random` enabling deterministic testing. `RandomProvider` is the default implementation using `System.Random`.

#### IScoringService / ScoringService

Defines scoring calculation logic. `ScoringService` implements classic Tetris scoring with `CalculateScore(linesCleared, level)`, `CalculateLevel(totalLinesCleared)`, and `LinesPerLevel` configuration.

#### IMainThreadDispatcher

Abstraction for dispatching work to the UI main thread, decoupling game logic from MAUI's `MainThread.BeginInvokeOnMainThread()`.

### Graphics Layer

#### GameDrawable

Implements `IDrawable` interface for rendering the main game grid, placed blocks, and the current falling piece using `Microsoft.Maui.Graphics` APIs.

#### NextPieceDrawable

Implements `IDrawable` to render a preview of the next piece in a smaller panel, giving the player look-ahead information.

#### TetrisColorPalette

Defines shared color constants used across all rendering components for consistent block and grid styling.

### ViewModel Layer

#### GameViewModel

Implements the MVVM pattern using `INotifyPropertyChanged` and `RelayCommand`.

**Responsibilities**:

- Exposes bindable properties (Score, Level, IsGameOver, etc.) to the View
- Provides commands (NewGameCommand, MoveLeftCommand, MoveRightCommand, DropCommand)
- Manages the game loop timer and tick logic
- Uses `IMainThreadDispatcher` to marshal UI updates to the main thread
- Implements `IDisposable` for proper timer cleanup

### UI Layer

#### MainPage

Main game interface using constructor injection to receive `GameViewModel`.

- Binds to `GameViewModel` for all game state and commands
- Contains `GraphicsView` components for game grid and next piece preview
- Control buttons bound to ViewModel commands
- Lifecycle management (OnAppearing/OnDisappearing) delegates to ViewModel

## Design Patterns

### Separation of Concerns

- **Models**: Pure game logic, no UI dependencies
- **Graphics**: Rendering logic using platform-agnostic APIs
- **UI**: Event handling and user interaction

### MVVM Pattern

Uses XAML for UI layout with `GameViewModel` as the binding context. The ViewModel exposes commands and observable properties, keeping the View (MainPage) free of game logic.

### IDrawable Pattern

Leverages MAUI's `IDrawable` interface for custom graphics, ensuring cross-platform compatibility without platform-specific rendering code.

### Dependency Injection

Services are registered in `MauiProgram.cs`:

- `IRandomProvider` → `RandomProvider`
- `IScoringService` → `ScoringService`
- `IMainThreadDispatcher` → `MainThreadDispatcher`
- `GameState` (transient)
- `GameViewModel` (transient)
- `MainPage` (transient, receives `GameViewModel` via constructor injection)

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
