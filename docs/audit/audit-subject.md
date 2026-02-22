# Tetris4Dummies — Repository Health Audit

**Date:** 2026-02-21  
**Scope:** Full codebase analysis — architecture, test coverage, CI health, code quality, documentation, and contributor readiness  
**Commit:** `b007ca55` (HEAD of `main`)

---

## Health Summary

Tetris4Dummies is a well-structured .NET MAUI Tetris clone with clean separation of concerns across Core (models/helpers) and Presentation (views/viewmodels/graphics) layers. The codebase demonstrates good engineering practices including dependency injection for testability, interface-driven design, and a solid 165-test suite at ~89% line coverage. The primary health concern is a **broken CI pipeline** due to a coverage threshold mismatch, plus several moderate code quality and documentation issues that should be addressed.

---

## Key Findings

### 1. CI Pipeline Is Broken (Coverage Threshold Failure)

| Field | Content |
|-------|---------|
| **Issue** | CI fails because the 90% line coverage threshold is not met (actual: 88.57%) |
| **Evidence** | `.github/workflows/ci.yml:39` — `/p:Threshold=90` enforced; CI logs show `The total line coverage is below the specified 90`. All 165 tests pass, but coverlet enforces the gate. |
| **Risk Tier** | **High** |
| **Impact** | All pushes and PRs to `main` are red. Developers lose trust in CI signal; merged code has no green gate. The latest push (run #4) has been failing since 2025-12-23. |
| **Direction** | Either raise coverage to ≥90% (approximately 5 more lines need covering — see coverage gaps below) or lower the threshold to 88% until gaps are closed. Fixing this is the single highest-priority item. |

---

### 2. Coverage Gaps in GameViewModel Timer/Lifecycle Code

| Field | Content |
|-------|---------|
| **Issue** | `GameViewModel.OnGameTimerTick` and timer lifecycle methods are untestable without MAUI's `MainThread`, leaving ~11% of lines uncovered |
| **Evidence** | `src/Tetris4Dummies/Presentation/ViewModels/GameViewModel.cs:99-121` — `OnGameTimerTick` calls `MainThread.BeginInvokeOnMainThread()`, which is MAUI-platform-specific and cannot be exercised in a pure unit test. The test project links source files directly (`Tetris4Dummies.Tests.csproj:32-35`) rather than via project reference to avoid Android SDK dependency, making MAUI runtime APIs inaccessible. |
| **Risk Tier** | **Medium** |
| **Impact** | Timer tick logic (game loop, level speed changes, game-over stop) has zero test coverage. Bugs in this path won't be caught until manual testing. This is the primary driver of the CI coverage failure. |
| **Direction** | Extract `MainThread.BeginInvokeOnMainThread` behind an `IMainThreadDispatcher` interface (similar to how `IRandomProvider` was done), allowing the timer callback to be tested with a synchronous mock dispatcher. Alternatively, exclude the ViewModel timer methods from coverage calculations via `[ExcludeFromCodeCoverage]` with a justification comment. |

---

### 3. Test Project References Source Files Directly (Fragile Linkage)

| Field | Content |
|-------|---------|
| **Issue** | The test project uses `<Compile Include="...">` glob patterns to pull in source files rather than a `<ProjectReference>` |
| **Evidence** | `tests/Tetris4Dummies.Tests/Tetris4Dummies.Tests.csproj:32-35` — four `<Compile Include>` directives link `Core/Models/*.cs`, `Core/Helpers/*.cs`, `Presentation/Graphics/*.cs`, and `Presentation/ViewModels/*.cs` from `src/`. A comment at line 31 explains: "Reference source files directly instead of project reference to avoid Android SDK requirement." |
| **Risk Tier** | **Medium** |
| **Impact** | If a new `.cs` file is added to a linked folder, it is silently included in tests and may cause compilation errors or unexpected coverage measurements. If a file is added to a non-linked folder (e.g., `Core/Services/`), it is silently excluded from tests. The test assembly also compiles source with potentially different preprocessor definitions than the main project. |
| **Direction** | Consider creating a shared `Tetris4Dummies.Core` class library project targeting `net9.0` (no MAUI dependency) that both the main project and test project reference. This would give proper dependency tracking and eliminate glob-based source linking. |

---

### 4. Stale Documentation

| Field | Content |
|-------|---------|
| **Issue** | Several documentation files are outdated and don't reflect current codebase structure |
| **Evidence** | `docs/architecture.md:16-37` — shows a flat project structure (`Models/`, `Graphics/`, `MainPage.xaml`) that doesn't match the actual nested structure (`Core/Models/`, `Core/Helpers/`, `Presentation/Views/`, `Presentation/ViewModels/`, `Presentation/Graphics/`). Key classes like `GameViewModel`, `ScoringService`, `IScoringService`, `NextPieceDrawable`, and `TetrisColorPalette` are not documented. `tests/Tetris4Dummies.Tests/README.md:7-11` shows an outdated test structure (missing `Core/Helpers/` and `Presentation/` test directories) and claims "89 Total Tests" when there are now 165. Lines 50-51 claim "5 tests currently fail" — this is no longer true; all 165 pass. |
| **Risk Tier** | **Low** |
| **Impact** | New contributors will be confused by discrepancies between docs and reality. The architecture doc omits the MVVM refactoring (ViewModel, ScoringService extraction). |
| **Direction** | Update `docs/architecture.md` to reflect the `Core/Helpers/`, `Presentation/ViewModels/`, and `Presentation/Graphics/` structure. Update the test README to reflect 165 tests, all passing, and the correct directory layout. |

---

### 5. Missing License File

| Field | Content |
|-------|---------|
| **Issue** | The README states "Open source - feel free to learn from and modify this code" but no `LICENSE` file exists |
| **Evidence** | `README.md:45` — "Open source - feel free to learn from and modify this code." No `LICENSE`, `LICENSE.md`, or `LICENSE.txt` file in the repository root. |
| **Risk Tier** | **Low** |
| **Impact** | Without a formal license file, the project has no legally enforceable open-source license. Contributors may be hesitant to contribute, and downstream users have no clear rights. "Open source" stated in a README is not a license grant. |
| **Direction** | Add a `LICENSE` file (e.g., MIT, Apache 2.0) to the repository root. |

---

### 6. DI Container Not Used for ViewModel Wiring

| Field | Content |
|-------|---------|
| **Issue** | `MauiProgram.cs` registers no services; `MainPage` manually instantiates `GameViewModel` |
| **Evidence** | `src/Tetris4Dummies/MauiProgram.cs:7-24` — only font registration, no `builder.Services.AddSingleton/Transient` calls. `src/Tetris4Dummies/Presentation/Views/MainPage.xaml.cs:13` — `_viewModel = new GameViewModel()` is a direct instantiation. The `.clinerules` file (line 68) recommends "Leverage dependency injection through `MauiProgram.cs` for services." |
| **Risk Tier** | **Low** |
| **Impact** | The MVVM pattern is partially applied — commands and property binding exist, but the ViewModel is tightly coupled to the View via `new`. This makes the View harder to unit test and prevents swapping ViewModel implementations. Not critical for a small game, but inconsistent with the project's stated conventions. |
| **Direction** | Register `GameViewModel` (and potentially `GameState`, `IScoringService`) in `MauiProgram.cs` and inject into `MainPage` via constructor injection. |

---

### 7. No Input Validation on GameGrid Indexer

| Field | Content |
|-------|---------|
| **Issue** | The `GameGrid` indexer allows out-of-bounds access, which will throw `IndexOutOfRangeException` |
| **Evidence** | `src/Tetris4Dummies/Core/Models/GameGrid.cs:22-26` — the indexer directly accesses `_grid[row, col]` with no bounds checking. The `IsInBounds` and `IsEmpty` methods exist but are separate from the indexer. A caller doing `grid[-1, 0] = 1` gets a raw runtime exception. |
| **Risk Tier** | **Low** |
| **Impact** | Currently mitigated by `GameState` checking bounds before access. But external callers (including tests: `GameGridTests.cs:43` uses `grid[5, 3] = 1` directly) have no guard. A misuse would produce an unhelpful `IndexOutOfRangeException` rather than a domain-specific error. |
| **Direction** | This is acceptable for an internal data structure where callers are trusted. If the grid is ever exposed as a public API, consider adding bounds-checking to the setter or returning a `bool` success result. |

---

### 8. MAUI Package Version Mismatch Between Main and Test Projects

| Field | Content |
|-------|---------|
| **Issue** | The main project uses `$(MauiVersion)` for MAUI Controls, but the test project pins `9.0.111` |
| **Evidence** | `src/Tetris4Dummies/Tetris4Dummies.csproj:54` — `<PackageReference Include="Microsoft.Maui.Controls" Version="$(MauiVersion)" />`. `tests/Tetris4Dummies.Tests/Tetris4Dummies.Tests.csproj:22-23` — `Version="9.0.111"` hardcoded for both `Microsoft.Maui.Controls` and `Microsoft.Maui.Graphics`. |
| **Risk Tier** | **Low** |
| **Impact** | When the MAUI workload updates, the main project gets the new version automatically while tests stay on 9.0.111. This can cause subtle API mismatches or missed regressions. |
| **Direction** | Align versions using a shared `Directory.Build.props` or `Directory.Packages.props` file with centralized package versioning. |

---

## Strengths

These patterns are well-designed and should be preserved:

1. **Clean Separation of Concerns** — Core models (`GameGrid`, `GamePiece`, `GameState`) have zero UI dependencies. The `Core/` namespace is fully testable without MAUI infrastructure. This is the project's strongest architectural decision.

2. **Interface-Driven Testability** — `IRandomProvider` and `IScoringService` abstractions enable deterministic testing via mocks. The `GameState` constructor chain (`default → IRandomProvider → IRandomProvider + IScoringService`) provides progressive DI levels. This is a textbook approach.

3. **Comprehensive Test Suite** — 165 tests covering models, helpers, viewmodel, and graphics rendering with proper use of xUnit `[Theory]`/`[InlineData]`, FluentAssertions, and Moq. Tests follow AAA pattern consistently with descriptive names (`MethodName_Scenario_ExpectedResult`).

4. **Extracted `ScoringService`** — Scoring logic is cleanly separated from `GameState` into its own service with an interface, following Single Responsibility Principle. The comment at `ScoringService.cs:6` ("Extracts scoring responsibility from GameState (Anti-Pattern #3)") shows deliberate refactoring.

5. **Shared Rendering Utilities** — `TetrisColorPalette` centralizes color definitions and beveled block rendering, eliminating duplication between `GameDrawable` and `NextPieceDrawable`.

6. **CI with Coverage Reporting** — The CI pipeline (`.github/workflows/ci.yml`) includes coverage collection, artifact upload, summary generation, and PR comments. This is a complete coverage feedback loop — it just needs the threshold fixed.

7. **Proper Timer Lifecycle Management** — `GameViewModel.StopTimer()` correctly unsubscribes the event handler, stops, and disposes the timer. The `Dispose` pattern with `_isDisposed` guard prevents double-disposal issues.

8. **Thorough `.clinerules` Configuration** — The `.clinerules` file provides comprehensive project conventions, coding standards, file patterns, and common commands. This serves as excellent onboarding documentation for AI-assisted development.

---

## Risk Register

| # | Issue | Likelihood | Impact | Risk Score | Dimension |
|---|-------|-----------|--------|------------|-----------|
| 1 | Broken CI (coverage threshold) | **Certain** | High | 🔴 Critical | CI Health |
| 2 | Untested timer/lifecycle code | High | Medium | 🟠 High | Test Coverage |
| 3 | Fragile source-file linking in tests | Medium | Medium | 🟡 Medium | Architecture |
| 4 | Stale documentation | High | Low | 🟡 Medium | Docs |
| 5 | Missing LICENSE file | Certain | Low | 🟡 Medium | Contributor Alignment |
| 6 | No DI container usage | Low | Low | 🟢 Low | Architecture |
| 7 | No indexer bounds checking | Low | Low | 🟢 Low | Code Quality |
| 8 | MAUI version mismatch | Medium | Low | 🟢 Low | Dependencies |

---

## CI Status Summary

| Run # | Status | Trigger | Title |
|-------|--------|---------|-------|
| 4 | ❌ Failure | push | Add code coverage thresholds and more tests |
| 3 | ✅ Success | push | Fix a bunch of anti-patterns |
| 2 | ✅ Success | push | Fix a few anti-patterns |
| 1 | ✅ Success | push | Improve the UX, add a workflow |

**Failure cause:** All 165 tests pass. Line coverage is 88.57% (310/350 lines), below the 90% threshold configured in `ci.yml:39`. Branch coverage is 81.88% (104/127).

---

## Recommendations (Priority Order)

1. **Fix CI immediately** — Lower threshold to 88% or add coverage for the ~5 missing lines to reach 90%
2. **Abstract `MainThread` dispatch** — Create `IMainThreadDispatcher` to make timer callback testable
3. **Update architecture docs** — Reflect MVVM refactoring, helpers, and current test counts
4. **Add LICENSE file** — MIT is common for educational/game projects
5. **Consider shared Core library** — Replace source-file linking with a proper `net9.0` class library
6. **Centralize package versions** — Add `Directory.Packages.props` for consistent MAUI versions
