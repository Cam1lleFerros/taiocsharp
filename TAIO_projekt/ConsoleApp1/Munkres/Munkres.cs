namespace SubgraphIsomorphism.Munkres;

public class Munkres
{
    public static (bool[,] assignment, double totalCost) Solve(double[,] costMatrix)
    {
        var n = costMatrix.GetLength(0);
        var cost = (double[,])costMatrix.Clone();

        for (var i = 0; i < n; i++)
        {
            double min = cost[i, 0];
            for (var j = 1; j < n; j++)
                if (cost[i, j] < min) min = cost[i, j];
            for (var j = 0; j < n; j++)
                cost[i, j] -= min;
        }

        for (var j = 0; j < n; j++)
        {
            double min = cost[0, j];
            for (var i = 1; i < n; i++)
                if (cost[i, j] < min) min = cost[i, j];
            for (var i = 0; i < n; i++)
                cost[i, j] -= min;
        }

        return RunHungarianAlgorithm(cost, costMatrix);
    }

    private static (bool[,] assignment, double totalCost) RunHungarianAlgorithm(double[,] cost, double[,] originalCost)
    {
        var n = cost.GetLength(0);
        var starred = new bool[n, n];
        var primeMarks = new bool[n, n];
        var rowCovered = new bool[n];
        var colCovered = new bool[n];
        var maxIterations = n * n * 10;
        var iterations = 0;

        for (var i = 0; i < n; i++)
        {
            for (var j = 0; j < n; j++)
            {
                if (IsZero(cost[i, j]) && !rowCovered[i] && !colCovered[j])
                {
                    starred[i, j] = true;
                    rowCovered[i] = true;
                    colCovered[j] = true;
                }
            }
        }

        ClearCovers(rowCovered, colCovered);

        while (iterations++ < maxIterations)
        {
            int coveredColumns = CoverStarredColumns(starred, colCovered);

            if (coveredColumns == n)
                break;

            while (true)
            {
                var (zeroRow, zeroCol) = FindUncoveredZero(cost, rowCovered, colCovered);

                if (zeroRow == -1)
                {
                    AdjustMatrix(cost, rowCovered, colCovered);
                    (zeroRow, zeroCol) = FindUncoveredZero(cost, rowCovered, colCovered);

                    if (zeroRow == -1)
                        break;
                }

                primeMarks[zeroRow, zeroCol] = true;

                var starCol = FindStarInRow(starred, zeroRow);

                if (starCol == -1)
                {
                    AugmentPath(starred, primeMarks, zeroRow, zeroCol);
                    ClearCovers(rowCovered, colCovered);
                    ClearPrimes(primeMarks);
                    break;
                }
                else
                {
                    rowCovered[zeroRow] = true;
                    colCovered[starCol] = false;
                }
            }
        }

        double totalCost = 0;
        for (var i = 0; i < n; i++)
            for (var j = 0; j < n; j++)
                if (starred[i, j])
                    totalCost += originalCost[i, j];

        return (starred, totalCost);
    }

    private static bool IsZero(double value, double tolerance = 1e-10) => Math.Abs(value) < tolerance;

    private static void ClearCovers(bool[] rowCovered, bool[] colCovered)
    {
        Array.Fill(rowCovered, false);
        Array.Fill(colCovered, false);
    }

    private static void ClearPrimes(bool[,] primeMarks)
    {
        int n = primeMarks.GetLength(0);
        for (int i = 0; i < n; i++)
            for (int j = 0; j < n; j++)
                primeMarks[i, j] = false;
    }

    private static int CoverStarredColumns(bool[,] starred, bool[] colCovered)
    {
        int n = starred.GetLength(0);
        int coveredCount = 0;

        for (int j = 0; j < n; j++)
        {
            for (int i = 0; i < n; i++)
            {
                if (starred[i, j])
                {
                    colCovered[j] = true;
                    coveredCount++;
                    break;
                }
            }
        }

        return coveredCount;
    }

    private static (int row, int col) FindUncoveredZero(double[,] cost, bool[] rowCovered, bool[] colCovered)
    {
        int n = cost.GetLength(0);
        for (int i = 0; i < n; i++)
        {
            if (rowCovered[i]) continue;
            for (int j = 0; j < n; j++)
            {
                if (colCovered[j]) continue;
                if (IsZero(cost[i, j]))
                    return (i, j);
            }
        }
        return (-1, -1);
    }

    private static int FindStarInRow(bool[,] starred, int row)
    {
        int n = starred.GetLength(1);
        for (int j = 0; j < n; j++)
            if (starred[row, j])
                return j;
        return -1;
    }

    private static int FindStarInColumn(bool[,] starred, int col)
    {
        int n = starred.GetLength(0);
        for (int i = 0; i < n; i++)
            if (starred[i, col])
                return i;
        return -1;
    }

    private static void AdjustMatrix(double[,] cost, bool[] rowCovered, bool[] colCovered)
    {
        int n = cost.GetLength(0);

        double minVal = double.MaxValue;
        for (int i = 0; i < n; i++)
        {
            if (rowCovered[i]) continue;
            for (int j = 0; j < n; j++)
            {
                if (colCovered[j]) continue;
                if (cost[i, j] < minVal)
                    minVal = cost[i, j];
            }
        }

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                if (rowCovered[i])
                    cost[i, j] += minVal;
                if (!colCovered[j])
                    cost[i, j] -= minVal;
            }
        }
    }

    private static void AugmentPath(bool[,] starred, bool[,] primeMarks, int startRow, int startCol)
    {
        var path = new List<(int, int)>
        {
            (startRow, startCol)
        };

        var lookingInRow = false;

        while (true)
        {
            if (!lookingInRow)
            {
                var lastCol = path[^1].Item2;
                var starRow = FindStarInColumn(starred, lastCol);

                if (starRow == -1)
                    break;

                path.Add((starRow, lastCol));
                lookingInRow = true;
            }
            else
            {
                var lastRow = path[^1].Item1;
                var primeCol = -1;
                for (int j = 0; j < primeMarks.GetLength(1); j++)
                {
                    if (primeMarks[lastRow, j])
                    {
                        primeCol = j;
                        break;
                    }
                }

                if (primeCol == -1)
                    break;

                path.Add((lastRow, primeCol));
                lookingInRow = false;
            }
        }

        foreach (var (row, col) in path)
            starred[row, col] = !starred[row, col];
    }
}