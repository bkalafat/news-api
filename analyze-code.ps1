# Code Quality Analysis Script for News Portal Backend
# This script runs comprehensive code analysis and reports issues

param(
    [switch]$Fix,              # Automatically fix issues where possible
    [switch]$Verbose,          # Show detailed output
    [switch]$FailOnWarnings    # Exit with error code if warnings found
)

$ErrorActionPreference = "Stop"
$ProjectPath = "c:\dev\newsportal\backend\newsApi.csproj"
$SolutionPath = "c:\dev\newsportal\newsApi.sln"

Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Cyan
Write-Host "  News Portal - Code Quality Analysis" -ForegroundColor Cyan
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Cyan
Write-Host ""

# Function to run a command and capture output
function Invoke-AnalysisCommand {
    param(
        [string]$Command,
        [string]$Description,
        [string[]]$Arguments
    )
    
    Write-Host "▶ $Description..." -ForegroundColor Yellow
    
    if ($Verbose) {
        & $Command @Arguments
    } else {
        & $Command @Arguments 2>&1 | Out-Null
    }
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "  ✓ Completed" -ForegroundColor Green
    } else {
        Write-Host "  ✗ Issues found" -ForegroundColor Red
    }
    Write-Host ""
    
    return $LASTEXITCODE
}

# 1. Clean the solution
Write-Host "1️⃣  Cleaning Solution" -ForegroundColor Magenta
Invoke-AnalysisCommand -Command "dotnet" -Description "Cleaning build artifacts" -Arguments @("clean", $SolutionPath, "-v", "minimal")

# 2. Restore packages
Write-Host "2️⃣  Restoring Packages" -ForegroundColor Magenta
Invoke-AnalysisCommand -Command "dotnet" -Description "Restoring NuGet packages" -Arguments @("restore", $SolutionPath)

# 3. Build with full analysis
Write-Host "3️⃣  Building with Code Analysis" -ForegroundColor Magenta
$buildArgs = @(
    "build",
    $SolutionPath,
    "-c", "Debug",
    "--no-restore",
    "/p:EnforceCodeStyleInBuild=true",
    "/p:AnalysisMode=AllEnabledByDefault",
    "/p:AnalysisLevel=latest-all"
)

if ($FailOnWarnings) {
    $buildArgs += "/p:TreatWarningsAsErrors=true"
}

Invoke-AnalysisCommand -Command "dotnet" -Description "Building solution with analyzers" -Arguments $buildArgs

# 4. Run code formatting check
Write-Host "4️⃣  Code Formatting Check" -ForegroundColor Magenta
$formatArgs = @(
    "format",
    $SolutionPath,
    "--verify-no-changes",
    "--verbosity", "diagnostic"
)

if ($Fix) {
    Write-Host "  → Fixing formatting issues automatically..." -ForegroundColor Yellow
    $formatArgs = @(
        "format",
        $SolutionPath,
        "--verbosity", "diagnostic"
    )
}

Invoke-AnalysisCommand -Command "dotnet" -Description "Checking code formatting" -Arguments $formatArgs

# 5. Generate code metrics report
Write-Host "5️⃣  Code Metrics Analysis" -ForegroundColor Magenta
Write-Host "  → Analyzing code complexity, maintainability..." -ForegroundColor Yellow

# Run build with binary log for detailed analysis
$binaryLogPath = Join-Path $PSScriptRoot "analysis.binlog"
dotnet build $SolutionPath --no-restore /bl:$binaryLogPath | Out-Null

if (Test-Path $binaryLogPath) {
    Write-Host "  ✓ Binary log created: $binaryLogPath" -ForegroundColor Green
} else {
    Write-Host "  ⚠ Could not create binary log" -ForegroundColor Yellow
}
Write-Host ""

# 6. Find unused code
Write-Host "6️⃣  Detecting Unused Code" -ForegroundColor Magenta
Write-Host "  → Searching for unused variables, methods, and members..." -ForegroundColor Yellow

# Create a temporary file to capture warnings
$warningFile = Join-Path $env:TEMP "newsportal-warnings.txt"
dotnet build $SolutionPath --no-restore 2>&1 | Select-String -Pattern "(IDE0051|IDE0052|IDE0059|IDE0060|RCS1163|RCS1213)" > $warningFile

if ((Get-Content $warningFile -ErrorAction SilentlyContinue).Length -gt 0) {
    Write-Host "  ⚠ Found unused code:" -ForegroundColor Yellow
    Get-Content $warningFile | ForEach-Object {
        Write-Host "    $_" -ForegroundColor DarkYellow
    }
} else {
    Write-Host "  ✓ No unused code detected" -ForegroundColor Green
}
Write-Host ""

# 7. Find redundant code
Write-Host "7️⃣  Detecting Redundant Code" -ForegroundColor Magenta
Write-Host "  → Searching for redundant assignments, casts, etc..." -ForegroundColor Yellow

$redundantFile = Join-Path $env:TEMP "newsportal-redundant.txt"
dotnet build $SolutionPath --no-restore 2>&1 | Select-String -Pattern "(RCS1074|RCS1097|RCS1118|RCS1129|RCS1174|RCS1188|RCS1212|IDE0004)" > $redundantFile

if ((Get-Content $redundantFile -ErrorAction SilentlyContinue).Length -gt 0) {
    Write-Host "  ⚠ Found redundant code:" -ForegroundColor Yellow
    Get-Content $redundantFile | ForEach-Object {
        Write-Host "    $_" -ForegroundColor DarkYellow
    }
} else {
    Write-Host "  ✓ No redundant code detected" -ForegroundColor Green
}
Write-Host ""

# 8. Summary Report
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Cyan
Write-Host "  Analysis Complete!" -ForegroundColor Cyan
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Cyan
Write-Host ""

# Count issues
$totalWarnings = (dotnet build $SolutionPath --no-restore 2>&1 | Select-String -Pattern "warning").Count
$totalErrors = (dotnet build $SolutionPath --no-restore 2>&1 | Select-String -Pattern "error").Count

Write-Host "📊 Results:" -ForegroundColor White
Write-Host "   Errors:   $totalErrors" -ForegroundColor $(if ($totalErrors -gt 0) { "Red" } else { "Green" })
Write-Host "   Warnings: $totalWarnings" -ForegroundColor $(if ($totalWarnings -gt 0) { "Yellow" } else { "Green" })
Write-Host ""

# Recommendations
Write-Host "💡 Recommendations:" -ForegroundColor White
Write-Host "   • Run 'dotnet format' to auto-fix formatting issues" -ForegroundColor Gray
Write-Host "   • Review warnings in Visual Studio or Rider for detailed info" -ForegroundColor Gray
Write-Host "   • Use 'dotnet build /p:TreatWarningsAsErrors=true' for strict mode" -ForegroundColor Gray
Write-Host ""

# Quick Actions
Write-Host "🚀 Quick Actions:" -ForegroundColor White
Write-Host "   • Fix formatting:  .\analyze-code.ps1 -Fix" -ForegroundColor Gray
Write-Host "   • Verbose output:  .\analyze-code.ps1 -Verbose" -ForegroundColor Gray
Write-Host "   • Strict mode:     .\analyze-code.ps1 -FailOnWarnings" -ForegroundColor Gray
Write-Host ""

# Cleanup
Remove-Item $warningFile -ErrorAction SilentlyContinue
Remove-Item $redundantFile -ErrorAction SilentlyContinue

# Exit with appropriate code
if ($FailOnWarnings -and $totalWarnings -gt 0) {
    Write-Host "❌ Analysis failed due to warnings (strict mode)" -ForegroundColor Red
    exit 1
}

if ($totalErrors -gt 0) {
    Write-Host "❌ Analysis failed due to errors" -ForegroundColor Red
    exit 1
}

Write-Host "✅ Analysis passed successfully!" -ForegroundColor Green
exit 0
