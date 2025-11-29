using System;
using System.Collections.Generic;
using System.Text;

namespace TAiO
{
    internal class Munkres
    {
        public (bool[,], int[,]) muknres(int[,] tab)
        {

            int x=tab.GetLength(0);
            int y=tab.GetLength(1);
            bool[,] star=new bool[x,y];
            bool[,] prime=new bool[x,y];
            bool[] row_covered=new bool[x];
            bool[] col_covered=new bool[y];
            int z0_row = -1, z0_col = -1;
            int emin = int.MaxValue;
            for (int i=0;i<x;i++)
            {
                int min=tab[i,0];
                for(int j=1;j<y;j++)
                {
                    if(tab[i,j]<min)
                    {
                        min=tab[i,j];
                    }
                }
                for(int j=0;j<y;j++)
                {
                    tab[i,j]-=min;
                }
            }
            for(int j=0;j<y;j++)
            {
                int min=tab[0,j];
                for(int i=1;i<x;i++)
                {
                    if(tab[i,j]<min)
                    {
                        min=tab[i,j];
                    }
                }
                for(int i=0;i<x;i++)
                {
                    tab[i,j]-=min;
                }
            }
            for(int i=0;i<x;i++)
            {
                for(int j=0;j<y;j++)
                {
                    if(tab[i,j]==0 && !row_covered[i] && !col_covered[j])
                    {
                        star[i,j]=true;
                        row_covered[i]=true;
                        col_covered[j]=true;
                    }
                }              
            }
            for (int i = 0; i < x; i++)
            {
                row_covered[i] = false;
            }
        Step1:
            {
                for (int i = 0; i < x; i++)
                {
                    for (int j = 0; j < y; j++)
                    {
                        if (col_covered[j])
                            break;
                        if (star[i, j] == true )
                        {
                            col_covered[j] = true;
                            break;
                        }
                    }
                }
                for( int i = 0; i < y; i++)
                {
                    if (!col_covered[i])
                    {
                        goto Step2;

                    }
                }
                goto Break;
            }
        Step2:
            {

                for( int i = 0; i < x; i++)
                {
                    for (int j=0; j < y; j++)
                    {
                        if (tab[i, j] == 0 & !col_covered[j] & !row_covered[i])
                        {
                            prime[i, j] = true;
                            for(int k = 0; k < x; k++)
                            {
                                if (star[k,j] == true)
                                {
                                    row_covered[k] = true;
                                    col_covered[j] = false;
                                    goto Step2;
                                }
                            }
                            goto Step3;
                        }
                        if(tab[i, j] <emin & !col_covered[j] & !row_covered[i])
                        {
                            emin = tab[i, j];
                        }
                    }
                }
                goto Step4;
            }
        Step3:
            {

                var series = new List<(int r, int c)>();
                if (z0_row < 0 || z0_col < 0)
                {

                    goto Step1;
                }

                series.Add((z0_row, z0_col));

                while (true)
                {
                    int lastCol = series[series.Count - 1].c;
                    int starRow = -1;
                    for (int r = 0; r < x; r++)
                    {
                        if (star[r, lastCol])
                        {
                            starRow = r;
                            break;
                        }
                    }

                    if (starRow == -1)
                    {

                        break;
                    }

                    series.Add((starRow, lastCol));


                    int primedCol = -1;
                    for (int c = 0; c < y; c++)
                    {
                        if (prime[starRow, c])
                        {
                            primedCol = c;
                            break;
                        }
                    }

                    if (primedCol == -1)
                    {

                        break;
                    }


                    series.Add((starRow, primedCol));
                }


                foreach (var p in series)
                {
                    if (star[p.r, p.c])
                        star[p.r, p.c] = false;
                    else
                        star[p.r, p.c] = true;
                }

                for (int i = 0; i < x; i++)
                {
                    for (int j = 0; j < y; j++)
                    {
                        prime[i, j] = false;
                    }
                }


                for (int i = 0; i < x; i++) row_covered[i] = false;
                for (int j = 0; j < y; j++) col_covered[j] = false;


                goto Step1;
            }
        Step4:
            {
                for (int i = 0;i < x; i++)
                {
                    for(int j = 0; j < y; j++)
                    {
                        if (row_covered[i] )
                        {
                            tab[i, j] += emin;
                        }
                        if (!col_covered[j])
                        {
                            tab[i, j] -= emin;
                        }
                    }
                }
                emin = int.MaxValue;
                goto Step2;
            }
        Break:
            {
                return (star,tab);
            }


        }
    }
}
