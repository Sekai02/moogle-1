namespace MoogleEngine;

public static class TFIDF
{
    public static int NumberOfDocuments;
    public static Dictionary<string, int> DocumentsWithTerm = new Dictionary<string, int>();

    public static double CalculateTermFrequency(Dictionary<string, int> doc, string term)
    {
        if (!doc.ContainsKey(term))
        {
            return 0;
        }
        else
        {
            return (double)doc[term] / doc.Count;
        }
    }

    public static double CalculateInverseDocumentFrequency(string term)
    {
        if (!DocumentsWithTerm.ContainsKey(term))
        {
            return 0;
        }
        else
        {
            return Math.Log((double)DocumentsWithTerm.Count / DocumentsWithTerm[term]);
        }
    }
}