using System.CodeDom.Compiler;
using NDesk.Options;

namespace SubgraphIsomorphism.Utils
{
    public class SIOptions
    {
        public string inPath = "input.txt";
        public string outPath = "output.txt";
        public string inDirectory = "";
        public string outDirectory = "";
        public bool help = false;
        public bool exact = false;
        public bool approximate = false;
        public bool console = false;
        public bool append = false;
        public bool generate = false;
        public bool verbose = false;

        public SIOptions(string[] args)
        {
            var opts = new OptionSet() {
            { "input=", "specify input file (default: input.txt)", v => inPath = v },
            { "output=", "specify output file (default: output.txt)", v => {outPath = v; } },
            { "inputDir=", "specify input directory for batch solving", v => inDirectory = v },
            { "outputDir=", "specify output directory for batch solving", v => outDirectory = v },
            { "g|generate", "Generate random inputs for the program in outputDir, then exit the program without solving.", v =>  {generate = true; }},
            { "e|exact", "run the exact algorithm (Ullman)", v => { exact = true; } },
            { "a|approximate", "run the exact algorithm (Munkres modification)" ,v => { approximate = true; } },
            { "c|console", "print results to command line", v => console = true},
            { "p|append", "append results to output file if it already exists", v => append = true },
            { "v|verbose", "write processing time into output file", v => verbose = true },
            { "h|?|help", "show help",  v => help = v != null },
        };
            List<string> extra = opts.Parse(args);

            if(generate)
            {
                if( (exact || approximate || inDirectory != "") )
                {
                    throw new ArgumentException("The --generate option must be used alone, without other options.");
                }

                if (outDirectory == "")
                {
                    throw new ArgumentException("The --generate option requires the --outputDir option to specify where to save generated inputs.");
                }

                InputGenerator.InputGenerator.GenerateInputsForSize(outDirectory, 10, 1, 9);
                Environment.Exit(0);
            }


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
}
