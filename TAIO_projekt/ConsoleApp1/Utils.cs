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


        public SIOptions(string[] args)
        {
            var opts = new OptionSet() {
            { "input=", "specify input file (default: input.txt)", v => inPath = v },
            { "output=", "specify output file (default: output.txt)", v => outPath = v },
            { "e|exact", "run the exact algorithm (Ullman)", v => { exact = true; } },
            { "a|approximate", "run the exact algorithm (Munkres modification)" ,v => { approximate = true; } },
            { "h|?|help", "show help",  v => help = v != null },
        };
            List<string> extra = opts.Parse(args);

            if (!exact && !approximate)
            {
                throw new ArgumentException("At least one of --exact or --approximate options must be specified.");
            }
        }
    }
    public static class Utils
    {
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
