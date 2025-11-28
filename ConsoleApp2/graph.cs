using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ConsoleApp2
{
    internal class graph
    {
        public int size;
        public bool[,] adjMatrix;
        public graph(int n)
        {
            size = n;
            adjMatrix = new bool[n, n];
        }

        //pierwszy wiersz pliku zawiera liczbę wierzchołków pierwszego grafu, ta informacja jest zapisana w jednym wierszu pliku,
        //następne wiersze pliku zawierają wiersze macierzy sąsiedztwa pierwszego grafu z elementami oddzielonymi spacją,
        //kolejny wiersz pliku zawiera liczbę wierzchołków drugiego grafu,
        //następne wiersze pliku zawierają wiersze macierzy sąsiedztwa drugiego grafu,
        internal static (graph, graph) ReadTwoFromFile(string path)
        {
            var rawLines = File.ReadAllLines(path).Select(l => l.Trim()).ToArray();
            int index = 0;

            int n1 = int.Parse(rawLines[index++]);
            var g1 = new graph(n1);
            for (int r = 0; r < n1; r++)
            {
                var tokens = rawLines[index++].Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
                for (int c = 0; c < n1; c++)
                {
                    g1.adjMatrix[r, c] = ParseBooleanToken(tokens[c], null);
                }
            }

            int n2 = int.Parse(rawLines[index++]);
            var g2 = new graph(n2);
            for (int r = 0; r < n2; r++)
            {
                var tokens = rawLines[index++].Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
                for (int c = 0; c < n2; c++)
                {
                    g2.adjMatrix[r, c] = ParseBooleanToken(tokens[c], null);
                }
            }

            return (g1, g2);
        }

        private static bool ParseBooleanToken(string token, string context)
        {
            if (token == "0" )
                return false;
            else
                return true;
        }
    }
}
