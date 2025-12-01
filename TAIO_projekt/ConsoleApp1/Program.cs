using SubgraphIsomorphism.Munkres;
using SubgraphIsomorphism.Ullman;
using SubgraphIsomorphism.Utils;

internal class Program
{
    private static void Main(string[] args)
    {
        SolverManager manager = new(args);
        manager.SolveOptions();
    }
}