# 0001 — Projekt agent-first

Status: przyjęte (2026-11-08)

## Kontekst
BCity tworzy jedna osoba, a większość kodu piszą agenci AI (Claude Code, Codex). Agenci nie „grają” w grę: widzą tylko tekst, wyniki komend i obrazy.

## Decyzja
1. Wspólne instrukcje w `AGENTS.md`; `CLAUDE.md` importuje je (`@AGENTS.md`).
2. Każdą rzecz da się sprawdzić z linii poleceń: build, testy, benchmark, zrzut ekranu.
3. Logika w czystej bibliotece C# (`BCity.Simulation`) — testowalna bez Godota.
4. Deterministyczna symulacja i scenariusze ruchu w JSON → testy zachowania ruchu na liczbach.
5. Wydajność jako automatyczny test z progami.
6. Zadania jako specyfikacje z kryteriami akceptacji (`docs/specs/`), decyzje jako ADR.
7. Grafika proceduralna / low-poly, aby agenci mogli ją tworzyć kodem.
8. Człowiek: właściciel produktu, recenzent i playtester.

## Konsekwencje
+ Agenci sami weryfikują swoją pracę, mniej regresji.
+ Wiedza projektu nie ginie między sesjami.
− Więcej pracy na start (infrastruktura testów, scenariuszy, benchmarku) — robimy to w M0.
