<#
.SYNOPSIS
    Produce a Linux deployment of a harmonydemo solution, the way the template's publish.bat does.

.DESCRIPTION
    Used by the LinuxSmokeTestPublish job, and runnable by hand against any generated harmonydemo
    solution. It performs the Linux-specific half of publish.bat: build the Traditional Bridge host
    program for linux64, publish Services.Host self-contained for linux-x64, and assemble the files
    a real deployment ships.

    This does not call publish.bat itself for two reasons. publish.bat aborts up front unless 7-Zip
    is installed, because it always ends by zipping the deployment and deleting the folder, and a
    smoke test wants the folder rather than the zip. It also checks its build results with
    "if errorlevel 0", which is true for every exit code, so a failed build inside it goes
    undetected. Both are worth fixing in the template; until then this script keeps the CI signal
    honest.

.PARAMETER SolutionDir
    Root of the generated harmonydemo solution.

.PARAMETER DeployDir
    Directory to assemble the deployment into. Created if missing.

.PARAMETER SmokeTestScript
    Optional path to linux-smoke-test.sh. When given it is copied into the deployment with LF line
    endings so the artifact can test itself.

.EXAMPLE
    .\publish-demo-linux.ps1 -SolutionDir C:\work\hcdemo -DeployDir C:\work\hcdemo-linux
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)] [string] $SolutionDir,
    [Parameter(Mandatory = $true)] [string] $DeployDir,
    [string] $Configuration = 'Release',
    [string] $SmokeTestScript
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

function Step($message) { Write-Host "`n=== $message" -ForegroundColor Cyan }

$SolutionDir = (Resolve-Path $SolutionDir).Path
if (-not (Test-Path $DeployDir)) { New-Item -ItemType Directory -Force $DeployDir | Out-Null }
$DeployDir = (Resolve-Path $DeployDir).Path
$bridgeDir = Join-Path $SolutionDir 'TraditionalBridge'

Write-Host "solution:   $SolutionDir"
Write-Host "deploy to:  $DeployDir"

# -----------------------------------------------------------------------------------------------
Step 'Locate MSBuild'

# Same lookup publish.bat uses, so this works wherever publish.bat works.
$vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
if (-not (Test-Path $vswhere)) { throw "The Visual Studio Installer was not found at $vswhere" }
$msbuild = & $vswhere -latest -products * -requires Microsoft.Component.MSBuild -find 'MSBuild\**\Bin\MSBuild.exe' |
           Select-Object -First 1
if (-not $msbuild) { throw 'vswhere did not find MSBuild' }
Write-Host $msbuild

# -----------------------------------------------------------------------------------------------
Step 'Build the Traditional Bridge host program for linux64'

# publish.bat builds the bridge with Platform=linux64 when it targets Linux. A solution build maps
# the bridge to Release|x86, which yields a 32-bit Windows program that a 64-bit Synergy runtime on
# Linux cannot run. Remove every previous bridge product first so a Windows build left by the
# solution build can never be mistaken for this one.
Get-ChildItem -Path $bridgeDir -Recurse -Include 'host.dbr', 'host.dbp' -ErrorAction SilentlyContinue |
    ForEach-Object { Write-Host "  removing stale $($_.FullName)"; Remove-Item $_.FullName -Force }

& $msbuild (Join-Path $bridgeDir 'TraditionalBridge.synproj') `
    -target:Rebuild `
    -p:Platform=linux64 `
    -p:Configuration=$Configuration `
    "-p:SolutionDir=$SolutionDir\" `
    -verbosity:minimal -nologo
if ($LASTEXITCODE -ne 0) { throw "The linux64 bridge build failed with exit code $LASTEXITCODE" }

# The bridge project declares an output path per platform and also an EXE: logical. Depending on
# whether that logical resolves the program lands in EXE, under bin\linux64, or only in the obj
# tree. Everything was cleared above, so the first hit here is always this build.
$dbr = 'EXE', 'bin\linux64\Release', "bin\linux64\$Configuration", 'obj\linux64\Release', "obj\linux64\$Configuration" |
       ForEach-Object { Join-Path $bridgeDir (Join-Path $_ 'host.dbr') } |
       Where-Object { Test-Path $_ } | Select-Object -First 1
if (-not $dbr) {
    Write-Host 'No host.dbr was produced. Everything under TraditionalBridge named host.*:'
    Get-ChildItem -Path $bridgeDir -Recurse -Include 'host.*' -ErrorAction SilentlyContinue |
        Select-Object FullName, LastWriteTime | Format-Table -AutoSize
    throw 'the linux64 bridge build produced no host.dbr'
}
Write-Host "  bridge host program: $dbr"

# -----------------------------------------------------------------------------------------------
Step 'Publish Services.Host for linux-x64'

Push-Location $SolutionDir
try {
    & dotnet publish (Join-Path $SolutionDir 'Services.Host\Services.Host.synproj') `
        -c $Configuration -r linux-x64 --self-contained true `
        -p:platform=AnyCPU -p:PublishTrimmed=false `
        -o $DeployDir -nologo -v minimal
    if ($LASTEXITCODE -ne 0) { throw "dotnet publish failed with exit code $LASTEXITCODE" }
} finally { Pop-Location }

# -----------------------------------------------------------------------------------------------
Step 'Assemble the deployment'

Copy-Item $dbr $DeployDir -Force
# host.dbp is a Synergy prototype file. publish.bat ships it, but launch.sh runs "dbs host.dbr" and
# nothing reads the prototype at run time, so its absence is not fatal.
$dbp = Join-Path (Split-Path $dbr) 'host.dbp'
if (Test-Path $dbp) { Copy-Item $dbp $DeployDir -Force }
else { Write-Host "  note: no host.dbp beside the host program; not needed to run the bridge" }

foreach ($relative in 'TraditionalBridge\launch.sh', 'Linux\startserver.sh', 'Linux\stopserver.sh',
                      'Linux\check.sh', 'Linux\dump.sh') {
    $path = Join-Path $SolutionDir $relative
    if (-not (Test-Path $path)) { throw "missing $relative" }
    Copy-Item $path $DeployDir -Force
}
Copy-Item (Join-Path $SolutionDir 'Linux\startserver.*.config') $DeployDir -Force

$sampleData = Join-Path $DeployDir 'SampleData'
New-Item -ItemType Directory -Force $sampleData | Out-Null
Copy-Item (Join-Path $SolutionDir 'SampleData\*.txt') $sampleData -Force
Copy-Item (Join-Path $SolutionDir 'SampleData\*.xdl') $sampleData -Force
Copy-Item (Join-Path $SolutionDir 'SampleData\sysparams.txt') (Join-Path $sampleData 'sysparams.ddf') -Force

& dotnet dev-certs https --export-path (Join-Path $DeployDir 'Services.Host.pfx') --password 'p@ssw0rd' --quiet
if (-not (Test-Path (Join-Path $DeployDir 'Services.Host.pfx'))) { throw 'dev-certs export failed' }

if ($SmokeTestScript) {
    if (-not (Test-Path $SmokeTestScript)) { throw "missing $SmokeTestScript" }
    # The repo normalizes text files to CRLF in the working tree, and bash cannot run a CRLF
    # script. The pipeline strips CR the same way for startagent.sh before using it in a container.
    $target = Join-Path $DeployDir (Split-Path $SmokeTestScript -Leaf)
    ((Get-Content -Raw $SmokeTestScript) -replace "`r", '') | Set-Content -NoNewline -Encoding utf8 $target
    Write-Host "  shipped $(Split-Path $SmokeTestScript -Leaf) with LF line endings"
}

# -----------------------------------------------------------------------------------------------
Step 'Verify the deployment'

foreach ($required in 'Services.Host', 'Services.Host.pfx', 'host.dbr', 'launch.sh',
                      'startserver.sh', 'stopserver.sh', 'check.sh') {
    if (-not (Test-Path (Join-Path $DeployDir $required))) { throw "deployment is missing $required" }
}
Get-ChildItem $DeployDir | Where-Object { -not $_.PSIsContainer } |
    Sort-Object Name | Select-Object Name, Length | Format-Table -AutoSize
Write-Host "Linux deployment assembled in $DeployDir"
