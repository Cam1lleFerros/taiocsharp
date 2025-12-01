using SubgraphIsomorphism.Utils;
namespace SubgraphIsomorphism.Ullman;

public class Ullman(Graph p, Graph g)
{
    public Graph g = g, p = p;

    private int Rows => p.size;
    private int Cols => g.size;
    private int deepestMatch = 0;
    private bool[,]? bestMatch = null;

    public (bool, bool[,]?) FindIsomorphism()
    {
        var mapping = new bool[Rows, Cols];
        var usedColumns = new bool[Cols];
        Array.Fill(usedColumns, false);

        for (var i = 0; i < Rows; ++i)
        {
            for (var j = 0; j < Cols; ++j)
            {
                if (p.OutDegree(i) <= g.OutDegree(j) && GetInDegree(p, i) <= GetInDegree(g, j))
                    mapping[i, j] = true;
            }
        }

        for (var i = 0; i < Rows; ++i)
        {
            bool hasCandidate = false;
            for (var j = 0; j < Cols; ++j)
            {
                if (mapping[i, j])
                {
                    hasCandidate = true;
                    break;
                }
            }
            if (!hasCandidate)
                return (false, null);
        }

        var res = Recurse(mapping, 0, usedColumns);
        if (res != null)
            return (true, res);
        else
            return (false, bestMatch);
    }

    public bool[,]? Recurse(bool[,] mapping, int currentRow, bool[] usedColumns, int depth = 0)
    {
        if (currentRow == Rows)
        {
            if (IsValidMapping(mapping))
                return mapping;
            return null;
        }
        if (depth > deepestMatch)
        {
            deepestMatch = depth;
            bestMatch = (bool[,])mapping.Clone();
        }

        var mprim = (bool[,])mapping.Clone();
        Prune(mprim);

        for (var col = 0; col < Cols; ++col)
        {
            if (mprim[currentRow, col] && !usedColumns[col])
            {
                usedColumns[col] = true;

                var newMapping = (bool[,])mprim.Clone();
                for (var i = 0; i < Cols; ++i)
                    newMapping[currentRow, i] = i == col;

                var result = Recurse(newMapping, currentRow + 1, usedColumns, depth + 1);
                if (result != null)
                    return result;

                usedColumns[col] = false;
            }
        }
        return null;
    }

    private void Prune(bool[,] mprim)
    {
        bool changed;
        do
        {
            changed = false;
            for (var i = 0; i < Rows; ++i)
            {
                for (var j = 0; j < Cols; ++j)
                {
                    if (mprim[i, j])
                    {
                        foreach (var np in p.OutNeighbours(i))
                        {
                            var hasMapping = false;
                            foreach (var ng in g.OutNeighbours(j))
                            {
                                if (mprim[np, ng])
                                {
                                    hasMapping = true;
                                    break;
                                }
                            }
                            if (!hasMapping)
                            {
                                mprim[i, j] = false;
                                changed = true;
                                break;
                            }
                        }
                    }
                }
            }
        } while (changed);
    }

    private bool IsValidMapping(bool[,] mapping)
    {
        for (var i = 0; i < Rows; ++i)
        {
            var sum = 0;
            for (var j = 0; j < Cols; ++j)
                sum += mapping[i, j] ? 1 : 0;
            if (sum != 1)
                return false;
        }

        for (var i = 0; i < Rows; ++i)
        {
            for (var j = 0; j < Rows; ++j)
            {
                if (p.adjMatrix[i, j])
                {
                    var mappedI = -1;
                    var mappedJ = -1;

                    for (var k = 0; k < Cols; ++k)
                    {
                        if (mapping[i, k]) mappedI = k;
                        if (mapping[j, k]) mappedJ = k;
                    }

                    if (mappedI != -1 && mappedJ != -1 && !g.adjMatrix[mappedI, mappedJ])
                        return false;
                }
            }
        }
        return true;
    }

    private static int GetInDegree(Graph graph, int node)
    {
        var degree = 0;
        for (var i = 0; i < graph.size; ++i)
        {
            if (i != node && graph.adjMatrix[i, node])
                ++degree;
        }
        return degree;
    }
}