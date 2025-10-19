# Getting Started with Tetris4Dummies

This guide will help you set up the development environment and get the game running.

## Prerequisites

- .NET 9.0 SDK or later
- .NET MAUI workload for your target platform

## Installation

### 1. Install .NET MAUI Workload

For Android development:

```bash
dotnet workload install maui-android
```

For iOS development (macOS only):

```bash
dotnet workload install maui-ios
```

For all platforms:

```bash
dotnet workload install maui
```

### 2. Clone and Restore Dependencies

```bash
git clone https://github.com/agorevski/Tetris4Dummies
cd Tetris4Dummies
dotnet restore
```

## Building the Project

### Android

```bash
dotnet build -f net9.0-android
```

### iOS (macOS only)

```bash
dotnet build -f net9.0-ios
```

### Windows

```bash
dotnet build -f net9.0-windows10.0.19041.0
```

## Running the Application

### On Android Device/Emulator

```bash
dotnet build -f net9.0-android -t:Run
```

### On iOS Simulator (macOS only)

```bash
dotnet build -f net9.0-ios -t:Run
```

### On Windows

```bash
dotnet build -f net9.0-windows10.0.19041.0 -t:Run
```

## Troubleshooting

### Common Issues

**Workload not found**: Ensure you've installed the correct workload for your target platform.

**Build errors**: Try cleaning the project:

```bash
dotnet clean
dotnet restore
dotnet build
```

**Android emulator issues**: Ensure you have an Android emulator configured in Android Studio or Visual Studio.

**iOS build fails**: Verify you're on macOS and have Xcode installed with command line tools.

## Development Environment

### Recommended IDEs

- Visual Studio 2022 (Windows/macOS)
- Visual Studio Code with C# Dev Kit extension
- JetBrains Rider

### Debugging

The project includes launch configurations for debugging. Use F5 to start debugging in your IDE.

## Next Steps

- Review the [Architecture documentation](architecture.md) to understand the codebase
- Check [Porting guide](porting.md) if you want to add support for additional platforms
