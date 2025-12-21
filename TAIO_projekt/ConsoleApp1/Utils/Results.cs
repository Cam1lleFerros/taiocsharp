namespace SubgraphIsomorphism.Utils;

public class Results
{
    public bool[,] Mapping { get; init; }
    public int? MissingEdges { get; init; }
    public bool IsExact { get; init; }
    public Graph? GraphComplement { get; init; }
    public double? EditDistance { get; init; }

    public Results(bool[,] mapping, int? missingEdges, bool isExact, Graph? graphComplement, double? editDistance = null)
    {
        Mapping = mapping;
        MissingEdges = missingEdges;
        IsExact = isExact;
        GraphComplement = graphComplement;
        EditDistance = editDistance;
    }

    public Results(int[] mapping, int? missingEdges, bool isExact, Graph? graphComplement, int g2size, double? editDistance = null)
    {
        Mapping = PrintGraphUtils.StandardiseMapping(mapping, g2size);
        MissingEdges = missingEdges;
        IsExact = isExact;
        GraphComplement = graphComplement;
        EditDistance = editDistance;
    }
}
