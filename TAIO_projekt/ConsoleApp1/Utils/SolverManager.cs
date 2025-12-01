using SubgraphIsomorphism.Munkres;
using SubgraphIsomorphism.Ullman;

namespace SubgraphIsomorphism.Utils
{
    public interface ISubgraphIsomorphismSolver
    {
        Results Solve(Graph g1, Graph g2);
        string Name();
    }
    public class SolverManager
    {
        private readonly List<ISubgraphIsomorphismSolver> solvers = [];
        private SIOptions options;
        private Graph g1;
        private Graph g2;
        private readonly bool batchSolving = false;

        public void RegisterSolver(ISubgraphIsomorphismSolver solver)
        {
            solvers.Add(solver);
        }

        public SolverManager(string[] args)
        {
            SIOptions options = new(args);

            if (options.inDirectory == string.Empty)
            {
                (g1, g2) = PrintGraphUtils.PrepGraphs(options.inPath);
            }
            else
            {
                batchSolving = true;
                if (options.outDirectory == string.Empty)
                {
                    throw new ArgumentException("When using batch solving, output directory must be specified with --outputDir option.");
                }
                if (!Directory.Exists(options.outDirectory))
                {
                    Directory.CreateDirectory(options.outDirectory);
                }
            }


            if (options.exact)
            {
                RegisterSolver(new UllmanSolver());
            }
            if (options.approximate)
            {
                RegisterSolver(new MunkresSolver());
            }

            this.options = options;
        }

        public void SolveAll()
        {
            for (var i = 0; i < solvers.Count; ++i)
            {
                Solve(g1, g2, i);
            }
        }

        public void SolveAllBatch()
        {
            var inputFiles = Directory.GetFiles(options.inDirectory, "*.txt");
            foreach (var inputFile in inputFiles)
            {
                if(inputFile.EndsWith("_mapping.txt"))
                    continue;
                var (g1, g2) = PrintGraphUtils.PrepGraphs(inputFile);
                var outputFileName = Path.GetFileName(inputFile);
                outputFileName = outputFileName.Replace(".txt", "_out.txt");
                var outPath = Path.Combine(options.outDirectory, outputFileName);
                for (var i = 0; i < solvers.Count; ++i)
                {
                    Solve(g1, g2, i, outPath);
                }
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
                {
                    Solve(g1, g2, i);
                }
            });
        }

        public void SolveOptions()
        {
            if (batchSolving)
            {
                SolveAllBatch();
            }
            else
            {
                SolveAll();
            }
        }

        public void Solve(Graph g1, Graph g2, int idx = 0, string outPath = "")
        {
            if(outPath == "")
            {
                outPath = options.outPath;
            }

            if(idx >= solvers.Count)
                return;

            var solver = solvers[idx];
            var results = solver.Solve(g1, g2);
            outPath = outPath.Replace(".txt", $"_{solver.Name()}.txt");
            using var writer = new StreamWriter(outPath, append: options.append);
            PrintGraphUtils.PrintMessageOptions($"--- Results for {solver.Name()} solver ---", writer, options);
            if (results.IsExact)
            {
                PrintGraphUtils.PrintMessageOptions(PrintGraphUtils.ExactMatchMessage, writer, options);
                PrintGraphUtils.PrintMessageOptions(PrintGraphUtils.ExactMappingDisplayMessage, writer, options);
                PrintGraphUtils.PrintMatrixOptions(results.Mapping, writer, options);
            }
            else
            {
                PrintGraphUtils.PrintMessageOptions(PrintGraphUtils.ExactMatchFailureMessage, writer, options);
                if (results.MissingEdges.HasValue)
                {
                    PrintGraphUtils.PrintMessageOptions($"{results.MissingEdges.Value} edges missing for an exact mapping.", writer, options);
                }
                if (results.GraphComplement != null)
                {
                    PrintGraphUtils.PrintMessageOptions(PrintGraphUtils.GraphComplementDisplayMessage, writer, options);
                    PrintGraphUtils.PrintMatrix(results.GraphComplement.adjMatrix, writer);
                    if (options.console)
                    {
                        PrintGraphUtils.PrintMatrix(results.GraphComplement.adjMatrix);
                    }
                }
                PrintGraphUtils.PrintMessageOptions(PrintGraphUtils.ApproximatedMappingDisplayMessage, writer, options);
                PrintGraphUtils.PrintMatrixOptions(results.Mapping, writer, options);
            }
        }
    }
}
