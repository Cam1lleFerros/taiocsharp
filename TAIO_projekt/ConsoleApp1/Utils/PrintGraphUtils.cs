namespace SubgraphIsomorphism.Utils;

public static class PrintGraphUtils
{
    public const string ExactMatchMessage = "Cel zawiera podgraf izomorficzny z wzorem.";
    public const string ExactMappingDisplayMessage = "Dokładne mapowanie:";
    public const string ExactMatchFailureMessage = "Cel nie zawiera podgrafu izomorficznego z wzorem.";
    public const string GraphComplementDisplayMessage = "Graf docelowy z uzupełnionymi krawędziami (zaznaczonymi jako *):";
    public const string ApproximatedMappingDisplayMessage = "Przybliżone mapowanie:";
    public const string OutputDirErrorMessage = "Przy rozwiązywaniu masowym, należy sprecyzować folder wyjściowy przy użyciu opcji --outputDir.";
    public static void PrintMatrix(bool[,] matrix, TextWriter writer)
    {
        var rows = matrix.GetLength(0);
        var cols = matrix.GetLength(1);
        for (var i = 0; i < rows; ++i)
        {
            for (var j = 0; j < cols; ++j)
                writer.Write(matrix[i, j] ? "1 " : "0 ");
            writer.WriteLine();
        }
    }


    public static void StandardizePrintMatrixOptions(int[] matrix, int g2size, TextWriter writer, SIOptions options)
    {
        PrintMatrixOptions(StandardiseMapping(matrix, g2size), writer, options);
    }

    public static bool[,] StandardiseMapping(int[] mapping, int g2size)
    {
        var standardisedMapping = new bool[mapping.Length, g2size];
        for (var i = 0; i < mapping.Length; ++i)
        {
            for (var j = 0; j < g2size; ++j)
                standardisedMapping[i, j] = mapping[i] == j;
        }
        return standardisedMapping;
    }

    public static void PrintMatrix(bool[,] matrix)
    {
        var rows = matrix.GetLength(0);
        var cols = matrix.GetLength(1);
        for (var i = 0; i < rows; ++i)
        {
            for (var j = 0; j < cols; ++j)
                Console.Write(matrix[i, j] ? "1 " : "0 ");
            Console.WriteLine();
        }
    }

    public static void PrintMatrixOptions(bool[,] matrix, TextWriter writer, SIOptions options)
    {
        PrintMatrix(matrix, writer);
        if (options.console)
            PrintMatrix(matrix);
    }

    public static void PrintExtendedGraphOptions(Graph graph, TextWriter writer, SIOptions options)
    {
        if(!graph.edgesWereAdded)
        {
            PrintMatrixOptions(graph.adjMatrix, writer, options);
            return;
        }

        var rows = graph.adjMatrix.GetLength(0);
        var cols = graph.adjMatrix.GetLength(1);
        for (var i = 0; i < rows; ++i)
        {
            for (var j = 0; j < cols; ++j)
            {
                var symbol = graph.adjMatrix[i, j] ? (graph.isNewEdge[i, j] ? "* " : "1 ") : "0 ";
                writer.Write(symbol);
                if (options.console)
                    Console.Write(symbol);
            }
            writer.WriteLine();
            if (options.console)
                Console.WriteLine();
        }
    }

    public static void PrintMappingOptions(bool[,] matrix, TextWriter writer, SIOptions options)
    {
        var rows = matrix.GetLength(0);
        var cols = matrix.GetLength(1);
        for (var i = 0; i < rows; ++i)
        {
            for (var j = 0; j < cols; ++j)
            {
                if (matrix[i, j])
                {
                    writer.WriteLine($"{i} -> {j}");
                    if (options.console)
                    {
                        Console.WriteLine($"{i} -> {j}");
                    }
                }
            }

        }
    }


    public static void PrintMessageOptions(string message, TextWriter writer, SIOptions options)
    {
        writer.WriteLine(message);
        if (options.console)
            Console.WriteLine(message);
    }

    public static (Graph p, Graph g) PrepGraphs(string inPath)
    {
        var (p, g) = Graph.ReadTwoFromFile(inPath);

        Graph g1, g2;
        if (p.size > g.size)
        {
            g2 = p;
            g1 = g;
        }
        else if (p.size == g.size)
        {
            if (p.EdgeCount() >= g.EdgeCount())
            {
                g2 = p;
                g1 = g;
            }
            else
            {
                g2 = g;
                g1 = p;
            }
        }
        else
        {
            g2 = g;
            g1 = p;
        }
        return (g1, g2);
    }
}
