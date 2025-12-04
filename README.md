# ISOlver
## Program do rozwiązywania problemu izomorfizmu podgrafów

Problem izomorfizmu podgrafów (ang. *Subgraph Isomorphism Problem*, SIP) dany jest przez dwa grafy na wejściu, **p** i **g**, t.że  V(**p**) <= V(**g**). Program ma za zadanie znaleźć odpowiedź na dwa pytania: po pierwsze, czy istnieje takie mapowanie 
**M**: V(**p**) ->  V(**g**), przy którym dla każdej krawędzi (*u*, *v*) w **p** istnieje krawędź (**M(*u*)**, **M(*v*)**) w **g**. Problem ten jest NP-zupełny, czyli nie istnieją szybkie dokładne algorytmy rozwiązujące go. W ISOlver zaimplementowane są
dwa algorytmy: algorytm Ullmana, czyli algorytm dokładny o złożoności wykładniczej, rozszerzony o znajdywanie dopełnienia grafu docelowego **g**, oraz algorytm przybliżony na podstawie algorytmu węgierskiego (tzw. algorytmu Munkersa), który działa znacznie szybciej.

## Instrukcja

Program akceptuje dane w postaci pojedynczego pliku tekstowego. W pierwszej linijce pliku powinna znajdywać się pojedyncza liczba całkowita oznaczająca liczbę wierzchołków pierwszego grafu (ozn. *n*), po czym w kolejnych n liniach powinno znajdywać się po n liczb
oddzielonych spacją, z których każda to 0 lub 1. 1 w i-tym wierszu i j-tej pozycji w tym wierszu oznacza istnienie krawędzi z i-tego do j-tego wierzchołka, tzn. dane wejściowe są po prostu macierzą sąsiedztwa pierwszego grafu. Po n-tej linijce macierzy sąsiedztwa, 
w następnych linijkach powinna znaleźć się analogiczna reprezentacja drugiego grafu.

### Argumenty wiersza polecenia

Program przyjmuje następujące argumenty wiersza polecenia:

| Argument | Wartość argumentu | Działanie | Wartość domyślna |
| -------- | --------- | ----------------- | ---------------- |
| --input=  | nazwa pliku | ścieżka do pliku z danymi wejściowymi, np. "/Downloads/input_1.txt" | "input.txt" |
| --output= | nazwa pliku | ścieżka do pliku, gdzie zostanie zapisane rozwiązanie, np. "/Documents/output_1.txt" | "output.txt" |
| --inputDir= | nazwa folderu | ścieżka do folderu z plikami z danymi wejściowymi do masowego rozwiązywania, np. "/Documents/SIPinputs" | Brak |
| --outputDir= | nazwa folderu | ścieżka do folderu gdzie wstawione zostaną pliki rozwiązań przy masowym rozwiązaniu lub wygenerowane dane przy opcji --generate, np. "/Documents/SIPoutputs" | Brak |
| --seed= | nazwa folderu | ścieżka do pliku z ziarnem generatora dla --generate | Brak |
| -g, --generate | Brak | Program generuje pliki z danymi wejściowymi przy ustawieniu tej flagi | Brak |
| -e, --exact | Brak | Do rozwiązania używany jest algorytm dokładny | Brak |
| -a, --approximate | Brak | Do rozwiązania używany jest algorytm przybliżony | Brak |
| -c, --console | Brak | Rozwiązania są wypisane również na standardowe wyjście | Brak |
| -p, --append | Brak | Rozwiązania są dopisywane na koniec plików, jeśli te już istniały | Brak |
| -v, --verbose | Brak | Do rozwiązań dopisana jest informacja o czasie działania programu | Brak |
| -l, --clean | Brak | Folder wyjściowy --outputDir jest czyszczony, jego zawartość jest bezpowrotnie usuwana | Brak |
| -h, -?, --help | Brak | Program pokazuje działanie argumentów wiersza polecenia | Brak |



## Przykłady użycia


