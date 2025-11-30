using System;
using System.Collections.Generic;

namespace SubgraphIsomorphism.Munkres;

internal class Munkres
{
    public static (bool[,], int[,]) Muknres(int[,] tab)
    {
        var x = tab.GetLength(0);
        var y = tab.GetLength(1);
        var star = new bool[x, y];
        var prime = new bool[x, y];
        var row_covered = new bool[x];
        var col_covered = new bool[y];
        var z0_row = -1;
        var z0_col = -1;
        var emin = int.MaxValue;

        
        for (var i = 0; i < x; ++i)
        {
            var min = tab[i, 0];
            for (var j = 1; j < y; ++j)
                if (tab[i, j] < min)
                    min = tab[i, j];
            for (var j = 0; j < y; ++j)
                tab[i, j] -= min;
        }

        
        for (var j = 0; j < y; ++j)
        {
            var min = tab[0, j];
            for (var i = 1; i < x; ++i)
                if (tab[i, j] < min)
                    min = tab[i, j];
            for (var i = 0; i < x; ++i)
                tab[i, j] -= min;
        }

        
        for (var i = 0; i < x; ++i)
        {
            for (var j = 0; j < y; ++j)
            {
                if (tab[i, j] == 0 && !row_covered[i] && !col_covered[j])       
                {
                    star[i, j] = true;
                    row_covered[i] = true;
                    col_covered[j] = true;
                }
            }
        }

       
        for (var i = 0; i < x; ++i)
            row_covered[i] = false;

    Step1:
        
        for (var j = 0; j < y; ++j)
        {
            col_covered[j] = false;
            for (var i = 0; i < x; ++i)
            {
                if (star[i, j])
                {
                    col_covered[j] = true;
                    break;
                }
            }
        }

       
        for (var j = 0; j < y; ++j)
        {
            if (!col_covered[j])
                goto Step2;
        }
        goto Break;

    Step2:
        
        emin = int.MaxValue;

        for (var i = 0; i < x; ++i)
        {
            for (var j = 0; j < y; ++j)
            {
                
                if (tab[i, j] == 0 && !col_covered[j] && !row_covered[i])
                {
                    prime[i, j] = true;

                   
                    var starColInRow = -1;
                    for (var c = 0; c < y; ++c)
                    {
                        if (star[i, c])
                        {
                            starColInRow = c;
                            break;
                        }
                    }

                    if (starColInRow == -1)
                    {
                        
                        z0_row = i;
                        z0_col = j;
                        goto Step3;
                    }
                    else
                    {
                        
                        row_covered[i] = true;
                        col_covered[starColInRow] = false;
                        
                        goto Step2;
                    }
                }

               
                if (!col_covered[j] && !row_covered[i] && tab[i, j] < emin)
                    emin = tab[i, j];
            }
        }

        goto Step4;

    Step3:
        var series = new List<(int r, int c)>();
        if (z0_row < 0 || z0_col < 0)
            goto Step1;

        series.Add((z0_row, z0_col));

        while (true)
        {
            var lastCol = series[^1].c;
            var starRow = -1;
            for (var r = 0; r < x; ++r)
            {
                if (star[r, lastCol])
                {
                    starRow = r;
                    break;
                }
            }

            if (starRow == -1)
                break;

            series.Add((starRow, lastCol));

            var primedCol = -1;
            for (var c = 0; c < y; ++c)
            {
                if (prime[starRow, c])
                {
                    primedCol = c;
                    break;
                }
            }

            if (primedCol == -1)
                break;

            series.Add((starRow, primedCol));
        }


        foreach (var (r, c) in series)
            star[r, c] = !star[r, c];


        for (var i = 0; i < x; ++i)
            for (var j = 0; j < y; ++j)
                prime[i, j] = false;


        for (var i = 0; i < x; ++i) row_covered[i] = false;
        for (var j = 0; j < y; ++j) col_covered[j] = false;


        goto Step1;

    Step4:

        for (var i = 0; i < x; ++i)
        {
            for (var j = 0; j < y; ++j)
            {
                if (row_covered[i])
                    tab[i, j] += emin;
                if (!col_covered[j])
                    tab[i, j] -= emin;
            }
        }


        emin = int.MaxValue;
        goto Step2;

    Break:
        return (star, tab);
    }
}
