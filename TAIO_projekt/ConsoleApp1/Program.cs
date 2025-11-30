using TAiO;

var (p, g) = Graph.ReadTwoFromFile("input.txt");

var ullman = new ConsoleApp1.Ullman.Ullman(p, g);

var result = ullman.FindIsomorphism();

using var writer = File.CreateText("output.txt");

writer.WriteLine("Mapping result:");

if (result == null)
    writer.WriteLine("No isomorphism found.");
else
{
    for (var i = 0; i < result.GetLength(0); ++i)
    {
        for (var j = 0; j < result.GetLength(1); ++j)
            writer.Write(result[i, j] ? "1 " : "0 ");
        writer.WriteLine();
    }
}
