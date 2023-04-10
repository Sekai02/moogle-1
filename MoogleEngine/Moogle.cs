namespace MoogleEngine;

public static class Moogle
{

    #region STATIC_GLOBAL_PARAMETERS
    public static Document[] AllDocs = new Document[] { };
    public static Dictionary<string, int> DocumentsWithTerm = new Dictionary<string, int>();
    public static Dictionary<string, float> IDFVector = new Dictionary<string, float>();
    public static Dictionary<string, float>[] TFVectors = new Dictionary<string, float>[] { };
    public static Dictionary<string, float>[] TFIDFVectors = new Dictionary<string, float>[] { };
    public static int NumberOfDocuments;
    #endregion

    public static void Preproccess()
    {
        AllDocs = DocumentCatcher.ReadDocumentsFromFolder(@"../Content");
        NumberOfDocuments = AllDocs.Length;
        DocumentsWithTerm = new Dictionary<string, int>();
        IDFVector = new Dictionary<string, float>();
        TFVectors = new Dictionary<string, float>[NumberOfDocuments];
        TFIDFVectors = new Dictionary<string, float>[NumberOfDocuments];

        for (int i = 0; i < NumberOfDocuments; i++)
        {
            TFVectors[i] = new Dictionary<string, float>();
            TFIDFVectors[i] = new Dictionary<string, float>();
        }

        TFIDFAnalyzer.CalculateDocumentsWithTerm();
        TFIDFAnalyzer.CalculateIDFVector();
        TFIDFAnalyzer.CalculateTFVectors();
        TFIDFAnalyzer.CalculateTFIDFVectors();
    }

    public static SearchResult Query(string query)
    {
        Preproccess();

        string[] QueryWords = query.ToLower().Split(DocumentCatcher.Delims).Select(p => p.Trim()).ToArray();
        Dictionary<string, int> QueryWordFrequency = new Dictionary<string, int>();
        Dictionary<string, float> QueryTFVector = new Dictionary<string, float>();
        Dictionary<string, float> QueryTFIDFVector = new Dictionary<string, float>();

        foreach (var word in QueryWords)
        {
            if (!QueryWordFrequency.ContainsKey(word))
            {
                QueryWordFrequency.Add(word, 1);
            }
            else
            {
                QueryWordFrequency[word]++;
            }
        }

        

        SearchItem[] items = new SearchItem[3] {
            new SearchItem("Hello World", "Lorem ipsum dolor sit amet", 0.9f),
            new SearchItem("Hello World", "Lorem ipsum dolor sit amet", 0.5f),
            new SearchItem("Hello World", "Lorem ipsum dolor sit amet", 0.1f),
        };

        return new SearchResult(items, query);
    }
}
