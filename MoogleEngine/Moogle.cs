namespace MoogleEngine;

public static class Moogle
{

    #region STATIC_GLOBAL_PARAMETERS

    public static Document[] AllDocs = new Document[] { };
    public static Dictionary<string, int> DocumentsWithTerm = new Dictionary<string, int>();
    public static Dictionary<string, float> IDFVector = new Dictionary<string, float>();
    public static int NumberOfDocuments;

    #endregion

    static Moogle()
    {
        Preproccess();
    }

    public static void Preproccess()
    {
        AllDocs = DocumentCatcher.ReadDocumentsFromFolder(@"../Content");
        NumberOfDocuments = AllDocs.Length;
        DocumentsWithTerm = new Dictionary<string, int>();
        IDFVector = new Dictionary<string, float>();

        for (int i = 0; i < NumberOfDocuments; i++)
        {
            AllDocs[i].TF = new Dictionary<string, float>();
            AllDocs[i].TFIDF = new Dictionary<string, float>();
        }

        TFIDFAnalyzer.CalculateDocumentsWithTerm();
        TFIDFAnalyzer.CalculateIDFVector();
        for (int i = 0; i < NumberOfDocuments; i++)
        {
            TFIDFAnalyzer.CalculateTFVector(ref AllDocs[i]);
        }
        for (int i = 0; i < NumberOfDocuments; i++)
        {
            TFIDFAnalyzer.CalculateTFIDFVector(ref AllDocs[i]);
        }
    }

    public static SearchResult Query(string query)
    {
        Document queryDocument = new Document("", query);
        foreach (var word in queryDocument.Words)
        {
            Console.WriteLine(word);
        }

        queryDocument.TF = new Dictionary<string, float>();
        queryDocument.TFIDF = new Dictionary<string, float>();
        TFIDFAnalyzer.CalculateTFVector(ref queryDocument);
        TFIDFAnalyzer.CalculateTFIDFVector(ref queryDocument);

        List<SearchItem> results = new List<SearchItem>();

        for (int i = 0; i < NumberOfDocuments; i++)
        {
            float score = TFIDFAnalyzer.ComputeRelevance(queryDocument, AllDocs[i]);

            if (score == 0.0f)
            {
                continue;
            }

            results.Add(new SearchItem(AllDocs[i].Title, "no snippet", score));
        }

        SearchItem[] items = results.ToArray();

        Array.Sort(items);
        Array.Reverse(items);

        return new SearchResult(items, query);
    }
}
