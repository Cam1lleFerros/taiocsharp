using SubgraphIsomorphism.Munkres;
using SubgraphIsomorphism.Ullman;
using System.Diagnostics;

namespace SubgraphIsomorphism.Utils;

public interface ISubgraphIsomorphismSolver
{
    Results Solve(Graph g1, Graph g2);
    string Name();
}
public class SolverManager
{
    private readonly List<ISubgraphIsomorphismSolver> solvers = [];
    private readonly SIOptions options;
    private readonly Graph g1;
    private readonly Graph g2;
    private readonly bool batchSolving = false;

    private static int ChooseAlgorithm(int n1, int n2, int m1, int m2)
    {
        if (n1 <= 10 && n2 <= 100)
            return 0; // Ullman
        else
            return 1; // Munkres
    }

    public void RegisterSolver(ISubgraphIsomorphismSolver solver) =>  solvers.Add(solver);

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public SolverManager(string[] args)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
        SIOptions options = new(args);

        if (options.inDirectory == string.Empty)
            (g1, g2) = PrintGraphUtils.PrepGraphs(options.inPath);
        else
        {
            batchSolving = true;
            if (options.outDirectory == string.Empty)
                throw new ArgumentException(PrintGraphUtils.OutputDirErrorMessage);
            if (!Directory.Exists(options.outDirectory))
                Directory.CreateDirectory(options.outDirectory);
        }


        if (options.exact)
            RegisterSolver(new UllmanSolver());
        if (options.approximate)
            RegisterSolver(new MunkresSolver());
        if (options.dynamic)
        {
            RegisterSolver(new UllmanSolver());
            RegisterSolver(new MunkresSolver());
            Console.WriteLine($"Nie został jawnie wybrany algorytm - algorytm wybrany zostanie dynamicznie.");
        }
            
        this.options = options;
    }

    public void SolveAll()
    {
        if (options.dynamic)
        {
            var algIdx = ChooseAlgorithm(g1.size, g2.size, g1.EdgeCount(), g2.EdgeCount());
            Solve(g1, g2, algIdx);
            return;
        }
        for (var i = 0; i < solvers.Count; ++i)
            Solve(g1, g2, i);
    }

    public void SolveAllBatch()
    {
        var inputFiles = Directory.GetFiles(options.inDirectory, "*.txt");
        inputFiles = [.. from f in inputFiles
                         where !f.EndsWith("_mapping.txt")
                         select f];
        var inputCount = inputFiles.Length;
        var processedCount = 0;
        var logPath = Path.Combine(options.outDirectory, "batch_log.txt");
        using var logWriter = new StreamWriter(logPath, append: true);
        var timer = new Stopwatch();
        foreach (var inputFile in inputFiles)
        {
            timer.Restart();
            var (g1, g2) = PrintGraphUtils.PrepGraphs(inputFile);
            var outputFileName = Path.GetFileName(inputFile);
            outputFileName = outputFileName.Replace(".txt", "_out.txt");
            var outPath = Path.Combine(options.outDirectory, outputFileName);
            if (options.dynamic)
            {
                var algIdx = ChooseAlgorithm(g1.size, g2.size, g1.EdgeCount(), g2.EdgeCount());
                PrintGraphUtils.PrintMessageOptions($"Dla pliku {inputFile} wybrano algorytm {(algIdx == 0 ? "Ullmana" : "Munkresa")}.", logWriter, options);
                Solve(g1, g2, algIdx, outPath, logWriter);
            }
            else
            {
                for (var i = 0; i < solvers.Count; ++i)
                    Solve(g1, g2, i, outPath, logWriter);
            }
            ++processedCount;
            timer.Stop();
            var infoString = $"Przetworzono {processedCount} z {inputCount} plików.";
            Console.Write(infoString);

            Console.Write(ConsoleUtils.PrepareDeletionString(infoString.Length));
        }
    }

    public void SolveAllBatchParallel()
    {
        var inputFiles = Directory.GetFiles(options.inDirectory, "*.txt");
        Parallel.ForEach(inputFiles, (inputFile) =>
        {
            var (g1, g2) = PrintGraphUtils.PrepGraphs(inputFile);
            var outputFileName = Path.GetFileName(inputFile);
            outputFileName = outputFileName.Replace(".txt", "_out.txt");
            var outPath = Path.Combine(options.outDirectory, outputFileName);
            for (var i = 0; i < solvers.Count; ++i)
                Solve(g1, g2, i);
        });
    }

    public void SolveOptions()
    {
        if (batchSolving)
            SolveAllBatch();
        else
            SolveAll();
    }

    public void Solve(Graph g1, Graph g2, int idx = 0, string outPath = "", StreamWriter? logWriter = null)
    {
        if (String.IsNullOrEmpty(outPath))
            outPath = options.outPath;

        if (idx >= solvers.Count)
            return;

        var solver = solvers[idx];
        var timer = new Stopwatch();
        timer.Start();
        var results = solver.Solve(g1, g2);
        timer.Stop();
        outPath = outPath.Replace(".txt", $"_{solver.Name()}.txt");
        using var writer = new StreamWriter(outPath, append: options.append);
        PrintGraphUtils.PrintMessageOptions($"Grafy dane na wejściu:", writer, options);
        PrintGraphUtils.PrintMessageOptions($"Graf wzoru (rozmiar {g1.size}):", writer, options);
        PrintGraphUtils.PrintMatrixOptions(g1.adjMatrix, writer, options);
        PrintGraphUtils.PrintMessageOptions($"Graf docelowy (rozmiar {g2.size}):", writer, options);
        PrintGraphUtils.PrintMatrixOptions(g2.adjMatrix, writer, options);


        logWriter?.WriteLine($"{solver.Name()}; {g1.size}; {g2.size}; {timer.ElapsedMilliseconds}");
        PrintGraphUtils.PrintMessageOptions($"--- Rezultaty dla algorytmu : {solver.Name()} ---", writer, options);
        if (options.verbose)
            PrintGraphUtils.PrintMessageOptions($"Czas działania: {timer.ElapsedMilliseconds} ms", writer, options);
        if (results.IsExact)
        {
            PrintGraphUtils.PrintMessageOptions(PrintGraphUtils.ExactMatchMessage, writer, options);
            PrintGraphUtils.PrintMessageOptions(PrintGraphUtils.ExactMappingDisplayMessage, writer, options);
            PrintGraphUtils.PrintMappingOptions(results.Mapping, writer, options);
        }
        else
        {
            PrintGraphUtils.PrintMessageOptions(PrintGraphUtils.ExactMatchFailureMessage, writer, options);
            if (results.MissingEdges.HasValue)
                PrintGraphUtils.PrintMessageOptions($"Brakuje {results.MissingEdges.Value} krawędzi do poprawnego mapowania.", writer, options);
            if (results.GraphComplement != null)
            {
                PrintGraphUtils.PrintMessageOptions(PrintGraphUtils.GraphComplementDisplayMessage, writer, options);
                PrintGraphUtils.PrintExtendedGraphOptions(results.GraphComplement, writer, options);
            }
            PrintGraphUtils.PrintMessageOptions(PrintGraphUtils.ApproximatedMappingDisplayMessage, writer, options);
            PrintGraphUtils.PrintMappingOptions(results.Mapping, writer, options);
        }
    }
}
