# BCity — wizja MVP

> Status: **szkic v0.4** (2026-11-08). ✅ = decyzja, ❓ = otwarte (sekcja 12).

## 1. Jednym zdaniem

**BCity to lekki city builder, w którym sercem gry jest system dróg i zarządzanie ruchem:** gracz projektuje sieć dróg i strefy, miasto rośnie samo, a zła sieć realnie dusi miasto. Gra działa płynnie na przeciętnym sprzęcie — tam, gdzie Cities: Skylines 2 zawodzi.

## 2. Wyróżnik (dlaczego BCity, a nie CS2)

| CS2 | BCity |
|---|---|
| Najlepsze: drogi i ruch | ✅ To jest cała gra — wszystko inne istnieje po to, żeby generować sensowny ruch |
| Najgorsze: wydajność | ✅ Wydajność jest funkcją produktu, z twardym budżetem i benchmarkiem od 1. dnia |
| Setki systemów | Kilka systemów, które razem tworzą presję na sieć dróg |

## 3. Filary produktu

1. **Drogi i ruch są gwiazdą.** Każdy pojazd ma realny cel. Korek ma przyczynę, którą widać i którą da się naprawić.
2. **Wydajność to feature.** Mechanika, która łamie budżet wydajności, nie wchodzi.
3. **Ruch ma konsekwencje.** Zła sieć dusi miasto — ale gracz widzi to z wyprzedzeniem i może odwrócić spiralę.
4. **Mało systemów, głęboko.**

## 4. Kontekst projektu

- ✅ Twórca: **jedna osoba**, zna JavaScript; C# do nauczenia (składnia bliska JS/TS, nauka przy okazji M0).
- ✅ Cel: frajda z budowania, w przyszłości wydanie na **Steam** → technologia i assety nie mogą blokować sprzedaży.
- ✅ Platforma: **desktop** (Windows na start; Linux/Steam Deck jako cel dodatkowy).
- Konsekwencja dla zakresu: każdą funkcję oceniamy pytaniem „czy jedna osoba to utrzyma?”.

## 5. Pętla rozgrywki

```
Buduj drogi → wyznacz strefy → budynki rosną → mieszkańcy dojeżdżają (praca, zakupy),
ciężarówki wożą towary → ruch → korki → miasto zaczyna cierpieć →
gracz analizuje (nakładki, trasy) → przebudowuje sieć (światła, ronda, aleje, objazdy) →
miasto odżywa i rośnie → odblokowanie wyższej gęstości → więcej ruchu → ...
```

✅ Brak limitu populacji i brak „końca gry”. Po ~30 tys. gra toczy się normalnie — gracz buduje dalej, ale nie gwarantujemy już wydajności ani balansu.

## 6. Zakres MVP

### 6.1 Drogi
- ✅ **Swobodne drogi** (krzywe Béziera) z mocnym przyciąganiem: do istniejących dróg, kątów (15°/45°/90°), długości i równoległych odsunięć.
- Typy: **ulica lokalna** (2 pasy), **aleja** (4 pasy), **droga jednokierunkowa** (2 pasy). Autostrady/węzły → po MVP.
- Narzędzia: prosta, łuk, usuwanie, ulepszenie typu drogi bez burzenia, podgląd kosztu przed budową.
- Połączenie z „resztą świata” na krawędzi mapy (pierwsi mieszkańcy, eksport/import towarów, tranzyt).

### 6.2 Skrzyżowania i zarządzanie ruchem
- Skrzyżowania generowane automatycznie, z sensownymi ustawieniami domyślnymi (gracz nie musi niczego ustawiać, ale może).
- **Ronda**: gotowe narzędzie, 2–3 rozmiary.

✅ **Narzędzie skrzyżowań** (panel po kliknięciu w skrzyżowanie):
| Funkcja | Opis |
|---|---|
| Tryb skrzyżowania | brak oznakowania / pierwszeństwo / znaki STOP / sygnalizacja |
| Pierwszeństwo | wybór, które wloty są drogą główną |
| Zakazy skrętu | per wlot: zakaz w lewo / w prawo / na wprost / zawracania |
| Fazy świateł | długość zielonego per faza, opcjonalna osobna faza dla lewoskrętu |
| Pasy | automatyczne strzałki z możliwością prostej zmiany przypisania kierunków do pasów na wlocie (np. lewy: lewo, prawy: prosto+prawo) |
| Podgląd | statystyki wlotów: kolejka, średnie czekanie, przepustowość |

✅ **Narzędzia drogowe:**
- zmiana kierunku drogi jednokierunkowej jednym kliknięciem,
- zakaz wjazdu ciężarówek na odcinek (ochrona osiedli),
- ograniczenie prędkości per odcinek (wpływa na wybór trasy przez kierowców).

Po MVP: pełna edycja połączeń pasów (lane connector), sygnalizacja sterowana ruchem, zielona fala.

### 6.3 Pojazdy i ruch
- **Samochody osobowe** i **ciężarówki** (przemysł → handel, import/eksport).
- Jazda po pasach, utrzymanie odstępu, zmiana pasa przed manewrem, respektowanie świateł i pierwszeństwa.
- Trasy: pathfinding po grafie dróg, koszt = przewidywany czas; kara za zatłoczenie odświeżana okresowo → kierowcy szukają objazdów (co też może przenieść korek w złe miejsce).
- Pojazd zablokowany zbyt długo znika, ale **liczy się jako nieudana podróż** z konsekwencjami (6.5) — to nie jest darmowy „wentyl”.
- Bez parkowania i pieszych.

### 6.4 Strefy i budynki
| Strefa | Gęstości | Rola |
|---|---|---|
| Mieszkalna | lekka, średnia, gęsta | mieszkańcy, wyjazdy do pracy i na zakupy |
| Komercyjna | lekka, średnia, gęsta | praca, cel zakupów, potrzebuje dostaw towarów |
| Przemysłowa | jedna | praca, produkcja towarów, dużo ciężarówek |
| Biurowa | jedna ✅ | dużo pracowników, zero ciężarówek → szczyty dojazdów |

- Strefowanie pędzlem wzdłuż dróg; budynki rosną przy dostępie do drogi i popycie.
- Bez poziomów budynków w MVP.

### 6.5 Ruch dusi miasto ✅
Każdy kluczowy przepływ w mieście jedzie po drogach, więc korek uderza w gospodarkę:

| Kto | Czego potrzebuje | Co się dzieje przy korkach |
|---|---|---|
| Mieszkańcy | dojazd do pracy i sklepów w rozsądnym czasie | spada zadowolenie → spada popyt mieszkalny → wyprowadzki → porzucone domy |
| Firmy (K, B, P) | pracownicy docierający na czas | braki kadrowe → niższa produktywność → porzucenie budynku |
| Handel | dostawy towarów ciężarówkami | pusty magazyn → brak klientów → upadek |
| Przemysł | wywóz towarów | pełny magazyn → wstrzymanie produkcji → zwolnienia |
| Budżet | podatki z działających budynków | spadek dochodów → ryzyko bankructwa |

Zasady projektowe kary:
- **Narasta stopniowo** (minuty gry), nie natychmiast — gracz ma czas zareagować.
- **Widać ją z wyprzedzeniem:** ikony problemu nad budynkami („brak pracowników”, „brak dostaw”, „za długi dojazd”), wykres czasu dojazdu, powiadomienie, gdy rośnie % nieudanych podróży.
- **Jest odwracalna:** naprawa sieci → porzucone budynki mogą zostać ponownie zasiedlone.
- **Ma wskazaną przyczynę:** z ikony problemu jedno kliknięcie prowadzi do nakładki ruchu z zaznaczonym wąskim gardłem.

### 6.6 Popyt i ekonomia
- Wskaźnik popytu **M / K / P / B** z bilansu mieszkańcy ↔ praca ↔ klienci ↔ towary, skorygowany o jakość ruchu.
- Podatki per typ strefy, koszt budowy i utrzymania dróg i świateł.

### 6.7 Progresja ✅
- Kamienie milowe populacji odblokowują: średnią gęstość, aleje, ronda, biura, gęstą zabudowę (kolejność do balansu).
- Bankructwo: ostrzeżenie → kredyt awaryjny → koniec gry.
- Tryb sandbox: wszystko odblokowane, nieograniczone pieniądze.

### 6.8 Interfejs
- Nakładki: **natężenie ruchu**, strefy, popyt, problemy budynków.
- Klik w pojazd: skąd → dokąd, trasa. Klik w drogę: natężenie, średnia prędkość, ile tras przez nią przechodzi.
- Panel miasta: populacja, praca, budżet, średni czas dojazdu, % nieudanych podróży.
- Prędkość: pauza, 1×, 2×, 4×. Zapis/wczytanie.

### 6.9 Mapa ✅
- Jedna płaska mapa (~2,5×2,5 km), bez wody i edycji terenu. Rozmiar mapy jest naturalnym sufitem wzrostu powyżej 30 tys.

## 7. Poza zakresem MVP

Komunikacja publiczna, piesi, rowery, parkingi, usługi miejskie, dzielnice i polityki, teren i woda, mosty, autostrady i węzły, dzień/noc, pory roku, katastrofy, poziomy budynków, pełna edycja połączeń pasów, inteligentna sygnalizacja, mody, multiplayer.

## 8. Wydajność

| Sprzęt | Rozdz. | Cel przy ~30 tys. mieszkańców |
|---|---|---|
| Średni PC (4 rdzenie, GTX 1060 / RX 580, 8 GB RAM) | 1080p | **60 FPS** także przy 4× |
| Laptop ze zintegrowaną grafiką (Iris Xe / Radeon 680M) | 1080p, niskie | **30+ FPS** |
| Steam Deck | 800p | **30+ FPS** |

- ~3–5 tys. pojazdów jednocześnie, RAM < 1,5 GB, zapis < 2 s, wczytanie < 5 s.
- Powyżej 30 tys. — bez gwarancji, ale bez sztucznych limitów.

## 9. Technologia ✅

| Warstwa | Wybór | Dlaczego |
|---|---|---|
| Silnik | **Godot 4.x (.NET)** | darmowy, bez prowizji (Steam), lekki edytor, eksport Windows/Linux |
| Język | **C#** (.NET 8+) | szybki kod symulacji, struktury, `Span<T>`, wielowątkowość |
| Symulacja | osobna biblioteka C# **niezależna od Godota** | testowalna, benchmarkowalna bez renderingu, możliwa praca w osobnym wątku |
| Rendering | `MultiMesh` / `RenderingServer` | tysiące aut i budynków w kilku draw callach |
| Steam (później) | Steamworks.NET lub Facepunch.Steamworks | dojrzałe biblioteki C# |
| Testy | xUnit + headless benchmark | regresje wydajności łapane automatycznie |

Architektura:
```
BCity.Simulation (czysty C#)  ← logika: graf dróg, pojazdy, strefy, popyt, ekonomia
        ↑ komendy (zbuduj drogę, ustaw światła)   ↓ snapshot stanu (pozycje, zdarzenia)
BCity.Game (Godot)            ← kamera, narzędzia, UI, rendering, audio
```

Zasady wydajności:
- **Żadnego Node3D per pojazd/budynek.** Pojazdy i budynki to instancje w `MultiMesh`, aktualizowane jednym buforem `float[]` na klatkę.
- Minimalizacja przejść C# ↔ silnik (każde wywołanie API Godota ma koszt) — dane wysyłane paczkami.
- Symulacja ze stałym tickiem (10–20 Hz) w osobnym wątku; rendering interpoluje pozycje.
- Dane jako tablice struktur (`struct`), zero alokacji w gorących pętlach → brak przycięć od GC.
- Pathfinding (A* / kopiec) z budżetem czasu na tick, cache tras, wielowątkowo.
- Siatka przestrzenna do wyszukiwania sąsiadów; LOD i culling dla budynków.
- Siatki dróg generowane proceduralnie (`ArrayMesh`) tylko dla zmienionych odcinków.
- Scena benchmarkowa od M0; regresja wydajności = błąd blokujący.

## 10. Kryteria sukcesu MVP
1. Gracz w 1–2 h buduje ~30 tys. mieszkańców bez poradnika.
2. Zła sieć wyraźnie dusi miasto, a gracz umie wskazać przyczynę dzięki ikonom i nakładce.
3. Przebudowa (światła, rondo, aleja) mierzalnie skraca czas dojazdu i odwraca spadek miasta.
4. Cele wydajności z sekcji 8 spełnione w scenie benchmarkowej.
5. Testerzy wskazują układanie ruchu jako najfajniejszą część gry.

## 11. Kamienie milowe
| # | Nazwa | Zawartość | Pytanie |
|---|---|---|---|
| M0 | Prototyp ruchu | kamera, drogi proste/łuki ze snapem, graf dróg, auta A→B, benchmark | Czy auta płynnie jeżdżą i czy architektura trzyma budżet? |
| M1 | Zarządzanie ruchem | pierwszeństwo, STOP, światła z fazami, ronda, zmiana pasów, zakazy skrętu, przypisanie pasów, nakładka ruchu | Czy naprawianie korka jest fajne? |
| M2 | Miasto | strefy, wzrost budynków, dom→praca/sklep, ciężarówki | Czy miasto żyje samo? |
| M3 | Konsekwencje | popyt, ekonomia, kara za ruch (6.5), ikony problemów, progresja | Czy jest presja i cel? |
| M4 | MVP | zapis, eksport desktop, optymalizacja, UX, testy | Czy spełniamy cele? |

## 12. Pytania i decyzje

| ID | Pytanie | Decyzja |
|---|---|---|
| Q1 | Cel projektu | ✅ Frajda z budowania, w przyszłości Steam |
| Q2 | Zespół i umiejętności | ✅ Solo, JavaScript |
| Q3 | Platforma | ✅ Desktop |
| Q4 | Technologia | ✅ Godot 4 + C#, symulacja jako osobna biblioteka |
| Q5 | Drogi | ✅ Swobodne, łuki, mocny snap |
| Q6 | Grafika i kamera | ✅ Low-poly 3D, swobodna kamera z obrotem |
| Q7 | Skala | ✅ Cel 30 tys., bez limitu, powyżej bez gwarancji |
| Q8 | Sprzęt minimalny | ✅ Sekcja 8 (roboczo) |
| Q9 | Trudność | ✅ Progresja + bankructwo + sandbox |
| Q10 | Głębia zarządzania ruchem w MVP | ✅ Rozszerzona (sekcja 6.2) |
| Q11 | Biura: jedna czy dwie gęstości | ✅ Jedna |
| Q12 | Teren | ✅ Płaski, bez wody |
| Q13 | Wyróżnik | ✅ Drogi + ruch jako rdzeń; wydajność |
| Q14 | End-game | ✅ Brak — gra toczy się dalej |
| Q15 | Surowość ruchu | ✅ Zła sieć realnie dusi miasto |

