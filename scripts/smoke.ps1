# Headless smoke test: imports the Godot project and runs the main scene without a window.
param([int]$Frames = 120)
$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
$godot = if ($env:GODOT) { $env:GODOT } else { [Environment]::GetEnvironmentVariable("GODOT", "User") }
if (-not $godot) { throw "Set the GODOT environment variable to the Godot .NET console executable." }

dotnet build (Join-Path $root "game/BCity.Game.csproj") --nologo -v q
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

& $godot --headless --path (Join-Path $root "game") --import
& $godot --headless --path (Join-Path $root "game") -- "--frames=$Frames"
exit $LASTEXITCODE
