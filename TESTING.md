# Testing Guide for Tetris4Dummies

This document describes the test framework setup for the Tetris4Dummies Android application.

## Test Framework Overview

The project uses a comprehensive testing approach with two types of tests:

### 1. Unit Tests (Local Tests)
Located in `app/src/test/java/`

**Framework:** JUnit 4
**Dependencies:**
- JUnit 4.13.2
- Mockito 5.3.1 (for mocking)
- AndroidX Core Testing (for Architecture Components)
- Kotlinx Coroutines Test (for testing coroutines)
- Google Truth (for assertions)

**Purpose:** Test individual components in isolation without requiring the Android framework.

**Example Tests:**
- `BlockTest.kt` - Tests for the Block class movement and positioning
- `GameManagerTest.kt` - Tests for game state management and scoring

### 2. Instrumented Tests (Instrumentation Tests)
Located in `app/src/androidTest/java/`

**Framework:** AndroidX Test + Espresso
**Dependencies:**
- AndroidX Test JUnit Extension
- Espresso Core 3.5.1
- Espresso Contrib (for RecyclerView testing)
- Espresso Intents (for intent verification)

**Purpose:** Test UI interactions and components that require the Android framework.

**Example Tests:**
- `MainActivityTest.kt` - Tests for the main activity UI components

## Running Tests

### Run Unit Tests
```bash
./gradlew test
```

Or for a specific module:
```bash
./gradlew app:test
```

### Run Instrumented Tests
```bash
./gradlew connectedAndroidTest
```

Note: Instrumented tests require either:
- A physical Android device connected via USB with USB debugging enabled
- An Android emulator running

### Run All Tests
```bash
./gradlew test connectedAndroidTest
```

## Test Coverage

To generate test coverage reports:
```bash
./gradlew testDebugUnitTest --info
```

## Writing New Tests

### Unit Test Example
```kotlin
class MyClassTest {
    @Before
    fun setUp() {
        // Setup code
    }
    
    @Test
    fun testSomething() {
        // Arrange
        val myClass = MyClass()
        
        // Act
        val result = myClass.doSomething()
        
        // Assert
        assertEquals(expectedValue, result)
    }
}
```

### Instrumented Test Example
```kotlin
@RunWith(AndroidJUnit4::class)
class MyActivityTest {
    @get:Rule
    val activityRule = ActivityScenarioRule(MyActivity::class.java)
    
    @Test
    fun testUI() {
        onView(withId(R.id.myView))
            .check(matches(isDisplayed()))
    }
}
```

## Continuous Integration

The test framework is designed to work with CI/CD pipelines. Tests can be run automatically on:
- Pull requests
- Merges to main branch
- Release builds

## Best Practices

1. **Write tests first (TDD)** - Consider writing tests before implementing features
2. **Keep tests independent** - Each test should be able to run independently
3. **Use descriptive names** - Test method names should describe what they test
4. **Follow AAA pattern** - Arrange, Act, Assert
5. **Mock external dependencies** - Use Mockito to mock dependencies in unit tests
6. **Test edge cases** - Don't just test happy paths

## Additional Resources

- [Android Testing Documentation](https://developer.android.com/training/testing)
- [JUnit 4 Documentation](https://junit.org/junit4/)
- [Espresso Documentation](https://developer.android.com/training/testing/espresso)
- [Mockito Documentation](https://site.mockito.org/)
