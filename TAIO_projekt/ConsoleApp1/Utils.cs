using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubgraphIsomorphism
{
    public interface ISubgraphIsomorphismSolver
    {
        Results Solve(Graph g1, Graph g2);
    }

    public static class Utils
    {
        public const string ExactMatchMessage = "The target contains an exact subgraph isomorphism of the pattern.";
        public const string ExactMappingDisplayMessage = "Exact mapping:";
        public const string ExactMatchFailureMessage = "The target does not contain a subgraph isomorphism of the pattern.";
        public const string GraphComplementDisplayMessage = "Target graph with missing edges restored:";
        public const string ApproximatedMappingDisplayMessage = "Approximated mapping:";
        public static void PrintMatrix(bool[,] matrix, System.IO.TextWriter writer)
        {
            var rows = matrix.GetLength(0);
            var cols = matrix.GetLength(1);
            for (var i = 0; i < rows; ++i)
            {
                for (var j = 0; j < cols; ++j)
                {
                    writer.Write(matrix[i, j] ? "1 " : "0 ");
                }
                writer.WriteLine();
            }
        }


        public static void StandardizePrintMatrixOptions(int[] matrix, int g2size, System.IO.TextWriter writer, SIOptions options)
        {
            PrintMatrixOptions(StandardiseMapping(matrix, g2size), writer, options);
        }

        public static bool[,] StandardiseMapping(int[] mapping, int g2size)
        {
            var standardisedMapping = new bool[mapping.Length, g2size];
            for (var i = 0; i < mapping.Length; ++i)
            {
                for(var j = 0; j < g2size; ++j)
                {
                    standardisedMapping[i, j] = (mapping[i] == j);
                }
            }
            return standardisedMapping;
        }

        public static void PrintMatrix(bool[,] matrix)
        {
            var rows = matrix.GetLength(0);
            var cols = matrix.GetLength(1);
            for (var i = 0; i < rows; ++i)
            {
                for (var j = 0; j < cols; ++j)
                {
                    Console.Write(matrix[i, j] ? "1 " : "0 ");
                }
                Console.WriteLine();
            }
        }



        public static void PrintMatrixOptions(bool[,] matrix, System.IO.TextWriter writer, SIOptions options)
        {
            PrintMatrix(matrix, writer);
            if(options.console) PrintMatrix(matrix);
        }


        public static void PrintMessageOptions(string message, System.IO.TextWriter writer, SIOptions options)
        {
            writer.WriteLine(message);
            if (options.console) Console.WriteLine(message);
        }

        public static (Graph p, Graph g) PrepGraphs(string inPath)
        {
            var (p, g) = Graph.ReadTwoFromFile(inPath);

            Graph g1, g2;
            if (p.size > g.size)
            {
                g2 = p;
                g1 = g;
            }
            else if (p.size == g.size)
            {
                if (p.EdgeCount() >= g.EdgeCount())
                {
                    g1 = p;
                    g2 = g;
                }
                else
                {
                    g1 = g;
                    g2 = p;
                }
            }
            else
            {
                g2 = g;
                g1 = p;
            }
            return (g1, g2);
        }
    }
}
