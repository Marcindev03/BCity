# Runs the game with real rendering for N frames, saves a screenshot and prints BCITY_STATS.
# Usage: scripts/screenshot.ps1 [-Frames 600] [-Out artifacts/screenshot.png]
param(
    [int]$Frames = 600,
    [string]$Out = "artifacts/screenshot.png"
)
$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
$godot = if ($env:GODOT) { $env:GODOT } else { [Environment]::GetEnvironmentVariable("GODOT", "User") }
if (-not $godot) { throw "Set the GODOT environment variable to the Godot .NET console executable." }

$outPath = [IO.Path]::GetFullPath((Join-Path $root $Out))
New-Item -ItemType Directory -Force (Split-Path $outPath) | Out-Null

dotnet build (Join-Path $root "game/BCity.Game.csproj") --nologo -v q
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

& $godot --path (Join-Path $root "game") -- "--frames=$Frames" "--screenshot=$outPath"
exit $LASTEXITCODE
