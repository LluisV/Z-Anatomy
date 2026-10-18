# =============================================================================
#  install_to_device.ps1
#  Builds Z-Anatomy APK and automatically installs it to connected Android phone
# =============================================================================

$ErrorActionPreference = "Stop"

$binDir = "$env:USERPROFILE\bin"
$env:JAVA_HOME = "C:\Android\OpenJDK"
$env:ANDROID_HOME = "C:\Android\SDK"
$env:ANDROID_SDK_ROOT = "C:\Android\SDK"
$env:ANDROID_NDK_ROOT = "C:\Android\NDK"
$env:ANDROID_NDK_HOME = "C:\Android\NDK"
$env:PATH = "C:\Android\SDK\platform-tools;$binDir;$env:PATH"

Write-Host "================================================" -ForegroundColor Cyan
Write-Host "  Z-Anatomy Android — Build & Install to Device" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan

# 1. Check connected device
Write-Host "`n[1/3] Checking connected Android device via ADB..." -ForegroundColor Yellow
$deviceList = (adb devices) | Where-Object { $_ -match "\bdevice\b" -and $_ -notmatch "List of" }
if (-not $deviceList) {
    Write-Host "No authorized Android device found. Checking status:" -ForegroundColor Red
    adb devices -l
    Write-Host "`nPlease connect your phone via USB and tap 'Allow USB debugging' on your phone screen." -ForegroundColor Yellow
    exit 1
}

$deviceId = ($deviceList -split "\s+")[0]
Write-Host "  Connected device: $deviceId" -ForegroundColor Green

# 2. Build APK
Write-Host "`n[2/3] Building Z-Anatomy APK..." -ForegroundColor Yellow
$scriptDir = $PSScriptRoot
& (Join-Path $scriptDir "build_android.ps1")
if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed with exit code $LASTEXITCODE" -ForegroundColor Red
    exit $LASTEXITCODE
}

$candidates = @(
    (Join-Path $scriptDir "Build\Android\ZAnatomy.apk"),
    (Join-Path $scriptDir "Z-Anatomy\Build\Android\ZAnatomy.apk")
)
$apkPath = $candidates | Where-Object { Test-Path $_ } | Select-Object -First 1
if (-not $apkPath) {
    Write-Host "APK not found at Build\Android\ZAnatomy.apk" -ForegroundColor Red
    exit 1
}

# 3. Install to device
Write-Host "`n[3/3] Installing APK onto your phone ($deviceId)..." -ForegroundColor Yellow
adb -s $deviceId install -r $apkPath
if ($LASTEXITCODE -eq 0) {
    Write-Host "`n================================================" -ForegroundColor Green
    Write-Host "  SUCCESS! Z-Anatomy is installed on your phone!" -ForegroundColor Green
    Write-Host "================================================" -ForegroundColor Green
    Write-Host "Look for 'Z-Anatomy' in your app drawer on your phone." -ForegroundColor White
} else {
    Write-Host "ADB install failed. See error above." -ForegroundColor Red
}
