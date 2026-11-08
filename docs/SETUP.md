# Developer setup (Windows)

Installed and verified on 2026-11-08.

| Tool | Version | Path / how to call |
|---|---|---|
| .NET SDK | 10.0.403 (pinned in `global.json`) + 8.0.427 (runtime required by Godot editor tools) | `dotnet` |
| Godot .NET | 4.7.2 stable mono (pinned in `godot-version.txt`) | env `GODOT` → console exe; folder in user PATH |
| Blender | 4.5.10 LTS | env `BLENDER`; `blender -b -P script.py` |
| uv | 0.13.7 | `uv`, `uvx` |
| blender-mcp | 2.0.0 (`uv tool install blender-mcp`) | `C:\Users\Marcin\.local\bin\blender-mcp.exe` |
| Git LFS | 3.7.1 | `git lfs` |

Godot projects target `net8.0` (Godot 4.7 default) and are built with SDK 10.

## Agents
- **Codex:** Blender MCP registered globally in `~/.codex/config.toml` (`[mcp_servers.blender]`).
- **Claude Code:** project-scoped `.mcp.json` (approve the server on first launch).
- Blender MCP needs Blender running: sidebar (N) → **BlenderMCP** tab → **Connect**. Only one agent may be connected at a time.

## Fresh machine
```powershell
winget install -e --id Microsoft.DotNet.SDK.10
winget install -e --id Microsoft.DotNet.SDK.8
winget install -e --id GodotEngine.GodotEngine.Mono --version 4.7.2
winget install -e --id BlenderFoundation.Blender.LTS.4.5
winget install -e --id astral-sh.uv
uv tool install blender-mcp
git lfs install
```
Then install `addon.py` from https://github.com/ahujasid/blender-mcp in Blender (Edit → Preferences → Add-ons → Install from disk) and set `GODOT` / `BLENDER` env vars.
