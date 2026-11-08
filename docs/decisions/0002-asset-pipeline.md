# 0002 — Pipeline assetów 3D

Status: przyjęte (2026-11-08)

## Decyzja
- **Budynki:** generator proceduralny w C# (`BCity.Simulation` daje parametry działki, `game/` buduje siatkę). Z góry generujemy 20–40 archetypów na strefę/gęstość i instancjonujemy je przez `MultiMesh`; daleko od kamery łączymy budynki w siatkę per chunk.
- **Samochody i budynki specjalne:** Blender. Agent szuka formy przez **Blender MCP**, a zaakceptowany model zapisuje jako skrypt `assets-src/<kategoria>/<nazwa>.py`, który eksportuje `.glb` do `game/assets/` i render podglądu PNG. Źródłem prawdy jest skrypt, nie plik `.blend`.
- **Godot MCP:** nieużywany; agent pracuje z Godotem przez CLI.
- **Placeholdery:** kostki + Kenney (CC0) w M0–M1.

## Zasady
- Jedna paleta kolorów (tekstura `palette.png`) i jeden materiał dla większości modeli.
- Budżety: samochód LOD0 ≤ 300 trójkątów, LOD1 ≤ 80; budynek LOD0 ≤ 1500, LOD1 ≤ 300. Skrypt eksportu sprawdza budżet i przerywa przy przekroczeniu.
- Skala: 1 jednostka = 1 m, oś Y w górę, przód modelu = −Z (konwencja Godota).
- Każdy zewnętrzny asset → wpis w `assets/LICENSES.md`. Zakaz: Hunyuan3D (licencja nie obejmuje UE), modele z licencją NC/ND.
- Jeden klient MCP naraz na instancję Blendera.
