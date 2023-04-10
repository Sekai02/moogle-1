namespace MoogleEngine;

public static class Moogle
{

    #region STATIC_GLOBAL_PARAMETERS
    public static Document[] AllDocs = new Document[] { };
    public static Dictionary<string, int> DocumentsWithTerm = new Dictionary<string, int>();
    public static Dictionary<string, float> IDFVector = new Dictionary<string, float>();
    public static int NumberOfDocuments;
    #endregion

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
        Preproccess();

        Document queryDocument = new Document("", query);
        queryDocument.TF = new Dictionary<string, float>();
        queryDocument.TFIDF = new Dictionary<string, float>();
        TFIDFAnalyzer.CalculateTFVector(ref queryDocument);
        TFIDFAnalyzer.CalculateTFIDFVector(ref queryDocument);

        SearchItem[] items = new SearchItem[3] {
            new SearchItem("Hello World", "Lorem ipsum dolor sit amet", 0.9f),
            new SearchItem("Hello World", "Lorem ipsum dolor sit amet", 0.5f),
            new SearchItem("Hello World", "Lorem ipsum dolor sit amet", 0.1f),
        };

        return new SearchResult(items, query);
    }
}
