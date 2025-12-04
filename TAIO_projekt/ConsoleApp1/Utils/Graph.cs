namespace SubgraphIsomorphism.Utils;

public class Graph
{
    public int size;
    public bool[,] adjMatrix;
    public bool edgesWereAdded = false;
    public bool[,] isNewEdge;

    public Graph(int n)
    {
        size = n;
        adjMatrix = new bool[n, n];
        isNewEdge = new bool[n, n];
    }

    internal static (Graph, Graph) ReadTwoFromFile(string path)
    {
        var rawLines = File.ReadAllLines(path).Select(l => l.Trim()).ToArray();
        var index = 0;

        var n1 = int.Parse(rawLines[index++]);
        var g1 = new Graph(n1);
        for (var r = 0; r < n1; ++r)
        {
            var tokens = rawLines[index++].Split(null as string[], StringSplitOptions.RemoveEmptyEntries);
            for (var c = 0; c < n1; ++c)
                g1.adjMatrix[r, c] = ParseBooleanToken(tokens[c]);
        }

        var n2 = int.Parse(rawLines[index++]);
        var g2 = new Graph(n2);
        for (var r = 0; r < n2; ++r)
        {
            var tokens = rawLines[index++].Split(null as string[], StringSplitOptions.RemoveEmptyEntries);
            for (var c = 0; c < n2; ++c)
                g2.adjMatrix[r, c] = ParseBooleanToken(tokens[c]);
        }

        return (g1, g2);

        static bool ParseBooleanToken(string token) => token != "0";
    }

    public bool HasEdge(int a, int b)
    {
        if (a < 0 || b < 0 || a >= size || b >= size)
            throw new IndexOutOfRangeException($"Vertex indexes must be in range [0;n). Arguments given: {a}, {b}");

        return adjMatrix[a, b];
    }

    public IEnumerable<int> OutNeighbours(int a)
    {
        for (var i = 0; i < size; ++i)
        {
            if (i != a && adjMatrix[a, i])
                yield return i;
        }
    }

    public int OutDegree(int a)
    {
        var degree = 0;
        for (var i = 0; i < size; ++i)
        {
            if (i != a && adjMatrix[a, i])
                ++degree;
        }
        return degree;
    }

    public int EdgeCount()
    {
        var sum = 0;
        for (var i = 0; i < size; ++i)
        {
            for (var j = 0; j < size; ++j)
                if (adjMatrix[i, j]) ++sum;
        }
        return sum;
    }

    public Graph Clone()
    {
        var g = new Graph(size);
        for (var i = 0; i < size; ++i)
        {
            for (var j = 0; j < size; ++j)
                g.adjMatrix[i, j] = adjMatrix[i, j];
        }
        return g;
    }
}
