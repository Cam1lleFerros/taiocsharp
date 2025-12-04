using SubgraphIsomorphism.Utils;

namespace SubgraphIsomorphism.Munkres;

public class MunkresSolver : ISubgraphIsomorphismSolver
{
    public record MappingResult(int[] Mapping, double EditDistance, bool IsExact, Graph Supergraph);

    public static double[,] BuildCostMatrix(Graph pattern, Graph target)
    {
        var n = pattern.size;
        var m = target.size;
        var totalSize = n + m;
        var costMatrix = new double[totalSize, totalSize];

        for (var i = 0; i < totalSize; ++i)
            for (var j = 0; j < totalSize; ++j)
                costMatrix[i, j] = 1000;

        for (var i = 0; i < n; ++i)
        {
            for (var j = 0; j < m; ++j)
                costMatrix[i, j] = NodeCompatibilityCost(pattern, target, i, j);
        }

        for (var i = 0; i < n; ++i)
            costMatrix[i, m + i] = NodeDeletionCost(pattern, i);

        for (var j = 0; j < m; ++j)
            costMatrix[n + j, j] = NodeInsertionCost(target, j);

        for (var i = 0; i < m; ++i)
        {
            for (var j = 0; j < n; ++j)
                costMatrix[n + i, m + j] = 0;
        }

        return costMatrix;
    }

    private static double NodeCompatibilityCost(Graph pattern, Graph target, int patternNode, int targetNode)
    {
        double cost = 0;

        var patternDegree = pattern.OutDegree(patternNode);
        var targetDegree = target.OutDegree(targetNode);

        if (patternDegree > targetDegree)
            cost += 1000;

        cost += NeighborhoodCompatibilityCost(pattern, target, patternNode, targetNode);

        return cost;
    }

    private static double NeighborhoodCompatibilityCost(Graph pattern, Graph target, int pNode, int tNode)
    {
        double cost = 0;
        var pNeighbors = GetNeighbors(pattern, pNode);
        var tNeighbors = GetNeighbors(target, tNode);

        foreach (var pNeighbor in pNeighbors)
        {
            var foundCompatible = false;
            foreach (var tNeighbor in tNeighbors)
            {
                if (AreNodesCompatible(pattern, target, pNeighbor, tNeighbor))
                {
                    foundCompatible = true;
                    break;
                }
            }
            if (!foundCompatible)
                cost += 10;
        }

        return cost;
    }

    private static bool AreNodesCompatible(Graph pattern, Graph target, int pNode, int tNode)
    {
        if (pattern.OutDegree(pNode) > target.OutDegree(tNode))
            return false;

        return true;
    }

    private static List<int> GetNeighbors(Graph g, int node)
    {
        var neighbors = new List<int>();
        for (var i = 0; i < g.size; i++)
        {
            if (g.adjMatrix[node, i] || (i != node && g.adjMatrix[i, node]))
                neighbors.Add(i);
        }
        return neighbors;
    }

    private static double NodeDeletionCost(Graph pattern, int node) => 1 + pattern.OutDegree(node);

    private static double NodeInsertionCost(Graph target, int node) => 1 + target.OutDegree(node);

    public static MappingResult ComputeEditDistance(Graph pattern, Graph target)
    {
        try
        {
            var costMatrix = BuildCostMatrix(pattern, target);
            var (assignment, totalCost) = Munkres.Solve(costMatrix);

            var mapping = ExtractNodeMapping(assignment, pattern.size, target.size);
            var supergraph = BuildSupergraph(pattern, target, mapping);

            var isExact = IsExactSubgraphIsomorphism(pattern, target, mapping);

            return new MappingResult(mapping, totalCost, isExact, supergraph);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in Munkres algorithm: {ex.Message}");
            return new MappingResult(new int[pattern.size], double.MaxValue, false, target);
        }
    }

    private static int[] ExtractNodeMapping(bool[,] assignment, int n, int m)
    {
        var mapping = new int[n];
        Array.Fill(mapping, -1);

        for (var i = 0; i < n; ++i)
        {
            for (var j = 0; j < m; ++j)
            {
                if (assignment[i, j])
                {
                    mapping[i] = j;
                    break;
                }
            }
        }
        return mapping;
    }

    private static Graph BuildSupergraph(Graph pattern, Graph target, int[] mapping)
    {
        var supergraph = new Graph(target.size);

        for (var i = 0; i < target.size; ++i)
            for (var j = 0; j < target.size; ++j)
                supergraph.adjMatrix[i, j] = target.adjMatrix[i, j];

        for (var u = 0; u < pattern.size; ++u)
        {
            var mu = mapping[u];
            if (mu < 0) continue;

            for (int v = 0; v < pattern.size; ++v)
            {
                if (pattern.adjMatrix[u, v])
                {
                    var mv = mapping[v];
                    if (mv >= 0 && !supergraph.adjMatrix[mu, mv])
                    {
                        supergraph.adjMatrix[mu, mv] = true;
                    }
                }
            }
        }

        return supergraph;
    }

    private static bool IsExactSubgraphIsomorphism(Graph pattern, Graph target, int[] mapping)
    {
        for (var i = 0; i < mapping.Length; ++i)
        {
            if (mapping[i] < 0 || mapping[i] >= target.size)
                return false;
        }

        for (var u = 0; u < pattern.size; ++u)
        {
            for (var v = 0; v < pattern.size; ++v)
            {
                if (pattern.adjMatrix[u, v])
                {
                    var mu = mapping[u];
                    var mv = mapping[v];
                    if (mu < 0 || mv < 0 || !target.adjMatrix[mu, mv])
                        return false;
                }
            }
        }

        return true;
    }

    public Results Solve(Graph g1, Graph g2)
    {
        var res = ComputeEditDistance(g1, g2);

        var missingEdges = CalculateMissingEdges(g1, g2, res.Mapping);

        return new Results(res.Mapping, missingEdges, res.IsExact, res.Supergraph, g2.size);
    }

    private static int CalculateMissingEdges(Graph pattern, Graph target, int[] mapping)
    {
        var missing = 0;

        for (var u = 0; u < pattern.size; ++u)
        {
            for (var v = 0; v < pattern.size; ++v)
            {
                if (pattern.adjMatrix[u, v])
                {
                    var mu = mapping[u];
                    var mv = mapping[v];
                    if (mu < 0 || mv < 0 || !target.adjMatrix[mu, mv])
                        missing++;
                }
            }
        }

        return missing;
    }

    public string Name() => "Munkres Approximation Solver";
}