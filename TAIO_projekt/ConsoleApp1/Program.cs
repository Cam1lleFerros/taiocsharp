using NDesk.Options;
using SubgraphIsomorphism.Munkres;
using SubgraphIsomorphism;
using SubgraphIsomorphism.Ullman;

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
                // Jakoś wydrukuj tą macierz
            }
            else
            {
                // Wywował metodę extend z Ullmana
                // Wydrukuj nowy graf
            }
        }
        if (options.approximate)
        {
            var res = MunkresUsage.FindMappingAndMissingEdges(g1, g2);
            if (res.IsExact)
            {
                writer.WriteLine("Graf mniejszy jest podgrafem większego. Dokładne mapowanie:");
            }
            else
            {
                writer.WriteLine($"Graf mniejszy nie jest podgrafem większego. Liczba brakujących krawędzi do dodania: {res.MissingEdges}. Przybliżone mapowanie:");
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
            }
        }
    }
}