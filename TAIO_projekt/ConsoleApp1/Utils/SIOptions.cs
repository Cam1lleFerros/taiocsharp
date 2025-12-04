using NDesk.Options;

namespace SubgraphIsomorphism.Utils;

public class SIOptions
{
    public string inPath = "input.txt";
    public string outPath = "output.txt";
    public string inDirectory = String.Empty;
    public string outDirectory = String.Empty;
    public string seedPath = String.Empty;
    public bool help = false;
    public bool exact = false;
    public bool approximate = false;
    public bool console = false;
    public bool append = false;
    public bool generate = false;
    public bool verbose = false;
    public bool clean = false;

    public SIOptions(string[] args)
    {
        var opts = new OptionSet() {
        { "input=", "specify input file (default: input.txt)", v => inPath = v },
        { "output=", "specify output file (default: output.txt)", v => {outPath = v; } },
        { "inputDir=", "specify input directory for batch solving", v => inDirectory = v },
        { "outputDir=", "specify output directory for batch solving", v => outDirectory = v },
        { "seed=", "seed file for input generation. A seed file needs to contain five values, separated with spaces: target size, pattern size minimum and maximum, pattern step, and edge probability", v => seedPath = v },
        { "g|generate", "Generate random inputs for the program in outputDir, then exit the program without solving.", v =>  {generate = true; }},
        { "e|exact", "run the exact algorithm (Ullman)", v => { exact = true; } },
        { "a|approximate", "run the approximate algorithm (Munkres modification)" ,v => { approximate = true; } },
        { "c|console", "print results to command line", v => console = true},
        { "p|append", "append results to output file if it already exists", v => append = true },
        { "v|verbose", "write processing time into output file", v => verbose = true },
        { "l|clean", "clean input directory before generating", v => clean = true },
        { "h|?|help", "show help",  v => help = v != null },

    };
        List<string> extra = opts.Parse(args);

        if (help)
        {
            ShowHelp(opts);
            Environment.Exit(0);
        }

        if (generate)
        {
            if (exact || approximate || !String.IsNullOrEmpty(inDirectory))
                throw new ArgumentException("The --generate option must be used alone, without other options.");

            if (String.IsNullOrEmpty(outDirectory))
                throw new ArgumentException("The --generate option requires the --outputDir option to specify where to save generated inputs.");

            if (clean && Directory.Exists(outDirectory))
            {
                Directory.Delete(outDirectory, true);
                Directory.CreateDirectory(outDirectory);
            }

            if (!String.IsNullOrEmpty(seedPath))
            {
                var seedContent = File.ReadAllText(seedPath).Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (seedContent.Length != 5)
                    throw new ArgumentException("Seed file must contain exactly five values: target size, " +
                        "pattern size minimum and maximum, pattern step, and edge probability.");
                var targetSize = int.Parse(seedContent[0]);
                var patternSizeMin = int.Parse(seedContent[1]);
                var patternSizeMax = int.Parse(seedContent[2]);
                var patternStep = int.Parse(seedContent[3]);
                var edgeProbability = double.Parse(seedContent[4]);
                InputGenerator.InputGenerator.GenerateInputsForSize(outDirectory, targetSize, patternSizeMin,
                    patternSizeMax, patternStep, edgeProbability);
            }
            else
                InputGenerator.InputGenerator.GenerateInputsForSize(outDirectory, 10, 1, 9, 1);
            Console.WriteLine("Input generation completed.");
            Environment.Exit(0);
        }

        if (!exact && !approximate)
            throw new ArgumentException("At least one of --exact or --approximate options must be specified.");
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
