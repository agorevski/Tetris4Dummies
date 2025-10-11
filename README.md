# Tetris4Dummies
A Tetris clone for Android except that all of the piece types are 1 single square block

## Test Framework

This project includes a comprehensive test framework for Android development:

- **Unit Tests**: Using JUnit 4, Mockito, and Google Truth for local testing
- **Instrumented Tests**: Using AndroidX Test and Espresso for UI testing

See [TESTING.md](TESTING.md) for detailed information about the test framework and how to run tests.

## Project Structure

```
app/
├── src/
│   ├── main/           # Main application code
│   ├── test/           # Unit tests
│   └── androidTest/    # Instrumented tests
```

## Running Tests

### Unit Tests
```bash
./gradlew test
```

### Instrumented Tests (requires device/emulator)
```bash
./gradlew connectedAndroidTest
```

## Dependencies

Key testing dependencies include:
- JUnit 4.13.2
- Mockito 5.3.1
- AndroidX Test Extensions
- Espresso 3.5.1
