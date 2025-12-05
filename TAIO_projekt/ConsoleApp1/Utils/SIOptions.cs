using NDesk.Options;

namespace SubgraphIsomorphism.Utils;

public class SIOptions
{
    public string inPath = "input.txt";
    public string outPath = "output.txt";
    public string inDirectory = String.Empty;
    public string outDirectory = String.Empty;
    public string seedPath = String.Empty;
    public bool help = false;
    public bool exact = false;
    public bool approximate = false;
    public bool console = false;
    public bool append = false;
    public bool generate = false;
    public bool verbose = false;
    public bool clean = false;
    public bool dynamic = false;

    public SIOptions(string[] args)
    {
        var opts = new OptionSet() {
        { "input=", "ścieżka pliku z danymi (domyślnie: input.txt)", v => inPath = v },
        { "output=", "ścieżka do pliku z odpowiedzią, który zostanie utworzony lub zaktualizowany (domyślnie: output.txt)", v => {outPath = v; } },
        { "inputDir=", "ścieżka do folderu z plikami z danymi dla rozwiązywania masowego", v => inDirectory = v },
        { "outputDir=", "ścieżka do folderu z plikami odpowiedzi dla rozwiązywania masowego", v => outDirectory = v },
        { "seed=", "ścieżka do pliku generacji danych. Szczegóły w dokumentacji.", v => seedPath = v },
        { "g|generate", "zamiast rozwiązywać problemy, program generuje losowe dane w folderze sprecyzowanym przez --outputDir.", v =>  {generate = true; }},
        { "e|exact", "program użyje algorytmu dokładnego (Ullmana)", v => { exact = true; } },
        { "a|approximate", "program użyje algorytmu przybliżonego (Munkresa/węgierskiego)" ,v => { approximate = true; } },
        { "c|console", "program wypisze odpowiedzi na standardowe wyjście", v => console = true},
        { "p|append", "program dopisze odpowiedzi w plikach wyjściowych, jeśli już istnieją, zamiast je nadpisywać", v => append = true },
        { "v|verbose", "program zapisze czas rozwiązywania do plików wyjściowych", v => verbose = true },
        { "l|clean", "program wyczyści folder --outputDir przed tworzeniem w nim nowych plików", v => clean = true },
        { "h|?|help", "pomoc",  v => help = v != null },

    };
        List<string> extra = opts.Parse(args);

        if (help || args.Length == 0)
        {
            ShowHelp(opts);
            Environment.Exit(0);
        }

        if (generate)
        {
            if (exact || approximate || !String.IsNullOrEmpty(inDirectory))
                throw new ArgumentException("Opcja --generate nie może być użyta z opcjami --exact, --approximate lub --inputDir");

            if (String.IsNullOrEmpty(outDirectory))
                throw new ArgumentException("Opcja --generate potrzebuje, by był sprecyzowany folder wyjściowy przy użyciu --outputDir");

            if (clean && Directory.Exists(outDirectory))
            {
                Directory.Delete(outDirectory, true);
                Directory.CreateDirectory(outDirectory);
            }

            if (!String.IsNullOrEmpty(seedPath))
            {
                var seedContent = File.ReadAllText(seedPath).Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (seedContent.Length != 5)
                    throw new ArgumentException("Niepoprawne ziarno generatora.");
                var targetSize = int.Parse(seedContent[0]);
                var patternSizeMin = int.Parse(seedContent[1]);
                var patternSizeMax = int.Parse(seedContent[2]);
                var patternStep = int.Parse(seedContent[3]);
                var edgeProbability = double.Parse(seedContent[4]);
                InputGenerator.InputGenerator.GenerateInputsForSize(outDirectory, targetSize, patternSizeMin,
                    patternSizeMax, patternStep, edgeProbability);
            }
            else
                InputGenerator.InputGenerator.GenerateInputsForSize(outDirectory, 10, 1, 9, 1);
            Console.WriteLine("Generowanie danych zakończone pomyślnie.");
            Environment.Exit(0);
        }

        if (!exact && !approximate)
        {
            dynamic = true;
        }
    }

    static void ShowHelp(OptionSet p)
    {
        Console.WriteLine("Sposób używania programu:");
        Console.WriteLine("Program rozwiązuje problem izomorfizmu podgrafów przy użyciu algorytmów Ullmana lub Munkresa.");
        Console.WriteLine();
        Console.WriteLine("Program akceptuje poniższe opcje:");
        p.WriteOptionDescriptions(Console.Out);
    }
}
