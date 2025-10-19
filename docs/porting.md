# Platform Porting Guide

This guide explains how to add support for additional platforms beyond Android.

## Overview

Tetris4Dummies is built with .NET MAUI, which supports multiple platforms from a single codebase. The project is currently configured for Android, but can be easily extended to support iOS, Windows, macOS, and Tizen.

## Supported Platforms

- **Android**: API 21+ (Android 5.0+)
- **iOS**: iOS 11+
- **Windows**: Windows 10.0.19041.0+
- **macOS**: macOS 10.15+
- **Tizen**: Tizen 4.0+

## Adding Platform Support

### Step 1: Update Target Frameworks

Edit `Tetris4Dummies.csproj` and modify the `<TargetFrameworks>` element:

**Current (Android only)**:

```xml
<TargetFrameworks>net9.0-android</TargetFrameworks>
```

**Multiple Platforms**:

```xml
<!-- Android + iOS -->
<TargetFrameworks>net9.0-android;net9.0-ios</TargetFrameworks>

<!-- Android + iOS + Windows -->
<TargetFrameworks>net9.0-android;net9.0-ios;net9.0-windows10.0.19041.0</TargetFrameworks>

<!-- All platforms -->
<TargetFrameworks>
  net9.0-android;
  net9.0-ios;
  net9.0-maccatalyst;
  net9.0-windows10.0.19041.0;
  net9.0-tizen
</TargetFrameworks>
```

### Step 2: Install Platform Workloads

Install the required workload for each target platform:

```bash
# Android
dotnet workload install maui-android

# iOS (macOS only)
dotnet workload install maui-ios

# Windows
dotnet workload install maui-windows

# macOS
dotnet workload install maui-maccatalyst

# Tizen
dotnet workload install maui-tizen

# Or install all at once
dotnet workload install maui
```

### Step 3: Platform-Specific Configuration

Each platform has its own configuration files in the `Platforms/` directory:

#### Android

- `Platforms/Android/AndroidManifest.xml` - App permissions and configuration
- `Platforms/Android/MainActivity.cs` - Main activity entry point

#### iOS

- `Platforms/iOS/Info.plist` - App configuration and permissions
- `Platforms/iOS/Entitlements.plist` - App capabilities
- `Platforms/iOS/AppDelegate.cs` - App lifecycle

#### Windows

- `Platforms/Windows/Package.appxmanifest` - App package configuration
- `Platforms/Windows/app.manifest` - Windows-specific settings

#### macOS (Mac Catalyst)

- `Platforms/MacCatalyst/Info.plist` - App configuration
- `Platforms/MacCatalyst/Entitlements.plist` - App capabilities

#### Tizen

- `Platforms/Tizen/tizen-manifest.xml` - App manifest

## Platform-Specific Code

### Using Conditional Compilation

When you need platform-specific code, use conditional compilation:

```csharp
#if ANDROID
    // Android-specific code
#elif IOS
    // iOS-specific code
#elif WINDOWS
    // Windows-specific code
#elif MACCATALYST
    // macOS-specific code
#elif TIZEN
    // Tizen-specific code
#endif
```

### Partial Classes

Create platform-specific implementations using partial classes:

**Shared code** (`MyService.cs`):

```csharp
public partial class MyService
{
    public void DoSomething()
    {
        DoSomethingPlatformSpecific();
    }
    
    partial void DoSomethingPlatformSpecific();
}
```

**Platform code** (`Platforms/Android/MyService.cs`):

```csharp
public partial class MyService
{
    partial void DoSomethingPlatformSpecific()
    {
        // Android implementation
    }
}
```

## Platform-Specific Considerations

### iOS

**Requirements**:

- macOS with Xcode installed
- Apple Developer account (for device deployment)
- Valid provisioning profile and certificate

**Additional Setup**:

1. Open Xcode and ensure command line tools are installed
2. Accept Xcode license: `sudo xcodebuild -license`
3. For device deployment, configure signing in `Info.plist`

**Running**:

```bash
# Simulator
dotnet build -f net9.0-ios -t:Run

# Specific simulator
dotnet build -f net9.0-ios -t:Run /p:_DeviceName="iPhone 15"
```

### Windows

**Requirements**:

- Windows 10.0.19041.0 or later
- Windows App SDK
- Visual Studio 2022 recommended for full Windows development experience

**Running**:

```bash
dotnet build -f net9.0-windows10.0.19041.0 -t:Run
```

**Packaging**:
For Microsoft Store deployment, use Visual Studio's packaging wizard or `dotnet publish`.

### macOS (Mac Catalyst)

**Requirements**:

- macOS 10.15+
- Xcode 13+
- Apple Developer account (for distribution)

**Running**:

```bash
dotnet build -f net9.0-maccatalyst -t:Run
```

### Tizen

**Requirements**:

- Tizen SDK
- Visual Studio with Tizen extension
- Tizen emulator or device

**Setup**:

1. Install Tizen SDK
2. Configure Tizen certificate
3. Set up emulator or connect device

## Testing on Multiple Platforms

### Build All Platforms

```bash
# Restore for all platforms
dotnet restore

# Build all platforms
dotnet build
```

### Selective Building

```bash
# Build only Android
dotnet build -f net9.0-android

# Build Android and iOS
dotnet build -f net9.0-android
dotnet build -f net9.0-ios
```

## Common Issues and Solutions

### Issue: Platform workload not found

**Solution**: Install the specific workload using `dotnet workload install`

### Issue: iOS build fails on Windows

**Solution**: iOS development requires macOS. Use a Mac or Mac cloud service.

### Issue: Windows build fails on macOS/Linux

**Solution**: Windows target framework only builds on Windows. Use Windows VM or dual boot.

### Issue: Code signing errors (iOS/macOS)

**Solution**: Configure proper provisioning profiles and certificates in Xcode.

### Issue: Missing platform-specific APIs

**Solution**: Use conditional compilation or abstract platform-specific code into separate files.

## Platform-Specific Features

### Tetris4Dummies Compatibility

The current game implementation uses cross-platform APIs only:

- ✅ **Graphics**: `Microsoft.Maui.Graphics` (works on all platforms)
- ✅ **UI**: XAML controls (cross-platform)
- ✅ **Input**: Button controls (cross-platform)
- ✅ **Timers**: `System.Timers.Timer` (cross-platform)

No platform-specific code modifications are needed to run on different platforms.

## Deployment

### Android

- Build APK or AAB for Google Play Store
- Requires signing key

### iOS

- Build IPA for App Store or TestFlight
- Requires Apple Developer account and signing

### Windows

- Build MSIX package for Microsoft Store
- Can also distribute as standalone installer

### macOS

- Build APP bundle for Mac App Store
- Can also distribute as DMG

## Performance Considerations

### Platform Differences

- **Mobile (Android/iOS)**: Optimize for battery, touch input
- **Desktop (Windows/macOS)**: Can use more resources, keyboard input
- **Tizen**: Varies by device type (TV, watch, phone)

### Graphics Performance

The game uses `IDrawable` which is hardware-accelerated on all platforms. No special optimization needed for basic gameplay.

## Resources

- [.NET MAUI Platform-Specific Features](https://learn.microsoft.com/dotnet/maui/platform-integration/)
- [iOS Deployment Guide](https://learn.microsoft.com/dotnet/maui/ios/deployment/)
- [Android Deployment Guide](https://learn.microsoft.com/dotnet/maui/android/deployment/)
- [Windows Deployment Guide](https://learn.microsoft.com/dotnet/maui/windows/deployment/)
