# Development Anti-Patterns Discovered

This document lists anti-patterns identified in the Tetris4Dummies codebase.

---

## 1. Tight Coupling Between View and Game Logic

**Location:** `MainPage.xaml.cs`

**Issue:** The view directly creates and manages `GameState` and `System.Timers.Timer`. No abstraction layer (ViewModel) exists between UI and game logic.

**Impact:** 
- Difficult to unit test UI interactions
- Harder to maintain separation of concerns
- MAUI MVVM patterns not utilized

**Recommendation:** Introduce a ViewModel layer with commands and observable properties.

---

## 2. Timer Resource Management Pattern

**Location:** `MainPage.xaml.cs` lines 37-51

**Issue:** Timer is stopped, disposed, and recreated on every new game. The `OnDisappearing` also disposes the timer.

**Impact:** Potential for timer leaks if page is navigated away during gameplay without proper disposal.

**Recommendation:** Consider using a single timer instance that's started/stopped, or ensure proper lifecycle management with page events.

---

## 3. Mixed Responsibility in GameState

**Location:** `GameState.cs`

**Issue:** The `GameState` class handles:
- Game state management
- Piece spawning and movement
- Collision detection
- Scoring calculation
- Level progression

**Impact:** Large class with multiple responsibilities, violating Single Responsibility Principle.

**Recommendation:** Consider extracting scoring logic, collision detection, or piece management into separate classes.

---

## 4. Inconsistent Null-Forgiving Operator Usage

**Location:** `GameStateTests.cs`

**Issue:** Heavy use of `!` (null-forgiving operator) on `CurrentPiece!` after `StartNewGame()` calls.

**Impact:** While tests control the flow, this pattern could mask null reference issues if test setup changes.

**Recommendation:** Use `Should().NotBeNull()` followed by conditional access, or restructure tests to avoid repeated null assertions.

---

## Summary

| Priority | Anti-Pattern | Effort |
|----------|-------------|--------|
| Medium | Tight View-Logic Coupling | High |
| Low | Mixed Responsibilities | High |
| Low | Timer Management | Medium |
| Low | Null-Forgiving Operators | Low |
