using System;
using System.Collections.Generic;
using System.Linq;

namespace SubgraphIsomorphism.Munkres
{
    public static class MunkresUsage
    {
        public record MappingResult(int[] Mapping, int MissingEdges, bool IsExact, Graph Supergraph);

        // Buduje macierz kosztów: wiersze = wierzcho³ki mniejszego grafu (s),
        // kolumny = wierzcho³ki wiêkszego grafu (l).
        // Koszt oparty jest na ró¿nicy stopni i ró¿nicy "s¹siednich stopni" (prosta sygnatura).
        public static int[,] BuildCostMatrix(Graph s, Graph l)
        {
            var m = s.size;
            var n = l.size;
            var cost = new int[m, n];

            for (var i = 0; i < m; ++i)
            {
                var sDeg = s.OutDegree(i);
                var sNbrDegrees = s.OutNeighbours(i).Select(v => s.OutDegree(v)).OrderBy(x => x).ToArray();

                for (var j = 0; j < n; ++j)
                {
                    var lDeg = l.OutDegree(j);
                    var lNbrDegrees = l.OutNeighbours(j).Select(v => l.OutDegree(v)).OrderBy(x => x).ToArray();

                    var c = Math.Abs(sDeg - lDeg);

                    var len = Math.Min(sNbrDegrees.Length, lNbrDegrees.Length);
                    for (var k = 0; k < len; ++k)
                        c += Math.Abs(sNbrDegrees[k] - lNbrDegrees[k]);

                    // kara za ró¿nicê liczby s¹siadów
                    c += Math.Abs(sNbrDegrees.Length - lNbrDegrees.Length);

                    cost[i, j] = c;
                }
            }

            return cost;
        }


        public static MappingResult FindMappingAndMissingEdges(Graph s, Graph l)
        {
            if (s.size > l.size)
                throw new ArgumentException("Graf mniejszy musi mieæ mniej lub tyle samo wierzcho³ków co wiêkszy graf.");

            var cost = BuildCostMatrix(s, l);


            var tab = new int[cost.GetLength(0), cost.GetLength(1)];
            Array.Copy(cost, tab, cost.Length);

            var (star, _) = Munkres.Muknres(tab); 

            var mapping = Enumerable.Repeat(-1, s.size).ToArray();
            for (var i = 0; i < s.size; ++i)
            {
                for (var j = 0; j < l.size; ++j)
                {
                    if (star.GetLength(0) > i && star.GetLength(1) > j && star[i, j])
                    {
                        mapping[i] = j;
                        break;
                    }
                }
            }


            var missing = 0;
            for (var u = 0; u < s.size; ++u)
            {
                for (var v = u + 1; v < s.size; ++v)
                {
                    if (!s.adjMatrix[u, v] && !s.adjMatrix[v, u]) continue;

                    var mu = mapping[u];
                    var mv = mapping[v];

                    if (mu < 0 || mv < 0)
                    {
 
                        missing++;
                        continue;
                    }

                    if (s.adjMatrix[u, v] && !l.adjMatrix[mu, mv])
                        missing++;

                    if (s.adjMatrix[v, u] && !l.adjMatrix[mv, mu])
                        missing++;
                }
            }


            var super = new Graph(l.size);
            for (var i = 0; i < l.size; ++i)
            for (var j = 0; j < l.size; ++j)
                super.adjMatrix[i, j] = l.adjMatrix[i, j];

            for (var u = 0; u < s.size; ++u)
            {
                for (var v = 0; v < s.size; ++v)
                {
                    if (!s.adjMatrix[u, v]) continue;

                    var mu = mapping[u];
                    var mv = mapping[v];

                    if (mu < 0 || mv < 0) continue; 

                    if (!super.adjMatrix[mu, mv])
                        super.adjMatrix[mu, mv] = true;
                }
            }

            return new MappingResult(mapping, missing, missing == 0, super);
        }
    }
}