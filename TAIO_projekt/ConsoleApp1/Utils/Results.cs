namespace SubgraphIsomorphism.Utils;

public class Results
{
    public bool[,] Mapping { get; init; }
    public int? MissingEdges { get; init; }
    public bool IsExact { get; init; }
    public Graph? GraphComplement { get; init; }

    public Results(bool[,] mapping, int? missingEdges, bool isExact, Graph? graphComplement)
    {
        Mapping = mapping;
        MissingEdges = missingEdges;
        IsExact = isExact;
        GraphComplement = graphComplement;
    }

    public Results(int[] mapping, int? missingEdges, bool isExact, Graph? graphComplement, int g2size)
    {
        Mapping = PrintGraphUtils.StandardiseMapping(mapping, g2size);
        MissingEdges = missingEdges;
        IsExact = isExact;
        GraphComplement = graphComplement;
    }
}
