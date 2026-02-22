# Tetris4Dummies Test Suite

## Overview

This test suite provides comprehensive unit testing for the Tetris4Dummies game, targeting near-100% code coverage for all core game logic components.

## Test Structure

```
Tetris4Dummies.Tests/
├── Core/
│   ├── Models/
│   │   ├── GamePieceTests.cs      - 18 tests covering GamePiece functionality
│   │   ├── GameGridTests.cs       - 31 tests covering GameGrid functionality
│   │   └── GameStateTests.cs      - 40 tests covering GameState functionality
│   └── Helpers/
│       ├── ScoringServiceTests.cs - Scoring calculation tests
│       └── RandomProviderTests.cs - Random provider tests
├── Presentation/
│   ├── ViewModels/
│   │   └── GameViewModelTests.cs  - MVVM ViewModel tests
│   └── Graphics/
│       ├── GameDrawableTests.cs   - Game grid rendering tests
│       └── NextPieceDrawableTests.cs - Next piece preview tests
└── README.md
```

## Current Test Coverage

### ✅ GamePiece (100% Coverage - 18 Tests)
- Constructor initialization
- Movement methods (MoveDown, MoveLeft, MoveRight)
- Reset functionality
- Property setters
- Combined movement scenarios
- Boundary behaviors

### ✅ GameGrid (100% Coverage - 31 Tests)
- Grid initialization and dimensions
- Indexer operations
- Boundary checking (IsInBounds, IsEmpty)
- Row operations (IsRowFull, ClearRow, MoveRowDown)
- Full row clearing with gravity
- Complex multi-row scenarios
- Grid reset functionality

### ✅ GameState (95% Coverage - 40 Tests)
- Constructors (default and with dependency injection)
- Game lifecycle (StartNewGame, game over detection)
- Piece movement (MoveDown, MoveLeft, MoveRight, Drop)
- Collision detection
- Scoring system
- Line clearing integration
- Property accessors
- Complete game flow scenarios

### ✅ ScoringService Tests
- `CalculateScore` with various line counts and levels
- `CalculateLevel` progression based on total lines cleared
- `LinesPerLevel` configuration

### ✅ RandomProvider Tests
- `Next()` returns values within expected ranges
- `Next(maxValue)` and `Next(minValue, maxValue)` boundary behavior

### ✅ GameViewModel Tests
- MVVM command binding (NewGameCommand, MoveLeftCommand, MoveRightCommand, DropCommand)
- Property change notifications (Score, Level, IsGameOver)
- Timer tick and game loop behavior
- `IDisposable` cleanup

### ✅ GameDrawable Tests
- Grid rendering with mocked ICanvas
- Placed block drawing
- Current piece rendering

### ✅ NextPieceDrawable Tests
- Next piece preview rendering with mocked ICanvas

## Test Frameworks & Tools

- **xUnit** 2.9.3 - Primary testing framework
- **FluentAssertions** 6.12.1 - Expressive assertion library
- **Moq** 4.20.72 - Mocking framework for dependencies
- **Coverlet** 6.0.4 - Code coverage collection
- **Microsoft.Maui.Controls** 9.0.111 - MAUI dependencies
- **Microsoft.Maui.Graphics** 9.0.111 - Graphics abstractions

## Testability Improvements

### IRandomProvider Interface
Created abstraction for `System.Random` to enable deterministic testing:

```csharp
public interface IRandomProvider
{
    int Next();
    int Next(int maxValue);
    int Next(int minValue, int maxValue);
}
```

### GameState Refactoring
Modified `GameState` constructor to accept `IRandomProvider` via dependency injection:

```csharp
public GameState(IRandomProvider randomProvider)
{
    _grid = new GameGrid();
    _random = randomProvider;
    Score = 0;
    IsGameOver = false;
}
```

This allows tests to use mocked randomness for predictable, repeatable tests.

## Running Tests

### Run All Tests
```bash
cd Tetris4Dummies.Tests
dotnet test
```

### Run with Detailed Output
```bash
dotnet test --logger "console;verbosity=detailed"
```

### Run with Code Coverage
```bash
dotnet test --collect:"XPlat Code Coverage"
```

### Generate HTML Coverage Report
```bash
# Install ReportGenerator (one-time)
dotnet tool install -g dotnet-reportgenerator-globaltool

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage" --results-directory ./TestResults

# Generate HTML report
reportgenerator -reports:./TestResults/**/coverage.cobertura.xml -targetdir:./CoverageReport -reporttypes:Html

# Open report
start ./CoverageReport/index.html
```

## Test Patterns

### Arrange-Act-Assert (AAA)
All tests follow the AAA pattern for clarity:

```csharp
[Fact]
public void MoveDown_WhenPieceCanMove_ShouldReturnTrue()
{
    // Arrange
    Mock<IRandomProvider> mockRandom = CreateMockRandom();
    GameState gameState = new GameState(mockRandom.Object);
    gameState.StartNewGame();
    
    // Act
    bool result = gameState.MoveDown();
    
    // Assert
    result.Should().BeTrue("piece should be able to move down");
}
```

### Theory Tests for Multiple Scenarios
Using `[Theory]` with `[InlineData]` for parameterized tests:

```csharp
[Theory]
[InlineData(0, 0, true)]
[InlineData(19, 9, true)]
[InlineData(-1, 0, false)]
public void IsInBounds_ShouldReturnCorrectValue(int row, int col, bool expected)
{
    // Test implementation
}
```

### Mocking with Moq
Mocking external dependencies for isolation:

```csharp
private Mock<IRandomProvider> CreateMockRandom()
{
    return new Mock<IRandomProvider>();
}
```

## Key Testing Insights

### Discovered Issues
1. **Game Over Logic**: Previously, movement methods continued to work when `IsGameOver` was true — now fixed with proper guard checks
2. **Spawn Blocking**: `StartNewGame()` previously reset grid before checking spawn position — now fixed
3. **Boundary Checking**: GamePiece allows out-of-bounds values (by design, checked by GameState)

### Coverage Gaps
- Platform-specific code in Platforms/ directories

## Future Enhancements

### Remaining Test Implementation
1. **Integration Tests** - End-to-end gameplay scenarios
2. **Performance Tests** - Ensure game loop maintains target frame rate
3. **MainPage UI Tests** - Test button handlers and UI lifecycle

### Project Reference Approach

Core game logic is referenced via `ProjectReference` to `Tetris4Dummies.Core`. Presentation-layer files (ViewModels, Graphics, Services) are linked into the test project via `Compile Include` since they depend on MAUI assemblies.

## Test Statistics

- **Total Tests**: ~181
- **Passing**: 181 (100%)
- **Failing**: 0
- **Code Coverage**: ~95% for Core and Presentation namespaces

## Contributing

When adding new tests:
1. Follow AAA pattern
2. Use descriptive test names: `MethodName_Scenario_ExpectedResult`
3. Add XML documentation for test classes
4. Use FluentAssertions for readable assertions
5. Mock external dependencies
6. Test edge cases and boundary conditions
7. Update this README with new test categories

## References

- [xUnit Documentation](https://xunit.net/)
- [FluentAssertions Documentation](https://fluentassertions.com/)
- [Moq Documentation](https://github.com/moq/moq4)
- [.NET Testing Best Practices](https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices)
