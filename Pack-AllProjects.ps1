#Requires -Version 5.1
<#
.SYNOPSIS
    Produces Release NuGet packages for every Frends.AzureBlobStorage.* project
    (excluding test projects). Optionally bumps the patch version before packing.

.DESCRIPTION
    1. Finds all *.csproj files matching Frends.AzureBlobStorage.* (non-test) under
       the script root.
    2. Runs  dotnet pack --configuration Release  and places the output in
       C:\Temp\AzureBlob.

    Use -BumpVersion to also increment the patch segment of <Version> in each
    project file before packing (preserving any pre-release suffix, e.g. "-beta").

.PARAMETER RootPath
    Root directory to search for project files. Defaults to the script directory.

.PARAMETER OutputDir
    Directory where NuGet packages are written. Defaults to C:\Temp\AzureBlob.

.PARAMETER BumpVersion
    When specified, increments the patch segment of <Version> in each project file
    before packing. Without this switch the version is left unchanged.

.EXAMPLE
    .\Pack-AllProjects.ps1
    Packs all projects using their current versions.

.EXAMPLE
    .\Pack-AllProjects.ps1 -BumpVersion
    Bumps each project's patch version by 1, then packs.
#>

[CmdletBinding()]
param (
    [string] $RootPath  = $PSScriptRoot,
    [string] $OutputDir = "C:\Temp\AzureBlob",
    [switch] $BumpVersion
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

# ---------------------------------------------------------------------------
# Ensure output directory exists
# ---------------------------------------------------------------------------
if (-not (Test-Path $OutputDir)) {
    New-Item -ItemType Directory -Path $OutputDir | Out-Null
    Write-Host "Created output directory: $OutputDir" -ForegroundColor Cyan
}

# ---------------------------------------------------------------------------
# Discover project files
# ---------------------------------------------------------------------------
$projects = Get-ChildItem -Path $RootPath -Recurse -Filter "*.csproj" |
    Where-Object { $_.Name -match "^Frends\.AzureBlobStorage\." -and $_.Name -notmatch "Tests" }

if ($projects.Count -eq 0) {
    Write-Warning "No matching project files found under '$RootPath'."
    exit 0
}

Write-Host ""
Write-Host "Found $($projects.Count) project(s):" -ForegroundColor Cyan
$projects | ForEach-Object { Write-Host "  $_" }
Write-Host ""

if ($BumpVersion) {
    Write-Host "Version bump: ENABLED" -ForegroundColor Yellow
} else {
    Write-Host "Version bump: disabled (use -BumpVersion to enable)" -ForegroundColor DarkGray
}
Write-Host ""

# ---------------------------------------------------------------------------
# Process each project
# ---------------------------------------------------------------------------
$results = [System.Collections.Generic.List[PSCustomObject]]::new()

foreach ($proj in $projects) {
    Write-Host "?????????????????????????????????????????????" -ForegroundColor DarkGray
    Write-Host "Processing: $($proj.Name)" -ForegroundColor Yellow

    # --- Read project XML ---
    [xml] $xml = Get-Content $proj.FullName -Raw

    $versionNode = $xml.SelectSingleNode("//Version")
    if ($null -eq $versionNode) {
        Write-Warning "  No <Version> element found – skipping."
        continue
    }

    $currentVersion = $versionNode.InnerText.Trim()
    $packedVersion  = $currentVersion
    Write-Host "  Version : $currentVersion"

    if ($BumpVersion) {
        # --- Parse version (supports optional pre-release suffix, e.g. "1.2.3-beta") ---
        if ($currentVersion -match '^(\d+)\.(\d+)\.(\d+)(.*)$') {
            $major      = [int] $Matches[1]
            $minor      = [int] $Matches[2]
            $patch      = [int] $Matches[3]
            $prerelease =       $Matches[4]   # e.g. "-beta" or ""
        } else {
            Write-Warning "  Version '$currentVersion' does not match expected format (MAJOR.MINOR.PATCH[suffix]) – skipping version bump."
        }

        $packedVersion = "$major.$minor.$($patch + 1)$prerelease"
        Write-Host "  Bumped  : $packedVersion" -ForegroundColor Green

        # --- Write bumped version back to project file ---
        $versionNode.InnerText = $packedVersion
        $xml.Save($proj.FullName)
    }

    # --- Pack ---
    Write-Host "  Packing..." -ForegroundColor Cyan
    $packArgs = @(
        "pack"
        $proj.FullName
        "--configuration", "Release"
        "--output", $OutputDir
        "--no-restore"
    )

    & dotnet @packArgs
    $exitCode = $LASTEXITCODE

    if ($exitCode -ne 0) {
        Write-Warning "  dotnet pack exited with code $exitCode for $($proj.Name)"
    } else {
        Write-Host "  Package written to: $OutputDir" -ForegroundColor Green
    }

    $results.Add([PSCustomObject]@{
        Project         = $proj.Name
        Version         = if ($BumpVersion) { "$currentVersion  ?  $packedVersion" } else { $currentVersion }
        PackSuccess     = ($exitCode -eq 0)
    })
}

# ---------------------------------------------------------------------------
# Summary
# ---------------------------------------------------------------------------
Write-Host ""
Write-Host "?????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host "Summary" -ForegroundColor Cyan
Write-Host "?????????????????????????????????????????????" -ForegroundColor Cyan
$results | Format-Table -AutoSize
Write-Host "Packages are in: $OutputDir" -ForegroundColor Cyan
