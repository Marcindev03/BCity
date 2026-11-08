# AGENTS.md — BCity

Instrukcje dla agentów AI (Codex, Claude Code). Ten plik jest jedynym źródłem zasad; `CLAUDE.md` tylko go importuje.

## Projekt
BCity: city builder w stylu Cities: Skylines 2, skupiony na drogach i zarządzaniu ruchem. Najważniejsza jest wydajność.
Wizja i zakres: `docs/MVP_VISION.md`. Jeżeli zadanie wykracza poza zakres MVP, zatrzymaj się i zapytaj.

## Stos (przypięte wersje)
- Godot **4.7.2 .NET** (`godot-version.txt`), projekty `net8.0`, budowane .NET SDK 10 (`global.json`). Setup: `docs/SETUP.md`.
- Godot wywołuj przez zmienną środowiskową `GODOT` (konsolowy exe), Blendera przez `BLENDER`.
- Tylko Godot 4 API (`Node3D`, `CharacterBody3D`, `RenderingServer`...). Nie używaj API Godota 3 (`Spatial`, `KinematicBody`) ani GDScriptu.

## Struktura (docelowa)
```
src/BCity.Simulation/        czysty C#, ZERO zależności od Godota (logika gry)
tests/BCity.Simulation.Tests/  testy jednostkowe i scenariuszowe (xUnit)
bench/BCity.Benchmarks/      benchmark headless z progami wydajności
game/                        projekt Godota: kamera, narzędzia, UI, rendering
scenarios/                   scenariusze ruchu (JSON) używane przez testy i benchmark
docs/                        wizja, specyfikacje (docs/specs), decyzje (docs/decisions)
```

## Komendy
| Cel | Komenda |
|---|---|
| Build (wszystko, w tym gra) | `dotnet build BCity.sln` |
| Testy | `dotnet test BCity.sln` |
| Benchmark (próg = exit code 1) | `dotnet run -c Release --project bench/BCity.Benchmarks` |
| Format | `dotnet format BCity.sln` |
| Test dymny gry bez okna | `pwsh -File scripts/smoke.ps1` |
| Gra z renderingiem + zrzut ekranu + FPS | `pwsh -File scripts/screenshot.ps1 [-Frames 600] [-Out artifacts/x.png]` |

Gra wypisuje linie `BCITY_STATS ...` (FPS, liczba pojazdów) i `BCITY_SCREENSHOT ...` — parsuj je zamiast zgadywać. Zrzuty i logi zapisuj w `artifacts/` (ignorowane przez git).

## Definicja „gotowe”
Zanim zakończysz zadanie:
1. `dotnet build` bez ostrzeżeń (ostrzeżenia = błędy).
2. `dotnet test` zielone; nowa logika ma testy.
3. Zmiana w symulacji lub renderingu → uruchom benchmark i podaj wynik w podsumowaniu. Przekroczenie progu = zadanie niegotowe.
4. Zmiana wizualna → zrób zrzut ekranu i go obejrzyj.
5. `dotnet format` bez zmian.

## Zasady architektury
- Logika gry tylko w `BCity.Simulation`. Godot czyta snapshot stanu i wysyła komendy. Nigdy odwrotnie.
- Symulacja deterministyczna: stały tick, jawne ziarno losowości, bez `DateTime.Now` i bez zależności od kolejności słownika.
- Kod pisany w scenach (`.tscn`) ogranicz do minimum; składaj węzły w C#. Nie edytuj ręcznie plików `.import` ani UID.

## Zasady wydajności (twarde)
- Brak alokacji w gorących pętlach (tick symulacji, aktualizacja renderingu): bez LINQ, lambd z przechwytywaniem, `new` klas, boxingu.
- Dane encji jako tablice `struct` (SoA tam, gdzie to ma sens); dostęp przez indeksy, nie referencje.
- Żadnego `Node` per pojazd/budynek. Pojazdy i budynki to instancje `MultiMesh` aktualizowane jednym buforem.
- Wywołania API Godota grupuj paczkami; nie wołaj ich per encję per klatkę.

## Assety 3D (szczegóły: docs/decisions/0002-asset-pipeline.md)
- Model z Blender MCP nie jest gotowy, dopóki nie ma skryptu w `assets-src/`, który go odtwarza i eksportuje `.glb`.
- Budżety: auto ≤ 300 tri (LOD1 ≤ 80), budynek ≤ 1500 tri (LOD1 ≤ 300). Wspólna paleta `palette.png`.
- 1 j. = 1 m, Y w górę, przód = −Z.
- Każdy zewnętrzny asset wpisz do `assets/LICENSES.md`. Nie używaj Hunyuan3D ani licencji NC/ND.

## Lokalizacja
- Angielski jest językiem źródłowym, polski drugi. Żadnych napisów widocznych dla gracza w kodzie — tylko klucze tłumaczeń (pliki w `game/localization/`).
- Nowy klucz → dodaj wartość EN; brak PL jest dozwolony, ale test zgłasza listę brakujących.

## Zapis gry
- Format zapisu ma pole `version` (liczba całkowita). Każda zmiana formatu podnosi wersję i dodaje migrację `vN → vN+1` z testem na przykładowym starym zapisie w `tests/fixtures/saves/`.

## Styl
- Kod, nazwy, komentarze i commity po angielsku. Dokumentacja produktowa po polsku.
- Pliki < ~400 linii, jedna odpowiedzialność. Nullable włączone.
- Commity: Conventional Commits (`feat(traffic): ...`).

## Praca z zadaniami
- Zadanie = plik `docs/specs/NNNN-tytul.md` z kryteriami akceptacji (nie GitHub Issues). Trzymaj się zakresu specyfikacji.
- Ważna decyzja techniczna → krótki ADR w `docs/decisions/NNNN-tytul.md`.
- Nie zmieniaj progów benchmarku ani nie wyłączaj testów, aby przeszły. Zgłoś problem.



