# =============================================================================
#  build_android.ps1
#  Z-Anatomy — One-Click Android Build Script
#
#  USAGE:
#    .\build_android.ps1
#    .\build_android.ps1 -UnityPath "C:\Program Files\Unity\Hub\Editor\2022.3.21f1\Editor\Unity.exe"
#    .\build_android.ps1 -BuildAAB   # Build .aab for Google Play instead of .apk
# =============================================================================

param (
    [string]$UnityPath = "",
    [string]$ProjectPath = "",
    [switch]$BuildAAB = $false,
    [switch]$Development = $false
)

$ErrorActionPreference = "Stop"

$env:JAVA_HOME = "C:\Android\OpenJDK"
$env:ANDROID_HOME = "C:\Android\SDK"
$env:ANDROID_SDK_ROOT = "C:\Android\SDK"
$env:ANDROID_NDK_ROOT = "C:\Android\NDK"
$env:ANDROID_NDK_HOME = "C:\Android\NDK"
$env:PATH = "C:\Android\SDK\platform-tools;$env:USERPROFILE\bin;$env:PATH"

Write-Host ""
Write-Host "================================================" -ForegroundColor Cyan
Write-Host "  Z-Anatomy Android Build Script" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan

# ── Auto-detect Unity installation ───────────────────────────────────────────
if ([string]::IsNullOrEmpty($UnityPath))
{
    Write-Host "`n[Auto-detect] Searching for Unity installation..." -ForegroundColor Yellow

    $searchPaths = @(
        "C:\Program Files\Unity\Hub\Editor",
        "C:\Program Files (x86)\Unity\Hub\Editor",
        "$env:LOCALAPPDATA\Programs\Unity\Hub\Editor"
    )

    $foundUnity = $null
    foreach ($basePath in $searchPaths) {
        if (Test-Path $basePath) {
            # Find the newest Unity version installed
            $versions = Get-ChildItem $basePath -Directory | Sort-Object Name -Descending
            foreach ($ver in $versions) {
                $candidate = Join-Path $ver.FullName "Editor\Unity.exe"
                if (Test-Path $candidate) {
                    $foundUnity = $candidate
                    Write-Host "  Found: $candidate" -ForegroundColor Green
                    break
                }
            }
        }
        if ($foundUnity) { break }
    }

    if (-not $foundUnity) {
        Write-Error @"
Unity not found automatically. Please specify the path:
  .\build_android.ps1 -UnityPath "C:\...\Unity.exe"

Download Unity Hub from: https://unity.com/download
Then install Unity 2021.3 LTS or 2022.3 LTS with Android Build Support.
"@
        exit 1
    }

    $UnityPath = $foundUnity
}

# ── Verify Unity exists ───────────────────────────────────────────────────────
if (-not (Test-Path $UnityPath)) {
    Write-Error "Unity.exe not found at: $UnityPath"
    exit 1
}

# ── Auto-detect project path ──────────────────────────────────────────────────
if ([string]::IsNullOrEmpty($ProjectPath)) {
    # Look for a sibling Z-Anatomy directory (created by setup_zanatomy_android.ps1)
    $candidates = @(
        (Join-Path (Get-Location) "Z-Anatomy"),
        (Join-Path $PSScriptRoot "..\Z-Anatomy"),
        (Join-Path $PSScriptRoot "Z-Anatomy")
    )
    foreach ($c in $candidates) {
        if (Test-Path (Join-Path $c "Assets")) {
            $ProjectPath = (Resolve-Path $c).Path
            Write-Host "`n[Auto-detect] Project found at: $ProjectPath" -ForegroundColor Green
            break
        }
    }

    if ([string]::IsNullOrEmpty($ProjectPath)) {
        Write-Error @"
Could not find the Z-Anatomy Unity project folder.
Please run setup_zanatomy_android.ps1 first, or specify:
  .\build_android.ps1 -ProjectPath "C:\path\to\Z-Anatomy"
"@
        exit 1
    }
}

# ── Build output path ─────────────────────────────────────────────────────────
$buildDir  = Join-Path $ProjectPath "Build\Android"
$method    = if ($BuildAAB) { "AndroidBuildScript.BuildAAB" } else { "AndroidBuildScript.BuildAPK" }
$outputExt = if ($BuildAAB) { ".aab" } else { ".apk" }
$outputFile = Join-Path $buildDir "ZAnatomy$outputExt"

New-Item -ItemType Directory -Force -Path $buildDir | Out-Null

# ── Log file ──────────────────────────────────────────────────────────────────
$logFile = Join-Path $buildDir "build_$(Get-Date -Format 'yyyyMMdd_HHmmss').log"

# ── Build flags ───────────────────────────────────────────────────────────────
$extraFlags = ""
if ($Development) { $extraFlags = "-development" }

Write-Host ""
Write-Host "  Unity   : $UnityPath" -ForegroundColor White
Write-Host "  Project : $ProjectPath" -ForegroundColor White
Write-Host "  Output  : $outputFile" -ForegroundColor White
Write-Host "  Log     : $logFile" -ForegroundColor White
Write-Host "  Method  : $method" -ForegroundColor White
Write-Host ""
Write-Host "[Build] Starting... (this can take 5-30 minutes)" -ForegroundColor Yellow

$startTime = Get-Date

# ── Invoke Unity in batch mode ────────────────────────────────────────────────
$args = @(
    "-batchmode",
    "-nographics",
    "-projectPath", "`"$ProjectPath`"",
    "-executeMethod", $method,
    "-logFile", "`"$logFile`"",
    "-quit"
)

if ($Development) { $args += "-development" }

$process = Start-Process -FilePath $UnityPath `
    -ArgumentList ($args -join " ") `
    -Wait `
    -PassThru `
    -NoNewWindow

$elapsed = (Get-Date) - $startTime

Write-Host ""
Write-Host "Build finished in $([math]::Round($elapsed.TotalSeconds))s" -ForegroundColor Cyan

if ($process.ExitCode -ne 0) {
    Write-Host "BUILD FAILED (exit code $($process.ExitCode))" -ForegroundColor Red
    Write-Host "Check log at: $logFile" -ForegroundColor Yellow
    # Show last 40 lines of log
    Write-Host "`n--- Last log lines ---" -ForegroundColor DarkGray
    Get-Content $logFile -Tail 40 | Write-Host -ForegroundColor DarkGray
    exit $process.ExitCode
}

if (-not (Test-Path $outputFile)) {
    Write-Host "BUILD SUCCEEDED but output file not found: $outputFile" -ForegroundColor Red
    Write-Host "Check the build log: $logFile" -ForegroundColor Yellow
    exit 1
}

$sizeMB = [math]::Round((Get-Item $outputFile).Length / 1MB, 1)

Write-Host ""
Write-Host "================================================" -ForegroundColor Green
Write-Host "  BUILD SUCCEEDED!" -ForegroundColor Green
Write-Host "================================================" -ForegroundColor Green
Write-Host "  Output : $outputFile" -ForegroundColor White
Write-Host "  Size   : $sizeMB MB" -ForegroundColor White
Write-Host ""

if (-not $BuildAAB) {
    Write-Host "Install on connected Android device:" -ForegroundColor Yellow
    Write-Host "  adb install -r `"$outputFile`"" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Or transfer the APK to your phone and open it." -ForegroundColor Yellow
} else {
    Write-Host "Your AAB is ready for Google Play Console upload:" -ForegroundColor Yellow
    Write-Host "  $outputFile" -ForegroundColor Cyan
}
Write-Host ""
