namespace SubgraphIsomorphism.Utils;

public class ConsoleUtils
{
    public static string PrepareDeletionString(int length)
    {
        string del = String.Empty;
        for (var i = 0; i < length; ++i)
            del += '\b';
        return del;
    }
}
