using System;
using System.Collections.Generic;
using System.Text;

namespace TAiO
{
    internal class Munkres
    {
        public void muknres(int[,] tab)
        {

            int x=tab.GetLength(0);
            int y=tab.GetLength(1);
            bool[,] star=new bool[x,y];
            bool[,] prime=new bool[x,y];
            bool[] row_covered=new bool[x];
            bool[] col_covered=new bool[y];
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
            for(int i=0;i<x;i++)
            {
                row_covered[i]=false;
            }

        }
    }
}
