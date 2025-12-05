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
| `--input=`  | nazwa pliku | ścieżka do pliku z danymi wejściowymi, np. "/Downloads/input_1.txt" | "input.txt" |
| `--output=` | nazwa pliku | ścieżka do pliku, gdzie zostanie zapisane rozwiązanie, np. "/Documents/output_1.txt" | "output.txt" |
| `--inputDir=` | nazwa folderu | ścieżka do folderu z plikami z danymi wejściowymi do masowego rozwiązywania, np. "/Documents/SIPinputs" | Brak |
| `--outputDir=` | nazwa folderu | ścieżka do folderu gdzie wstawione zostaną pliki rozwiązań przy masowym rozwiązaniu lub wygenerowane dane przy opcji --generate, np. "/Documents/SIPoutputs" | Brak |
| `--seed=` | nazwa folderu | ścieżka do pliku z ziarnem generatora dla --generate | Brak |
| `-g, --generate` | Brak | Program generuje pliki z danymi wejściowymi przy ustawieniu tej flagi | Brak |
| `-e, --exact` | Brak | Do rozwiązania używany jest algorytm dokładny | Brak |
| `-a, --approximate` | Brak | Do rozwiązania używany jest algorytm przybliżony | Brak |
| `-c, --console` | Brak | Rozwiązania są wypisane również na standardowe wyjście | Brak |
| `-p, --append` | Brak | Rozwiązania są dopisywane na koniec plików, jeśli te już istniały | Brak |
| `-v, --verbose` | Brak | Do rozwiązań dopisana jest informacja o czasie działania programu | Brak |
| `-l, --clean` | Brak | Folder wyjściowy --outputDir jest czyszczony, jego zawartość jest bezpowrotnie usuwana | Brak |
| `-h, -?, --help` | Brak | Program pokazuje działanie argumentów wiersza polecenia | Brak |

#### Ziarno generatora
Przy używaniu flagi --generate, program pozwala użytkownikowi na zdefiniowanie parametrów losowej generacji danych przy użyciu pliku ziarna, wskazanego przez flagę --seed. 
Plik ziarna powinien zawierać pięć liczb, oddzielonych spacją. Po kolei, liczby te reprezentują:
1. Rozmiar grafu docelowego
2. Minimalny rozmiar grafu wzorca
3. Maksymalny rozmiar grafu wzorca
4. Krok generowania rozmiaru grafu wzorca
5. Prawdopodobieństwo wystąpienia krawędzi - liczba niecałkowita w zakresie [0;1], oddzielona przecinkiem, np. "0,15"

Oprócz prawdopodobieństwa wystąpienia krawędzi, pozostałe wartości powinny być liczbami naturalnymi.
Algorytm wygeneruje dwa pliki dla każdego zestawu rozmiarów wzorca i grafu docelowego - plik, który **może** zawierać izomorfizm (ze względu na wbudowaną losowość procesu generacji), oraz plik, który **musi** zawierać izomorfizm.
Drugi z tych plików jest oznaczony ciągiem liter "_izo_" w nazwie, i dołączany jest do niego plik zakończony ciągiem "_mapping", demonstrujący poprawne mapowanie.

## Przykłady użycia

### Folder \EXAMPLES
W folderze \EXAMPLES znajduje się siedem prostych plików z danymi do sprawdzenia działania programu. Omówione są one poniżej:

#### k3_k5
Graf wzorca: graf pełny $K_3$

<img width="160" height="147" alt="graph(5)" src="https://github.com/user-attachments/assets/08d132e3-2eca-40cf-97a3-e82be11b0fac" />

Graf docelowy: graf pełny $K_5$

<img width="183" height="179" alt="graph(6)" src="https://github.com/user-attachments/assets/b105e624-8afc-4342-83ea-92756ed2f2fd" />

Oczywiście $K_5$ zawiera $\binom{5}{3}$ podgrafów będących izomorfizmami $K_3$. Przykładowe mapowanie to 0->0, 1->1, 2->2.

#### k3_z5
Graf wzorca: graf pełny $K_3$

<img width="160" height="147" alt="graph(5)" src="https://github.com/user-attachments/assets/c8c0d526-5e02-48d5-9baa-24b30b3972fb" />

Graf docelowy: graf z pięcioma izolowanymi wierzchołkami

<img width="294" height="294" alt="graph" src="https://github.com/user-attachments/assets/415b183f-de48-4c6e-ac26-c6b27504734b" />

Graf docelowy nie zawiera izomorfizmu $K_3$, ponieważ nie ma żadnych krawędzi. Łatwo wykazać, że minimalna liczba krawędzi rozszerzenia wynosi 6 (licząc oczywiście jako krawędzie skierowane). 
Po dodaniu ich może powstać np. mapowanie 0->0, 1->1, 2->2

#### c5_c10
Graf wzorca: cykl $C_5$

<img width="201" height="209" alt="graph(11)" src="https://github.com/user-attachments/assets/27576202-47af-4636-ae75-86346debc423" />

Graf docelowy: cykl $C_{10}$

<img width="254" height="253" alt="graph(2)" src="https://github.com/user-attachments/assets/3241ec47-e42c-4a4a-89b3-873ad932e582" />

Cykl $C_{10}$ nie zawiera $C_5$, natomiast wystarczy rozszerzyć go o jedną krawędź, by znaleźć izomorfizm. Najprościej dodać krawędź (4, 0), wtedy mapowanie dane jest przez 0->0, 1->1, 2->2, 3->3, 4->4

#### square_hourglass
Graf wzorca: cykl $C_4$ - kwadrat

<img width="183" height="183" alt="graph(3)" src="https://github.com/user-attachments/assets/fb4b58cc-4739-48f0-9ffa-69047f97c6ca" />

Graf docelowy: klepsydra

<img width="189" height="177" alt="graph(4)" src="https://github.com/user-attachments/assets/b4c66e56-b01d-47c9-845d-edbd777576d3" />

Graf docelowy jest izomorfizmem $C_4$, mapowanie 0->0, 1->1, 2->3, 3->2

#### k1,3_k5

Graf wzorca: $K_{1,3}$ - gwiazda

<img width="182" height="227" alt="graph(7)" src="https://github.com/user-attachments/assets/b0f54363-ace0-4709-8c08-240036127df4" />

Graf docelowy: $K_5$

<img width="183" height="179" alt="graph(6)" src="https://github.com/user-attachments/assets/53ff0372-2bdd-4903-b3cc-66d7487a7db9" />

Graf docelowy, podobnie jak z $K_3$, zawiera kilka rozwiązań problemu.

#### p3_c5

Graf wzorca: $P_3$ - ścieżka

<img width="196" height="119" alt="graph(8)" src="https://github.com/user-attachments/assets/4aea0367-9bf8-40cb-9cdc-7271a2be1613" />

Graf docelowy: $C_5$

<img width="201" height="209" alt="graph(11)" src="https://github.com/user-attachments/assets/b24e139e-0434-401f-a20d-e1dd1d34f0a6" />

Graf docelowy zawiera podgraf izomorficzny z wzorcem, przykładowe mapowanie 0->0, 1->1, 2->2

#### p4_matching

Graf wzorca: $P_4$ - ścieżka

<img width="195" height="149" alt="graph(9)" src="https://github.com/user-attachments/assets/fd0c8a15-3710-4707-91e8-4571d8b501d3" />

Graf docelowy: skojarzenie doskonałe na 4 wierzchołkach

<img width="197" height="153" alt="graph(10)" src="https://github.com/user-attachments/assets/1b876fa4-ca2a-4ef7-814f-e54a7abf06bc" />

Graf docelowy nie zawiera podgrafu izomorficznego z $P_4$. Wystarczy dodać krawędź (1,2), by docelowy stał się izomorfizmem wzorca.






