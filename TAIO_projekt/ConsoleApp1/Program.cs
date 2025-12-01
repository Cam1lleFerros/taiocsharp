using SubgraphIsomorphism.Munkres;
using SubgraphIsomorphism;
using SubgraphIsomorphism.Ullman;

internal class Program
{
    private static void Main(string[] args)
    {
        SolverManager manager = new(args);
        manager.SolveAll();
    }
}