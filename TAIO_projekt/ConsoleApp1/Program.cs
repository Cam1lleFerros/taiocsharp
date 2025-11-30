using System.Security.Cryptography.X509Certificates;
using TAiO;

var (p, g) = Graph.ReadTwoFromFile("input.txt");

Graph g1, g2;
if (p.size > g.size)
{
    g2 = p;
    g1 = g;
}
else if (p.size == g.size)
{
    if (p.EdgeCount() >= g.EdgeCount())
    {
        g1 = p;
        g2 = g;
    }
    else
    {
        g1 = g;
        g2 = p;
    }
}
else 
{
    g2 = g;
    g1 = p;
}


bool exact = false; // Potrzebujemy jakoś to wczytywać na wejściu
using var writer = File.CreateText("output.txt");

if (exact)
{
    var ullman = new Ullman(g1, g2);
    var (result, matrix) = ullman.FindIsomorphism();
    if (result)
    {
        writer.WriteLine("Graf mniejszy jest podgrafem większego. Dokładne mapowanie:");
        // Jakoś wydrukuj tą macierz
    }
    else
    {
        // Wywował metodę extend z Ullmana
        // Wydrukuj nowy graf
    }
}
else
{
    var 
}
