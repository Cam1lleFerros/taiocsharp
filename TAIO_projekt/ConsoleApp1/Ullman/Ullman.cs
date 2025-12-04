using SubgraphIsomorphism.Utils;
namespace SubgraphIsomorphism.Ullman;

public class Ullman(Graph p, Graph g)
{
    public Graph g = g, p = p;
    private int deepestMatch = 0;
    private bool[,]? bestMatch = null;
    private int bestCost = int.MaxValue;
    private List<(int, int)> bestEdgesToAdd = [];


    private int Rows => p.size;
    private int Cols => g.size;

    public (bool foundExact, bool[,]? mapping, int cost, List<(int, int)> edgesToAdd) FindIsomorphismOrExtension()
    {
        var mapping = new bool[Rows, Cols];
        var usedColumns = new bool[Cols];

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
                return (false, null, int.MaxValue, []);
        }

        RecurseWithCost(mapping, 0, usedColumns, 0, 0);

        if (bestMatch != null)
        {
            var finalMapping = new bool[Rows, Cols];
            var vertexMap = new int[Rows];
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    if (bestMatch[i, j])
                    {
                        finalMapping[i, j] = true;
                        vertexMap[i] = j;
                        break;
                    }
                }
            }

            var edgesToAdd = new List<(int, int)>();
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Rows; j++)
                {
                    if (p.adjMatrix[i, j] && !g.adjMatrix[vertexMap[i], vertexMap[j]])
                    {
                        edgesToAdd.Add((vertexMap[i], vertexMap[j]));
                    }
                }
            }

            bool exact = (bestCost == 0);
            return (exact, finalMapping, bestCost, edgesToAdd);
        }

        return (false, null, int.MaxValue, []);
    }

    private void RecurseWithCost(bool[,] mapping, int currentRow, bool[] usedColumns,
                                 int currentCost, int depth)
    {
        if (currentCost >= bestCost)
            return;

        if (currentRow == Rows)
        {
            if (IsCompleteInjectiveMapping(mapping))
            {

                int finalCost = CalculateMissingEdgesCost(mapping);
                if (finalCost < bestCost)
                {
                    bestCost = finalCost;
                    bestMatch = (bool[,])mapping.Clone();
                    deepestMatch = depth;
                }
            }
            return;
        }
        if (depth > deepestMatch)
        {
            deepestMatch = depth;
        }

        var mprim = (bool[,])mapping.Clone();
        PruneWithCostTracking(mprim, currentCost);

        for (var col = 0; col < Cols; ++col)
        {
            if (mprim[currentRow, col] && !usedColumns[col])
            {
                usedColumns[col] = true;

                var newMapping = (bool[,])mprim.Clone();
                for (var i = 0; i < Cols; ++i)
                    newMapping[currentRow, i] = i == col;

                int additionalCost = CalculateAdditionalCost(newMapping, currentRow, col, usedColumns);
                int newCost = currentCost + additionalCost;

                if (newCost < bestCost)
                {
                    RecurseWithCost(newMapping, currentRow + 1, usedColumns, newCost, depth + 1);
                }

                usedColumns[col] = false;
            }
        }
    }

    private int CalculateAdditionalCost(bool[,] mapping, int currentRow, int chosenCol, bool[] usedColumns)
    {
        int cost = 0;

        for (int prevRow = 0; prevRow < currentRow; prevRow++)
        {
            int prevCol = -1;
            for (int c = 0; c < Cols; c++)
            {
                if (mapping[prevRow, c])
                {
                    prevCol = c;
                    break;
                }
            }

            if (prevCol != -1)
            {
                if (p.adjMatrix[currentRow, prevRow] && !g.adjMatrix[chosenCol, prevCol])
                {
                    cost++;
                }
                if (p.adjMatrix[prevRow, currentRow] && !g.adjMatrix[prevCol, chosenCol])
                {
                    cost++;
                }
            }
        }

        return cost;
    }

    private void PruneWithCostTracking(bool[,] mprim, int currentCost)
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
                            bool hasFeasibleMapping = false;
                            foreach (var ng in g.OutNeighbours(j))
                            {
                                if (mprim[np, ng])
                                {
                                    hasFeasibleMapping = true;
                                    break;
                                }
                            }

                            if (!hasFeasibleMapping && currentCost + 1 >= bestCost)
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

    private bool IsCompleteInjectiveMapping(bool[,] mapping)
    {
        for (var i = 0; i < Rows; ++i)
        {
            var count = 0;
            for (var j = 0; j < Cols; ++j)
                if (mapping[i, j]) count++;
            if (count != 1) return false;
        }

        for (var j = 0; j < Cols; ++j)
        {
            var count = 0;
            for (var i = 0; i < Rows; ++i)
                if (mapping[i, j]) count++;
            if (count > 1) return false;
        }

        return true;
    }

    private int CalculateMissingEdgesCost(bool[,] mapping)
    {
        var vertexMap = new int[Rows];
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Cols; j++)
            {
                if (mapping[i, j])
                {
                    vertexMap[i] = j;
                    break;
                }
            }
        }

        int missingEdges = 0;
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Rows; j++)
            {
                if (p.adjMatrix[i, j] && !g.adjMatrix[vertexMap[i], vertexMap[j]])
                {
                    missingEdges++;
                }
            }
        }

        return missingEdges;
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