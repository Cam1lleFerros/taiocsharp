using SubgraphIsomorphism.Utils;

namespace SubgraphIsomorphism.Munkres;

public class MunkresSolver : ISubgraphIsomorphismSolver
{
    public record MappingResult(int[] Mapping, double EditDistance, bool IsExact, Graph Supergraph);

    private readonly double nodeBaseCost;
    private readonly double edgeCost;

    private const double INF = double.MaxValue / 4;

    public MunkresSolver(double nodeBaseCost = 1.0, double edgeCost = 1.0)
    {
        this.nodeBaseCost = nodeBaseCost;
        this.edgeCost = edgeCost;
    }

    public MappingResult ComputeEditDistance(Graph pattern, Graph target)
    {
        var costMatrix = BuildCostMatrix(pattern, target);
        var (assignment, totalCost) = Munkres.Solve(costMatrix);

        var mapping = ExtractNodeMappingForceReal(assignment, pattern.size, target.size, costMatrix);
        var supergraph = BuildSupergraph(pattern, target, mapping);

        var isExact = IsExactSubgraphIsomorphism(pattern, target, mapping);

        return new MappingResult(mapping, totalCost, isExact, supergraph);
    }

    public double[,] BuildCostMatrix(Graph pattern, Graph target)
    {
        var n = pattern.size;
        var m = target.size;
        var total = n + m;
        var C = new double[total, total];

        for (var i = 0; i < total; ++i)
        {
            for (var j = 0; j < total; ++j)
                C[i, j] = INF;
        }

        for (var i = 0; i < n; ++i)
        {
            for (var j = 0; j < m; ++j)
                C[i, j] = NodeSubstitutionCost(pattern, target, i, j);
        }

        for (var i = 0; i < n; ++i)
            C[i, m + i] = INF;

        for (var j = 0; j < m; ++j)
            C[n + j, j] = NodeInsertionCost(target, j);

        for (var i = 0; i < m; ++i)
        {
            for (var j = 0; j < n; ++j)
                C[n + i, m + j] = 0.0;
        }

        return C;
    }

    private double NodeSubstitutionCost(Graph pattern, Graph target, int pu, int tv)
    {
        double nodeLabelCost = 0.0; // brak etykiet w przykładzie — jeśli masz etykiety zastąp
        double local = LocalNeighborhoodCost(pattern, target, pu, tv);
        return nodeLabelCost + local;
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
                    L[i, j] = nodeBaseCost + edgeCost * pattern.OutDegree(u);
                }
                else if (j < dt) 
                {
                    var v = tNeighbors[j];
                    L[i, j] = nodeBaseCost + edgeCost * target.OutDegree(v);
                }
                else
                    L[i, j] = 0.0;
            }
        }

        var (assign, localCost) = Munkres.Solve(L);
        return localCost;
    }

    private double NeighborSubstitutionCost(Graph pattern, Graph target, int pu, int tv)
    {
        var dpu = pattern.OutDegree(pu);
        var dtv = target.OutDegree(tv);
        // Jeżeli chcesz surowiej karać, podnieś wagę:
        return edgeCost * Math.Abs(dpu - dtv);
    }

    private double NodeInsertionCost(Graph target, int tNode)
    {
        return nodeBaseCost + edgeCost * target.OutDegree(tNode);
    }


    private static List<int> GetNeighbors(Graph g, int node)
    {
        var list = new List<int>();
        for (var i = 0; i < g.size; i++)
        {
            if (i == node) continue;
            if (g.adjMatrix[node, i] || g.adjMatrix[i, node])
                list.Add(i);
        }
        return list;
    }

    private static int[] ExtractNodeMappingForceReal(bool[,] assignment, int n, int m, double[,] originalCost)
    {
        var mapping = new int[n];
        Array.Fill(mapping, -1);

        var targetTaken = new bool[m];

        for (var i = 0; i < n; ++i)
        {
            for (var j = 0; j < m; ++j)
            {
                if (assignment[i, j])
                {
                    mapping[i] = j;
                    targetTaken[j] = true;
                    break;
                }
            }
        }

        for (var i = 0; i < n; ++i)
        {
            if (mapping[i] != -1) continue;

            int bestFree = -1;
            double bestFreeCost = double.PositiveInfinity;

            for (var j = 0; j < m; ++j)
            {
                if (targetTaken[j]) continue;
                var c = originalCost[i, j];
                if (c < bestFreeCost)
                {
                    bestFreeCost = c;
                    bestFree = j;
                }
            }

            if (bestFree != -1)
            {
                mapping[i] = bestFree;
                targetTaken[bestFree] = true;
                continue;
            }

            var bestAny = -1;
            var bestAnyCost = double.PositiveInfinity;
            for (var j = 0; j < m; ++j)
            {
                var c = originalCost[i, j];
                if (c < bestAnyCost)
                {
                    bestAnyCost = c;
                    bestAny = j;
                }
            }

            mapping[i] = bestAny >= 0 ? bestAny : 0;
        }

        return mapping;
    }

    private static Graph BuildSupergraph(Graph pattern, Graph target, int[] mapping)
    {
        var supergraph = target.Clone();

        for (var u = 0; u < pattern.size; ++u)
        {
            var mu = mapping[u];
            if (mu < 0 || mu >= supergraph.size) continue;

            for (var v = 0; v < pattern.size; ++v)
            {
                if (!pattern.adjMatrix[u, v]) continue;

                var mv = mapping[v];
                if (mv >= 0 && mv < supergraph.size && !supergraph.adjMatrix[mu, mv])
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
                    ++missing;
            }
        }

        return missing;
    }

    public Results Solve(Graph g1, Graph g2)
    {
        var res = ComputeEditDistance(g1, g2);
        var missingEdges = CalculateMissingEdges(g1, g2, res.Mapping);
        return new Results(res.Mapping, missingEdges, res.IsExact, res.Supergraph, g2.size);
    }

    public string Name() => "Przybliżenie Munkres";
}
