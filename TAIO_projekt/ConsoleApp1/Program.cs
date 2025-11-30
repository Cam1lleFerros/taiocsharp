using SubgraphIsomorphism.Munkres;
using SubgraphIsomorphism;

internal class Program
{
    private static void Main(string[] args)
    {
        SIOptions options = new(args);
        var (g1, g2) = Utils.PrepGraphs(options.inPath);


        using var writer = File.CreateText(options.outPath);

        if (options.exact)
        {
            var ullman = new Ullman(g1, g2);
            var (result, matrix) = ullman.FindIsomorphism();
            if (result)
            {
                writer.WriteLine("Graf mniejszy jest podgrafem większego. Dokładne mapowanie:");
                Utils.PrintMatrix(matrix!, writer);
                if (options.console)
                {
                    Console.WriteLine("Graf mniejszy jest podgrafem większego. Dokładne mapowanie:");
                    Utils.PrintMatrix(matrix!);
                }
            }
            else
            {
                // Wywował metodę extend z Ullmana
                // Wydrukuj nowy graf
                var (expandedG2, workingMapping) = UllmanExpand.Expand(g1, g2, matrix); 
                writer.WriteLine("Graf mniejszy nie jest podgrafem większego.");
                   
                writer.WriteLine("Graf po dodaniu brakujących krawędzi:");
                for (var r = 0; r < expandedG2.size; ++r)
                {
                    for (var c = 0; c < expandedG2.size; ++c)
                    {
                        writer.Write(expandedG2.HasEdge(r, c) ? "1 " : "0 ");
                    }
                    writer.WriteLine();
                }
                writer.WriteLine("Przybliżone mapowanie:");
                Utils.PrintMatrix(workingMapping!, writer);
                if (options.console)
                {
                    Console.WriteLine("Graf mniejszy nie jest podgrafem większego.");
                    Console.WriteLine("Graf po dodaniu brakujących krawędzi:");
                    for (var r = 0; r < expandedG2.size; ++r)
                    {
                        for (var c = 0; c < expandedG2.size; ++c)
                        {
                            Console.Write(expandedG2.HasEdge(r, c) ? "1 " : "0 ");
                        }
                        Console.WriteLine();
                    }
                    Console.WriteLine("Przybliżone mapowanie:");
                    Utils.PrintMatrix(workingMapping!);
                }

            }
        }
        if (options.approximate)
        {
            var res = MunkresUsage.FindMappingAndMissingEdges(g1, g2);
            if (res.IsExact)
            {
                writer.WriteLine("Graf mniejszy jest podgrafem większego. Dokładne mapowanie:");
                if (options.console)
                {
                    Console.WriteLine("Graf mniejszy jest podgrafem większego. Dokładne mapowanie:");
                }
            }
            else
            {
                writer.WriteLine($"Graf mniejszy nie jest podgrafem większego. Liczba brakujących krawędzi do dodania: {res.MissingEdges}. Przybliżone mapowanie:");
                if (options.console)
                {
                    Console.WriteLine($"Graf mniejszy nie jest podgrafem większego. Liczba brakujących krawędzi do dodania: {res.MissingEdges}. Przybliżone mapowanie:");
                }
            }
            var rows = g1.size;
            var cols = g2.size;
            var matrix = new bool[rows, cols];
            for (var i = 0; i < rows; ++i)
            {
                var j = res.Mapping[i];
                matrix[i, j] = true;
            }
            Utils.PrintMatrix(matrix, writer);
            if (options.console)
            {
                Utils.PrintMatrix(matrix);
            }
            if (!res.IsExact)
            {

                writer.WriteLine("Graf po dodaniu brakujących krawędzi:");
                for (var r = 0; r < res.Supergraph.size; ++r)
                {
                    for (var c = 0; c < res.Supergraph.size; ++c)
                    {
                        writer.Write(res.Supergraph.HasEdge(r, c) ? "1 " : "0 ");
                    }
                    writer.WriteLine();
                }
                if (options.console)
                {
                    Console.WriteLine("Graf po dodaniu brakujących krawędzi:");
                    Utils.PrintMatrix(matrix);
                }
            }
        }
    }
}