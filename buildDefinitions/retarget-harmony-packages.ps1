<#
.SYNOPSIS
    Point a generated Harmony Core solution at a specific set of Harmony.Core packages.

.DESCRIPTION
    Project templates pin the released Harmony.Core versions. A smoke test has to exercise the
    packages produced by the run under test instead, so this writes a nuget.config that restores
    from a local feed and rewrites the Harmony.Core package references to the version in it.

    The <clear /> element drops machine and user level sources inherited on the agent, such as
    stale local feeds registered by other pipelines, so the solution restores from exactly the two
    sources named here.

.PARAMETER SolutionDir
    Root of the generated solution. Its nuget.config is written here and every project beneath it
    is rewritten.

.PARAMETER LocalFeed
    Folder holding the .nupkg files to restore from.

.PARAMETER Version
    Version to pin the Harmony.Core packages to. Must be present in the local feed.

.EXAMPLE
    .\retarget-harmony-packages.ps1 -SolutionDir C:\work\hcdemo -LocalFeed C:\work\packages -Version 10.0.51
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)] [string] $SolutionDir,
    [Parameter(Mandatory = $true)] [string] $LocalFeed,
    [Parameter(Mandatory = $true)] [string] $Version
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$SolutionDir = (Resolve-Path $SolutionDir).Path
$LocalFeed = (Resolve-Path $LocalFeed).Path
$packageIds = 'Harmony.Core', 'Harmony.Core.EF', 'Harmony.Core.OData', 'Harmony.Core.AspNetCore'

Write-Host "solution:   $SolutionDir"
Write-Host "local feed: $LocalFeed"
Write-Host "version:    $Version"

foreach ($id in $packageIds) {
    if (-not (Test-Path (Join-Path $LocalFeed "$id.$Version.nupkg"))) {
        Write-Host "`nPackages present in the feed:"
        Get-ChildItem $LocalFeed -Filter '*.nupkg' | Select-Object -ExpandProperty Name | Sort-Object | ForEach-Object { Write-Host "  $_" }
        throw "$id.$Version.nupkg is not in $LocalFeed"
    }
}

$nugetConfig = @"
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="local" value="$LocalFeed" />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
</configuration>
"@
$nugetConfig | Out-File -FilePath (Join-Path $SolutionDir 'nuget.config') -Encoding utf8
Write-Host "wrote $(Join-Path $SolutionDir 'nuget.config')"

$escaped = ($packageIds | ForEach-Object { [regex]::Escape($_) }) -join '|'
$pattern = '(?s)(<PackageReference\s+Include="(' + $escaped + ')"\s*>\s*<Version>)[^<]+(</Version>)'

$changed = 0
Get-ChildItem -Path $SolutionDir -Recurse -Filter '*.synproj' | ForEach-Object {
    $text = Get-Content -Raw $_.FullName
    $new = [regex]::Replace($text, $pattern, ('${1}' + $Version + '${3}'))
    if ($new -ne $text) {
        [IO.File]::WriteAllText($_.FullName, $new)
        $changed++
        Write-Host "  retargeted $($_.FullName)"
    }
}
if ($changed -eq 0) { throw 'no Harmony.Core package reference was retargeted' }
Write-Host "Harmony.Core references pinned to $Version in $changed project(s)"
