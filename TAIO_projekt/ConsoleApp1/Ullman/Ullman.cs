using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAiO;

namespace ConsoleApp1.Ullman
{
    public class Ullman
    {
        public Graph g, p;

        public Ullman(Graph p, Graph g)
        {
            this.g = g;
            this.p = p;
        }

        public int rows => p.size;
        public int cols => g.size;

        public bool[,]? FindIsomorphism()
        {
            bool[,] mapping = new bool[rows, cols];
            bool[] usedColumns = new bool[cols];
            Array.Fill(usedColumns, false);
            bool found = false;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (p.OutDegree(i) <= g.OutDegree(j))
                    {
                        mapping[i, j] = true;
                        found = true;
                    }
                }
            }
            if(!found)
                return null;    
            return Recurse(mapping, 0, usedColumns);
        }

        public bool[,]? Recurse(bool[,] mapping, int currentRow, bool[] usedColumns)
        {
            if(currentRow == rows)
            {
                if (IsValidMapping(mapping))
                {
                    return mapping;
                }
                else
                {
                    return null;
                }
            }

            bool[,] mprim = new bool[rows, cols];
            for(int i = 0; i<rows; i++)
            {
                for(int j = 0; j < cols; j++)
                {
                    mprim[i, j] = mapping[i, j];
                }
                
            }
            Prune(mprim);


            for (int col = 0; col < cols; col++)
            {
                if (!usedColumns[col])
                {
                    usedColumns[col] = true;
                    for(int i = 0; i<cols; i++)
                    {
                        mprim[currentRow, i] = (i == col);
                    }
                    var result = Recurse(mprim, currentRow + 1, usedColumns);
                    if (result != null)
                    {
                        return result;
                    }
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
                for (int i = 0; i < rows; i++)
                {
                    for(int j = 0; j < cols; j++)
                    {
                        if (mprim[i, j])
                        {
                            foreach (var np in p.OutNeighbours(i))
                            {
                                bool hasMapping = false;
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
            int sum;
            for(int i = 0; i<rows; i++)
            {
                sum = 0;
                for (int j = 0; j < cols; j++)
                {
                    sum += (mapping[i, j] ? 1 : 0);
                }
                if (sum != 1) return false;
            }
            return true;
        }
    }
}
