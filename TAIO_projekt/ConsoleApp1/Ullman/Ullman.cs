using TAiO;

namespace ConsoleApp1.Ullman;

public class Ullman
{
    public Graph g, p;

    public Ullman(Graph p, Graph g)
    {
        this.g = g;
        this.p = p;
    }

    public int Rows => p.size;
    public int Cols => g.size;

    public bool[,]? FindIsomorphism()
    {
        var mapping = new bool[Rows, Cols];
        var usedColumns = new bool[Cols];
        Array.Fill(usedColumns, false);
        var found = false;
        for (var i = 0; i < Rows; ++i)
        {
            for (var j = 0; j < Cols; ++j)
            {
                if (p.OutDegree(i) <= g.OutDegree(j))
                {
                    mapping[i, j] = true;
                    found = true;
                }
            }
        }
        if (!found)
            return null;    
        return Recurse(mapping, 0, usedColumns);
    }

    public bool[,]? Recurse(bool[,] mapping, int currentRow, bool[] usedColumns)
    {
        if (currentRow == Rows)
            return IsValidMapping(mapping) ? mapping : null;

        var mprim = new bool[Rows, Cols];
        for (var i = 0; i < Rows; ++i)
        {
            for (var j = 0; j < Cols; ++j)
                mprim[i, j] = mapping[i, j];
        }
        Prune(mprim);


        for (var col = 0; col < Cols; ++col)
        {
            if (!usedColumns[col])
            {
                usedColumns[col] = true;
                for (var i = 0; i < Cols; ++i)
                    mprim[currentRow, i] = i == col;
                var result = Recurse(mprim, currentRow + 1, usedColumns);
                if (result != null)
                    return result;
                mprim[currentRow, col] = false;
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
        return true;
    }
}
