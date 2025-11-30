using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NDesk.Options;

namespace SubgraphIsomorphism
{
    public class SIOptions
    {
        public string inPath = "input.txt";
        public string outPath = "output.txt";
        public bool help = false;
        public bool exact = false;
        public bool approximate = false;
        public bool console = false;

        public SIOptions(string[] args)
        {
            var opts = new OptionSet() {
            { "input=", "specify input file (default: input.txt)", v => inPath = v },
            { "output=", "specify output file (default: output.txt)", v => {outPath = v; } },
            { "e|exact", "run the exact algorithm (Ullman)", v => { exact = true; } },
            { "a|approximate", "run the exact algorithm (Munkres modification)" ,v => { approximate = true; } },
            { "c|console", "print results to command line", v => console = true},
            { "h|?|help", "show help",  v => help = v != null },
        };
            List<string> extra = opts.Parse(args);

            if (help)
            {
                ShowHelp(opts);
                Environment.Exit(0);
            }

            if (!exact && !approximate)
            {
                throw new ArgumentException("At least one of --exact or --approximate options must be specified.");
            }
        }

        static void ShowHelp(OptionSet p)
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("The program solves the Subgraph Isomorphism Problem with either the Ullman or Munkres algorithms.");
            Console.WriteLine();
            Console.WriteLine("The following options can be used:");
            p.WriteOptionDescriptions(Console.Out);
        }
    }

    public static class Utils
    {
        public const string ExactMatchMessage = "Graf mniejszy jest podgrafem większego.";
        public const string ExactMappingDisplayMessage = "Dokładne mapowanie:";
        public const string ExactMatchFailureMessage = "Graf mniejszy nie jest podgrafem większego.";
        public const string GraphComplementDisplayMessage = "Graf po dodaniu brakujących krawędzi:";
        public const string ApproximatedMappingDisplayMessage = "Przybliżone mapowanie:";
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

        public static void PrintMatrix(int[,] matrix, System.IO.TextWriter writer)
        {
            var rows = matrix.GetLength(0);
            var cols = matrix.GetLength(1);
            for (var i = 0; i < rows; ++i)
            {
                for (var j = 0; j < cols; ++j)
                {
                    writer.Write(matrix[i, j] == 1? "1 " : "0 ");
                }
                writer.WriteLine();
            }
        }

        public static void StandardizePrintMatrixOptions(int[] matrix, int g2size, System.IO.TextWriter writer, SIOptions options)
        {
            PrintMatrixOptions(StandardiseMapping(matrix, g2size), writer, options);
        }

        public static int[,] StandardiseMapping(int[] mapping, int g2size)
        {
            var standardisedMapping = new int[mapping.Length, g2size];
            for (var i = 0; i < mapping.Length; ++i)
            {
                for(var j = 0; j < g2size; ++j)
                {
                    standardisedMapping[i, j] = (mapping[i] == j) ? 1 : 0;
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


        public static void PrintMatrix(int[,] matrix)
        {
            var rows = matrix.GetLength(0);
            var cols = matrix.GetLength(1);
            for (var i = 0; i < rows; ++i)
            {
                for (var j = 0; j < cols; ++j)
                {
                    Console.Write(matrix[i, j] == 1 ? "1 " : "0 ");
                }
                Console.WriteLine();
            }
        }
        public static void PrintMatrixOptions(bool[,] matrix, System.IO.TextWriter writer, SIOptions options)
        {
            PrintMatrix(matrix, writer);
            if(options.console) PrintMatrix(matrix);
        }

        public static void PrintMatrixOptions(int[,] matrix, System.IO.TextWriter writer, SIOptions options)
        {
            PrintMatrix(matrix, writer);
            if (options.console) PrintMatrix(matrix);
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
