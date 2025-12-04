using SubgraphIsomorphism.Utils;

namespace SubgraphIsomorphism.Ullman;

public class UllmanSolver : ISubgraphIsomorphismSolver
{
    public static (Graph, bool[,]) Expand(Graph g1, Graph g2, bool[,]? matrixRecord)
    {
        matrixRecord ??= FindBestMapping(g1, g2);

        var workingMapping = (bool[,])matrixRecord.Clone();

        CompleteMapping(g1, g2, workingMapping);

        var expandedG2 = CreateExpandedGraph(g1, g2, workingMapping);

        return (expandedG2, workingMapping);
    }

    private static bool[,] FindBestMapping(Graph g1, Graph g2)
    {
        var bestMapping = new bool[g1.size, g2.size];
        var minEdgesToAdd = int.MaxValue;

        foreach (var mapping in GenerateAllMappings(g1.size, g2.size, g1, g2))
        {
            var edgesToAdd = CountEdgesToAdd(g1, g2, mapping);
            if (edgesToAdd < minEdgesToAdd)
            {
                minEdgesToAdd = edgesToAdd;
                bestMapping = mapping;
            }
        }

        return bestMapping;
    }

    private static IEnumerable<bool[,]> GenerateAllMappings(int sourceSize, int targetSize, Graph g1, Graph g2)
    {
        if (sourceSize > targetSize)
            yield break;

        var mapping = new bool[sourceSize, targetSize];

        for (var i = 0; i < sourceSize; ++i)
        {
            var bestMatch = -1;
            var bestScore = int.MaxValue;

            for (var j = 0; j < targetSize; ++j)
            {
                var alreadyMapped = false;
                for (var k = 0; k < i; ++k)
                {
                    if (mapping[k, j])
                    {
                        alreadyMapped = true;
                        break;
                    }
                }

                if (!alreadyMapped)
                {
                    var score = Math.Abs(GetOutDegree(g1, i) - GetOutDegree(g2, j)) +
                               Math.Abs(GetInDegree(g1, i) - GetInDegree(g2, j));

                    if (score < bestScore)
                    {
                        bestScore = score;
                        bestMatch = j;
                    }
                }
            }

            if (bestMatch != -1)
                mapping[i, bestMatch] = true;
        }

        yield return mapping;
    }

    private static int CountEdgesToAdd(Graph g1, Graph g2, bool[,] mapping)
    {
        var edgesToAdd = 0;

        for (var i = 0; i < g1.size; i++)
        {
            for (var j = 0; j < g1.size; j++)
            {
                if (g1.adjMatrix[i, j])
                {
                    var mappedI = GetMappedNode(mapping, i, g2.size);
                    var mappedJ = GetMappedNode(mapping, j, g2.size);

                    if (mappedI != -1 && mappedJ != -1 && !g2.adjMatrix[mappedI, mappedJ])
                        ++edgesToAdd;
                }
            }
        }

        return edgesToAdd;
    }

    private static void CompleteMapping(Graph g1, Graph g2, bool[,] mapping)
    {
        for (var i = 0; i < g1.size; ++i)
        {
            if (IsRowEmpty(mapping, i, g2.size))
            {
                for (var j = 0; j < g2.size; ++j)
                {
                    if (IsColumnEmpty(mapping, j, g1.size))
                    {
                        mapping[i, j] = true;
                        break;
                    }
                }
            }
        }
    }

    private static Graph CreateExpandedGraph(Graph g1, Graph g2, bool[,] mapping)
    {
        var expanded = new Graph(g2.size);
        for (var i = 0; i < g2.size; i++)
        {
            for (var j = 0; j < g2.size; j++)
                expanded.adjMatrix[i, j] = g2.adjMatrix[i, j];
        }

        for (var i = 0; i < g1.size; i++)
        {
            for (var j = 0; j < g1.size; j++)
            {
                if (g1.adjMatrix[i, j])
                {
                    var mappedI = GetMappedNode(mapping, i, g2.size);
                    var mappedJ = GetMappedNode(mapping, j, g2.size);

                    if (mappedI != -1 && mappedJ != -1 && !expanded.adjMatrix[mappedI, mappedJ])
                    {
                        expanded.adjMatrix[mappedI, mappedJ] = true; 
                        expanded.edgesWereAdded = true;
                        expanded.isNewEdge[mappedI, mappedJ] = true;
                    }
                }
            }
        }

        return expanded;
    }

    private static int GetOutDegree(Graph graph, int node)
    {
        var degree = 0;
        for (var i = 0; i < graph.size; ++i)
        {
            if (graph.adjMatrix[node, i])
                ++degree;
        }
        return degree;
    }

    private static int GetInDegree(Graph graph, int node)
    {
        var degree = 0;
        for (var i = 0; i < graph.size; i++)
        {
            if (graph.adjMatrix[i, node])
                ++degree;
        }
        return degree;
    }

    public static int GetMappedNode(bool[,] mapping, int g1Node, int g2Size)
    {
        for (var j = 0; j < g2Size; ++j)
        {
            if (mapping[g1Node, j])
                return j;
        }
        return -1;
    }

    private static bool IsRowEmpty(bool[,] matrixRecord, int row, int colCount)
    {
        for (var j = 0; j < colCount; ++j)
        {
            if (matrixRecord[row, j])
                return false;
        }
        return true;
    }

    private static bool IsColumnEmpty(bool[,] matrixRecord, int col, int rowCount)
    {
        for (var i = 0; i < rowCount; ++i)
        {
            if (matrixRecord[i, col])
                return false;
        }
        return true;
    }

    public Results Solve(SubgraphIsomorphism.Utils.Graph g1, SubgraphIsomorphism.Utils.Graph g2)
    {
        var ullman = new Ullman(g1, g2);
        var (exact, mapping, cost, edges) = ullman.FindIsomorphismOrExtension();

        if (exact)
            return new Results(mapping!, cost, exact, null);
        else
        {
            //var (expandedG2, workingMapping) = Expand(g1, g2, matrix);
            //var missingEdges = CountEdgesToAdd(g1, expandedG2, workingMapping);
            Graph supergraph = new Graph(g2.size);
            for (var i = 0; i < g2.size; ++i)
            {
                for (var j = 0; j < g2.size; ++j)
                {
                    supergraph.adjMatrix[i, j] = g2.adjMatrix[i, j];
             
                }
            }
            foreach(var edge in edges)
            {
                supergraph.adjMatrix[edge.Item1, edge.Item2] = true;
                supergraph.edgesWereAdded = true;
                supergraph.isNewEdge[edge.Item1, edge.Item2] = true;
            }
            return new Results(mapping!, cost, exact, supergraph);
        }
    }

    public string Name() => "Algorytm Dokładny Ullmana";
}