# Tetris4Dummies

A simplified Tetris clone where all pieces are single blocks. Built with .NET MAUI for cross-platform deployment.

## Quick Start

```bash
# Install workload
dotnet workload install maui-android

# Run the game
dotnet build -f net9.0-android -t:Run
```

## How to Play

1. Tap **New Game** to start
2. Use controls to move the falling block:
   - **◄** Move left
   - **►** Move right
   - **▼** Drop instantly
3. Fill complete rows to score 100 points per line
4. Game ends when blocks reach the top

## Features

- Single-block gameplay (Tetris made simple!)
- Cross-platform support (Android, iOS, Windows, macOS)
- Clean, minimalist UI
- Touch and button controls

## Documentation

- **[Getting Started](docs/getting-started.md)** - Installation, building, and running
- **[Architecture](docs/architecture.md)** - Project structure and design patterns
- **[Platform Porting](docs/porting.md)** - Adding iOS, Windows, and other platforms

## Requirements

- .NET 9.0 SDK or later
- .NET MAUI workload for your target platform

## License

Open source - feel free to learn from and modify this code.
