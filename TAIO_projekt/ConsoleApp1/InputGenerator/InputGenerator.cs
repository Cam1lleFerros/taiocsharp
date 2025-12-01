using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubgraphIsomorphism.InputGenerator
{
    public class InputGenerator
    {
        public static void GenerateInputsForSize(string directoryPath, int targetSize=100, int startPatternSize=10, int endPatternSize = 99, int patternStep = 1, double edgeProbability = 0.5)
        {
            for (int patternSize = startPatternSize; patternSize <= endPatternSize; patternSize+=patternStep)
            {
                string path = Path.Combine(directoryPath, $"input_pattern{patternSize}_target{targetSize}.txt");
                GenerateRandomInput(path, patternSize, targetSize, edgeProbability);
                string pathIso = Path.Combine(directoryPath, $"input_iso_pattern{patternSize}_target{targetSize}.txt");
                GenerateSubgraphIsomorphism(pathIso, patternSize, targetSize, edgeProbability);
            }
        }

        public static void GenerateMultipleFiles(string directoryPath, int numberOfFiles = 15, int patternSize = 25, int targetSize = 100, double edgeProbability = 0.5)
        {
            for (int i = 0; i < numberOfFiles; i+=2)
            {
                string path = Path.Combine(directoryPath, $"input_{i + 1}.txt");
                GenerateRandomInput(path, patternSize, targetSize, edgeProbability);
                string pathIso = Path.Combine(directoryPath, $"input_{i + 2}.txt");
                GenerateSubgraphIsomorphism(pathIso, patternSize, targetSize, edgeProbability);
            }
        }
        public static void GenerateRandomInput(string path, int patternSize, int targetSize, double edgeProbability)
        {
            if (edgeProbability < 0 || edgeProbability > 1)
                throw new ArgumentException("Edge probability must be between 0 and 1.");

            var rand = new Random();
            bool[,] patternAdjMat = new bool[patternSize, patternSize];
            bool[,] targetAdjMat = new bool[targetSize, targetSize];
            for (int i = 0; i < patternSize; i++)
            {
                for (int j = 0; j < patternSize; j++)
                {
                    if (i != j && rand.NextDouble() < edgeProbability)
                    {
                        patternAdjMat[i, j] = true;
                    }
                    else
                    {
                        patternAdjMat[i, j] = false;
                    }
                }
            }
            for (int i = 0; i < targetSize; i++)
            {
                for (int j = 0; j < targetSize; j++)
                {
                    if (i != j && rand.NextDouble() < edgeProbability)
                    {
                        targetAdjMat[i,j] = true;
                    }
                    else
                    {
                        targetAdjMat[i, j] = false;
                    }
                }
            }
            using var writer = new StreamWriter(path);
            writer.WriteLine($"{patternSize}");
            for(int i = 0; i < patternSize; i++)
            {
                for (int j = 0; j < patternSize; j++)
                {
                    writer.Write(patternAdjMat[i, j] ? "1 " : "0 ");
                }
                writer.WriteLine();
            }
            writer.WriteLine($"{targetSize}");
            for (int i = 0; i < targetSize; i++)
            {
                for (int j = 0; j < targetSize; j++)
                {
                    writer.Write(targetAdjMat[i, j] ? "1 " : "0 ");
                }
                writer.WriteLine();
            }
        }

        public static Dictionary<int, int> GenerateRandomMapping(int patternSize, int targetSize)
        {
            var rand = new Random();
            var mapping = new Dictionary<int, int>();
            var usedTargetNodes = new HashSet<int>();
            for (int i = 0; i < patternSize; i++)
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
            bool[,] patternAdjMat = new bool[patternSize, patternSize];
            bool[,] targetAdjMat = new bool[targetSize, targetSize];
            for (int i = 0; i < patternSize; i++)
            {
                for (int j = 0; j < patternSize; j++)
                {
                    if (i != j && rand.NextDouble() < edgeProbability)
                    {
                        patternAdjMat[i, j] = true;
                    }
                    else
                    {
                        patternAdjMat[i, j] = false;
                    }
                }
            }

            var mapping = GenerateRandomMapping(patternSize, targetSize);

            for (int i = 0; i < targetSize; i++)
            {
                for (int j = 0; j < targetSize; j++)
                {
                    if (i != j && rand.NextDouble() < edgeProbability)
                    {
                        targetAdjMat[i, j] = true;
                    }
                    else
                    {
                        targetAdjMat[i, j] = false;
                    }
                }
            }

            for (int i = 0; i < patternSize; i++)
            {
                for (int j = 0; j < patternSize; j++)
                {
                    if (i != j)
                    {
                        if (patternAdjMat[i, j])
                        {
                            int targetI = mapping[i];
                            int targetJ = mapping[j];
                            targetAdjMat[targetI, targetJ] = true;
                        }
                    }

                }
            }

            using var writer = new StreamWriter(path);
            writer.WriteLine($"{patternSize}");
            for (int i = 0; i < patternSize; i++)
            {
                for (int j = 0; j < patternSize; j++)
                {
                    writer.Write(patternAdjMat[i, j] ? "1 " : "0 ");
                }
                writer.WriteLine();
            }
            writer.WriteLine($"{targetSize}");
            for (int i = 0; i < targetSize; i++)
            {
                for (int j = 0; j < targetSize; j++)
                {
                    writer.Write(targetAdjMat[i, j] ? "1 " : "0 ");
                }
                writer.WriteLine();
            }

            string pathMapping = path.Replace(".txt", "_mapping.txt");
            using var writerMapping = new StreamWriter(pathMapping);
            for (int i = 0; i < patternSize; i++)
            {
                writerMapping.WriteLine($"{i} -> {mapping[i]}");
            }
        }
    }
}
