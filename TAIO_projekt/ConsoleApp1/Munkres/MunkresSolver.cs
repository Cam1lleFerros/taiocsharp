using SubgraphIsomorphism.Utils;

namespace SubgraphIsomorphism.Munkres;

public class MunkresSolver : ISubgraphIsomorphismSolver
{
    public record MappingResult(int[] Mapping, double EditDistance, bool IsExact, Graph Supergraph);

    private readonly double nodeDeleteCostBase; 
    private readonly double edgeEditCost;

    public MunkresSolver(double nodeDeleteCostBase = 1.0, double edgeEditCost = 1.0)
    {
        this.nodeDeleteCostBase = nodeDeleteCostBase;
        this.edgeEditCost = edgeEditCost;
    }

    public double[,] BuildCostMatrix(Graph pattern, Graph target)
    {
        var n = pattern.size;
        var m = target.size;
        var totalSize = n + m;
        var C = new double[totalSize, totalSize];

        var large = 1e6;
        for (var i = 0; i < totalSize; ++i)
        {
            for (var j = 0; j < totalSize; ++j)
                C[i, j] = large;
        }

        for (var i = 0; i < n; ++i)
        {
            for (var j = 0; j < m; ++j)
                C[i, j] = NodeSubstitutionCost(pattern, target, i, j);
        }

        for (var i = 0; i < n; ++i)
            C[i, m + i] = NodeDeletionCost(pattern, i);

        for (var j = 0; j < m; ++j)
            C[n + j, j] = NodeInsertionCost(target, j);

        for (var i = 0; i < m; ++i)
        {
            for (var j = 0; j < n; ++j)
                C[n + i, m + j] = 0.0;
        }

        return C;
    }

    private double NodeSubstitutionCost(Graph pattern, Graph target, int p, int t)
    {
        double nodeLabelCost = 0.0;

        double localEdgeCost = LocalNeighborhoodCost(pattern, target, p, t);

        return nodeLabelCost + localEdgeCost;
    }

    private double LocalNeighborhoodCost(Graph pattern, Graph target, int p, int t)
    {
        var pNeighbors = GetNeighbors(pattern, p).ToList();
        var tNeighbors = GetNeighbors(target, t).ToList();

        var dp = pNeighbors.Count;
        var dt = tNeighbors.Count;
        var k = Math.Max(dp, dt);

        if (k == 0) 
            return 0.0;

        var L = new double[k, k];

        for (var i = 0; i < k; ++i)
        {
            for (var j = 0; j < k; ++j)
            {
                if (i < dp && j < dt)
                {
                    var u = pNeighbors[i];
                    var v = tNeighbors[j];

                    L[i, j] = NeighborSubstitutionCost(pattern, target, u, v);
                }
                else if (i < dp) 
                {
                    var u = pNeighbors[i];
                    L[i, j] = NodeDeletionCost(pattern, u);
                }
                else if (j < dt)
                {
                    var v = tNeighbors[j];
                    L[i, j] = NodeInsertionCost(target, v);
                }
                else
                    L[i, j] = 0.0;
            }
        }

        var (assignment, totalLocalCost) = Munkres.Solve(L);
        return totalLocalCost;
    }

    private double NeighborSubstitutionCost(Graph pattern, Graph target, int pu, int tv)
    {
        var dpu = pattern.OutDegree(pu);
        var dtv = target.OutDegree(tv);

        if (dpu == dtv) 
            return 0.0;

        return edgeEditCost * Math.Abs(dpu - dtv);
    }

    private double NodeDeletionCost(Graph pattern, int pNode)
    {
        return nodeDeleteCostBase + edgeEditCost * pattern.OutDegree(pNode);
    }

    private double NodeInsertionCost(Graph target, int tNode)
    {
        return nodeDeleteCostBase + edgeEditCost * target.OutDegree(tNode);
    }

    private static List<int> GetNeighbors(Graph g, int node)
    {
        var list = new List<int>();
        for (var i = 0; i < g.size; ++i)
        {
            if (i != node && (g.adjMatrix[node, i] || g.adjMatrix[i, node]))
                list.Add(i);
        }
        return list;
    }

    public MappingResult ComputeEditDistance(Graph pattern, Graph target)
    {
        var costMatrix = BuildCostMatrix(pattern, target);
        var (assignment, totalCost) = Munkres.Solve(costMatrix);

        var mapping = ExtractNodeMapping(assignment, pattern.size, target.size);
        var supergraph = BuildSupergraph(pattern, target, mapping);

        var isExact = IsExactSubgraphIsomorphism(pattern, target, mapping);

        return new MappingResult(mapping, totalCost, isExact, supergraph);
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
        var supergraph = target.Clone();

        for (var u = 0; u < pattern.size; ++u)
        {
            var mu = mapping[u];
            if (mu < 0) continue;

            for (var v = 0; v < pattern.size; ++v)
            {
                if (!pattern.adjMatrix[u, v]) continue;

                var mv = mapping[v];
                if (mv >= 0 && !supergraph.adjMatrix[mu, mv])
                {
                    supergraph.adjMatrix[mu, mv] = true;
                    supergraph.edgesWereAdded = true;
                    supergraph.isNewEdge[mu, mv] = true;
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
                if (!pattern.adjMatrix[u, v]) continue;

                var mu = mapping[u];
                var mv = mapping[v];
                if (mu < 0 || mv < 0 || !target.adjMatrix[mu, mv])
                    return false;
            }
        }

        return true;
    }

    public static int CalculateMissingEdges(Graph pattern, Graph target, int[] mapping)
    {
        var missing = 0;

        for (var u = 0; u < pattern.size; ++u)
        {
            for (var v = 0; v < pattern.size; ++v)
            {
                if (!pattern.adjMatrix[u, v]) continue;

                var mu = mapping[u];
                var mv = mapping[v];
                if (mu < 0 || mv < 0 || !target.adjMatrix[mu, mv])
                    missing++;
            }
        }

        return missing;
    }

    public Results Solve(Graph g1, Graph g2)
    {
        var mappingResult = ComputeEditDistance(g1, g2);
        var missingEdges = CalculateMissingEdges(g1, g2, mappingResult.Mapping);
        return new Results(mappingResult.Mapping, missingEdges, mappingResult.IsExact, mappingResult.Supergraph, g2.size);
    }

    public string Name() => "Przybliżenie Riesen-Bunke Bipartite (Munkres)"; // TODO: ZMIEN NAZWE
    // Riesen-Bunke Bipartite (Munkres) Approximation
}
