# .NET MAUI Android Project

This repository contains a .NET MAUI Android project.
Follow this guide to set up your environment and run the project on **Windows**, **Linux**, or **macOS**.

---

## 📦 Prerequisites

### 1. Install .NET SDK

Install the latest [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0).

Check installation:

```bash
dotnet --version
```

### 2. Install Android SDK & Tools

You need the Android SDK, NDK, and related build tools.

* **Windows**: Install [Visual Studio 2022](https://visualstudio.microsoft.com/) with **.NET Multi-platform App UI (MAUI)** and **Mobile development with .NET** workloads.
* **macOS**: Install [Visual Studio 2022 for Mac](https://visualstudio.microsoft.com/vs/mac/).
* **Linux**: Visual Studio is not supported. Install [Android command line tools](https://developer.android.com/studio#command-tools) manually:

  ```bash
  wget https://dl.google.com/android/repository/commandlinetools-linux-11076708_latest.zip
  unzip commandlinetools-linux-11076708_latest.zip -d ~/Android/cmdline-tools
  ```

  Then update and install:

  ```bash
  ~/Android/cmdline-tools/bin/sdkmanager --install "platform-tools" "platforms;android-34" "build-tools;34.0.0" "ndk;25.2.9519653"
  ```

### 3. Environment Variables

Add these to your shell config (`~/.bashrc`, `~/.zshrc`, or PowerShell profile):

```bash
export ANDROID_HOME=$HOME/Android
export ANDROID_SDK_ROOT=$HOME/Android
export PATH=$ANDROID_HOME/platform-tools:$PATH
```

Reload shell:

```bash
source ~/.bashrc
```

### 4. Check MAUI Workload

Install/update MAUI workload:

```bash
dotnet workload install maui
dotnet workload install android
dotnet workload update
```

---

## 🚀 Running the Project

### Common .NET Commands

```bash
# Restore dependencies
dotnet restore

# Build in Debug mode
dotnet build

# Run project on connected Android device/emulator
dotnet build -t:Run -f net8.0-android
```

---

## 📱 Building APK

Build a signed APK for release:

```bash
dotnet publish -f net8.0-android -c Release -o ./publish
```

This generates:

```
./publish/com.companyname.app-Signed.apk
```

---

## 📲 Installing APK on Device

1. Connect device with USB debugging enabled.
2. Verify connection:

   ```bash
   adb devices
   ```
3. Install APK:

   ```bash
   adb install -r ./publish/com.companyname.app-Signed.apk
   ```

---

## 🖥️ Running with VS Code

1. Install extensions:

   * **C# Dev Kit**
   * **.NET MAUI** (optional preview)
   * **Android Debugger**

2. Open the project folder.

3. Launch configuration: select **`.NET MAUI Android`** target.

4. Run/debug the app (`F5`).

---

## ⚠️ Notes

* On **Linux**, only Android targets are supported (no iOS/MacCatalyst).
* Ensure `adb` recognizes your device before running.
* Use an emulator (`avdmanager` / `Android Studio`) if no physical device is connected.

