namespace SubgraphIsomorphism.InputGenerator;

public class InputGenerator
{
    public static void GenerateInputsForSize(string directoryPath, int targetSize = 100, int startPatternSize = 10, int endPatternSize = 99, int patternStep = 1, double edgeProbability = 0.5)
    {
        for (var patternSize = startPatternSize; patternSize <= endPatternSize; patternSize += patternStep)
        {
            var path = Path.Combine(directoryPath, $"input_pattern{patternSize}_target{targetSize}.txt");
            GenerateRandomInput(path, patternSize, targetSize, edgeProbability);
            var pathIso = Path.Combine(directoryPath, $"input_iso_pattern{patternSize}_target{targetSize}.txt");
            GenerateSubgraphIsomorphism(pathIso, patternSize, targetSize, edgeProbability);
        }
    }

    public static void GenerateMultipleFiles(string directoryPath, int numberOfFiles = 15, int patternSize = 25, int targetSize = 100, double edgeProbability = 0.5)
    {
        for (var i = 0; i < numberOfFiles; i += 2)
        {
            var path = Path.Combine(directoryPath, $"input_{i + 1}.txt");
            GenerateRandomInput(path, patternSize, targetSize, edgeProbability);
            var pathIso = Path.Combine(directoryPath, $"input_{i + 2}.txt");
            GenerateSubgraphIsomorphism(pathIso, patternSize, targetSize, edgeProbability);
        }
    }
    public static void GenerateRandomInput(string path, int patternSize, int targetSize, double edgeProbability)
    {
        if (edgeProbability < 0 || edgeProbability > 1)
            throw new ArgumentException("Edge probability must be between 0 and 1.");

        var rand = new Random();
        var patternAdjMat = new bool[patternSize, patternSize];
        var targetAdjMat = new bool[targetSize, targetSize];
        for (var i = 0; i < patternSize; i++)
        {
            for (var j = 0; j < patternSize; j++)
                patternAdjMat[i, j] = i != j && rand.NextDouble() < edgeProbability;
        }
        for (var i = 0; i < targetSize; i++)
        {
            for (var j = 0; j < targetSize; j++)
                targetAdjMat[i, j] = i != j && rand.NextDouble() < edgeProbability;
        }
        using var writer = new StreamWriter(path);
        writer.WriteLine($"{patternSize}");
        for (var i = 0; i < patternSize; i++)
        {
            for (var j = 0; j < patternSize; j++)
                writer.Write(patternAdjMat[i, j] ? "1 " : "0 ");
            writer.WriteLine();
        }
        writer.WriteLine($"{targetSize}");
        for (var i = 0; i < targetSize; i++)
        {
            for (var j = 0; j < targetSize; j++)
                writer.Write(targetAdjMat[i, j] ? "1 " : "0 ");
            writer.WriteLine();
        }
    }

    public static Dictionary<int, int> GenerateRandomMapping(int patternSize, int targetSize)
    {
        var rand = new Random();
        var mapping = new Dictionary<int, int>();
        var usedTargetNodes = new HashSet<int>();
        for (var i = 0; i < patternSize; i++)
        {
            int targetNode;
            do
            {
                targetNode = rand.Next(0, targetSize);
            } while (usedTargetNodes.Contains(targetNode));
            mapping[i] = targetNode;
            usedTargetNodes.Add(targetNode);
        }
        return mapping;
    }

    public static void GenerateSubgraphIsomorphism(string path, int patternSize, int targetSize, double edgeProbability)
    {
        var rand = new Random();
        var patternAdjMat = new bool[patternSize, patternSize];
        var targetAdjMat = new bool[targetSize, targetSize];
        for (var i = 0; i < patternSize; i++)
        {
            for (var j = 0; j < patternSize; j++)
                patternAdjMat[i, j] = i != j && rand.NextDouble() < edgeProbability;

        }

        var mapping = GenerateRandomMapping(patternSize, targetSize);

        for (var i = 0; i < targetSize; i++)
        {
            for (var j = 0; j < targetSize; j++)
                targetAdjMat[i, j] = i != j && rand.NextDouble() < edgeProbability;
        }

        for (var i = 0; i < patternSize; i++)
        {
            for (var j = 0; j < patternSize; j++)
            {
                if (i != j)
                {
                    if (patternAdjMat[i, j])
                    {
                        var targetI = mapping[i];
                        var targetJ = mapping[j];
                        targetAdjMat[targetI, targetJ] = true;
                    }
                }

            }
        }

        using var writer = new StreamWriter(path);
        writer.WriteLine($"{patternSize}");
        for (var i = 0; i < patternSize; i++)
        {
            for (var j = 0; j < patternSize; j++)
                writer.Write(patternAdjMat[i, j] ? "1 " : "0 ");
            writer.WriteLine();
        }
        writer.WriteLine($"{targetSize}");
        for (var i = 0; i < targetSize; i++)
        {
            for (var j = 0; j < targetSize; j++)
                writer.Write(targetAdjMat[i, j] ? "1 " : "0 ");
            writer.WriteLine();
        }

        var pathMapping = path.Replace(".txt", "_mapping.txt");
        using var writerMapping = new StreamWriter(pathMapping);
        for (var i = 0; i < patternSize; i++)
            writerMapping.WriteLine($"{i} -> {mapping[i]}");
    }
}
