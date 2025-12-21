Instrukcja

By wywołać program, otwieramy w wierszu polecenia folder zawierający plik wykonywalny .exe. Uruchamiamy program, wpisując w wiersz polecenia nazwę pliku wykonywalnego oraz wskazanie na plik z danymi poprzedzony flagą -f. Takie wywołanie może wyglądać następująco:

.\SubgraphIsomorphism.exe -f "C:\\Users\\Shared\\Documents\\input.txt"

Program domyślnie wybiera algorytm przybliżony lub dokładny w zależności od rozmiaru większego grafu. Jeśli chcemy wymusić użycie jednego z nich, dodajemy flagę -e (dla algorytmu dokładnego) lub -a (dla algorytmu przybliżonego). Przykładowo:

.\SubgraphIsomorphism.exe -f "C:\\Users\\Shared\\Documents\\input.txt" -a
.\SubgraphIsomorphism.exe -f "C:\\Users\\Shared\\Documents\\input.txt" -e

