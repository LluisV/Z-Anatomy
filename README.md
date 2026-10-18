# Z-Anatomy (Android Edition)

Open-source 3D human anatomy atlas ported and optimized for Android.

> **📱 Android Version Development**:
> While the original Z-Anatomy project was developed exclusively for desktop systems (PC/Mac/Linux), this repository has been adapted, ported, and developed for **Android** by **[@MAROON7doc](https://github.com/MAROON7doc)**. It introduces mobile touch interaction, mobile-compatible shader pipelines, runtime UI adaptations, and an automated build toolchain for Android devices.

---

## 📱 Download Android APK

You can directly download and install the latest compiled APK on your Android device:

👉 **[Download ZAnatomy.apk (v1.0.0)](https://github.com/MAROON7doc/Z-Anatomy-BEDA-version/releases/download/v1.0.0-android/ZAnatomy.apk)** (166.5 MB)

Or view the [v1.0.0-android Release Page](https://github.com/MAROON7doc/Z-Anatomy-BEDA-version/releases/tag/v1.0.0-android).

---

## 👨‍💻 Development & Credits

* **Android Port & Mobile Development**: Developed by **[@MAROON7doc](https://github.com/MAROON7doc)**
  * Ported the Unity desktop project to Android (GLES3 / URP).
  * Resolved mobile TextMeshPro shader incompatibilities and dynamic Canvas channel rendering.
  * Implemented mobile touch navigation (1-finger orbit, 2-finger pan, and pinch zoom).
  * Fixed startup camera auto-centering, scaling, and initial model visibility.
  * Created one-click automated build and ADB install toolchains (`build_android.ps1`, `install_to_device.ps1`).
* **Original Z-Anatomy Project**: Created by [Lluis Vinent (LluisV)](https://github.com/LluisV/Z-Anatomy) and [Z-Anatomy](https://github.com/Z-Anatomy) contributors (original desktop version).

---

## 🔗 Upstream Contribution

This work has been submitted as an official contribution back to the upstream project:
* **Pull Request**: [**PR #44: Comprehensive Android Port**](https://github.com/LluisV/Z-Anatomy/pull/44) on [LluisV/Z-Anatomy](https://github.com/LluisV/Z-Anatomy)
* **Target Branch**: `Mobile-Version`

---

## 🚀 One-Click Build & Install

If you are cloning this repository to build from source:

### 1. Build APK
```powershell
.\build_android.ps1
```

### 2. Install to Connected Android Device
```powershell
.\install_to_device.ps1
```

---

## ✨ Features in this Release
- **Full 3D Anatomy Models**: High-resolution anatomical structures (~120 MB of FBX data) across skeleton, muscles, nervous, cardiovascular, joints, and organs.
- **TextMeshPro Mobile Optimization**: Clean, crisp typography on mobile without triangle artifacts (`▲▲▲▲`) or text warbling.
- **Automatic Camera Centering**: Fits and centers the anatomy model on launch.
- **Instant Model Visibility**: 3D anatomy renders immediately on startup.
- **Smooth Mobile UI**: Responsive menus, sub-menus, and hierarchy inspection.


