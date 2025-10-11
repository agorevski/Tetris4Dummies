# Tetris4Dummies

A Tetris clone for Android except that all of the piece types are 1 single square block (hence, the game is for dummies).

This game is built using .NET MAUI (Multi-platform App UI), which is the evolution of Xamarin. While currently configured for Android, it can be easily ported to iOS by modifying the `TargetFrameworks` in the project file.

## Features

- Simple single-block Tetris gameplay
- Score tracking (100 points per line cleared)
- Touch controls for left, right, and drop actions
- Game over detection
- Clean, minimalist UI

## Requirements

- .NET 9.0 SDK or later
- .NET MAUI workload for Android

## Building the Project

1. Install the .NET MAUI Android workload:
   ```bash
   dotnet workload install maui-android
   ```

2. Restore dependencies:
   ```bash
   dotnet restore
   ```

3. Build the project:
   ```bash
   dotnet build -f net9.0-android
   ```

## Running on Android

To run on an Android device or emulator:
```bash
dotnet build -f net9.0-android -t:Run
```

## Porting to iOS

To enable iOS support, update the `TargetFrameworks` in `Tetris4Dummies.csproj`:

```xml
<TargetFrameworks>net9.0-android;net9.0-ios</TargetFrameworks>
```

Then install the iOS workload:
```bash
dotnet workload install maui-ios
```

## Project Structure

- `Models/` - Core game logic (GameGrid, GamePiece, GameState)
- `Graphics/` - Custom graphics rendering for the game canvas
- `MainPage.xaml` - UI layout
- `MainPage.xaml.cs` - UI logic and game controls

## How to Play

1. Tap "New Game" to start
2. Use the arrow buttons to move the falling block:
   - ◄ Move left
   - ► Move right
   - ▼ Drop instantly
3. Fill complete rows to score points and clear them
4. Game ends when blocks reach the top

