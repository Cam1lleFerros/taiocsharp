using TAiO;

Console.WriteLine("Hello, World!");

var (p, g) = Graph.ReadTwoFromFile("input.txt");

var ullman = new ConsoleApp1.Ullman.Ullman(p, g);

var result = ullman.FindIsomorphism();


Console.WriteLine("Mapping result:");
if(result == null)
{
    Console.WriteLine("No isomorphism found.");
}
else{
    for (int i = 0; i<result.GetLength(0); i++)
    {
        for (int j = 0; j < result.GetLength(1); j++)
        {
            Console.Write(result[i, j] ? "1 " : "0 ");
        }
        Console.WriteLine();
    }
}