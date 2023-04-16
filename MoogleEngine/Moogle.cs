namespace MoogleEngine;

public static class Moogle
{

    #region STATIC_GLOBAL_PROPERTIES

    //For All Documents
    public static Document[] AllDocs = new Document[] { };
    public static Dictionary<string, int> DocumentsWithTerm = new Dictionary<string, int>();
    public static Dictionary<string, float> IDFVector = new Dictionary<string, float>();
    public static int NumberOfDocuments;

    //For Query
    public static float maxDocumentTFIDF = 0.0f;
    public static List<string> MandatoryWords = new List<string>();
    public static List<string> ExcludedWords = new List<string>();
    public static Dictionary<string, float> NewRelevance = new Dictionary<string, float>();
    public static Dictionary<string, int> NumberOfAsters = new Dictionary<string, int>();

    #endregion

    #region CONSTANTS_FOR_BLAZOR

    public const string INVALID_QUERY = "Su búsqueda debe contener palabras, pruebe cambiar por una válida y reintente.";

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
        maxDocumentTFIDF = 0.0f;

        for (int i = 0; i < NumberOfDocuments; i++)
        {
            AllDocs[i].TF = new Dictionary<string, float>();
            AllDocs[i].TFIDF = new Dictionary<string, float>();
        }

        TFIDFAnalyzer.CalculateDocumentsWithTerm();
        TFIDFAnalyzer.CalculateIDFVector();
        for (int i = 0; i < NumberOfDocuments; i++)
        {
            TFIDFAnalyzer.CalculateTFVector(AllDocs[i]);
        }
        for (int i = 0; i < NumberOfDocuments; i++)
        {
            TFIDFAnalyzer.CalculateTFIDFVector(AllDocs[i]);

            //Calulate Max TFIDF from all documents
            foreach (var word in AllDocs[i].TFIDF)
            {
                maxDocumentTFIDF = Math.Max(maxDocumentTFIDF, word.Value);
            }
        }
    }

    public static SearchResult Query(string query)
    {
        //Checks if the given query is valid
        if (!Document.ValidQuery(query))
        {
            return new SearchResult(new SearchItem[] { }, INVALID_QUERY);
        }

        #region RESET_GLOBAL_VARIABLES

        MandatoryWords = new List<string>();
        ExcludedWords = new List<string>();
        NumberOfAsters = new Dictionary<string, int>();
        NewRelevance = new Dictionary<string, float>(); 

        #endregion

        #region RESET_QUERY_VARIABLES

        Document queryDocument = new Document(query);

        /*queryDocument.TF = new Dictionary<string, float>();
        queryDocument.TFIDF = new Dictionary<string, float>();
        TFIDFAnalyzer.CalculateTFVector(queryDocument);
        TFIDFAnalyzer.CalculateTFIDFVector(queryDocument);*/              

        #endregion

        #region FIND_MAX_TF_IDF 

        float maxTFIDF = maxDocumentTFIDF;

        foreach (var word in queryDocument.TFIDF)
        {
            maxTFIDF = Math.Max(maxTFIDF, word.Value);
        }

        #endregion

        #region CALCULATE_NEW_RELEVANCE_BASED_ON_*_OPERATORS_ON_INPUT
        
        foreach (var word in NumberOfAsters)
        {
            int numberOfAsters = word.Value;

            if (numberOfAsters > 0)
            {
                float newRelevance = maxTFIDF;

                while (numberOfAsters > 0)
                {
                    newRelevance *= 2.0f;
                    numberOfAsters--;
                }

                if (!NewRelevance.ContainsKey(word.Key))
                {
                    NewRelevance.Add(word.Key, newRelevance);
                }
                else
                {
                    NewRelevance[word.Key] = newRelevance;
                }
            }
        }

        #endregion

        #region UPDATE_NEW_RELEVANCE_ON_TFIDF

        foreach (var word in NewRelevance)
        {
            if (queryDocument.TFIDF.ContainsKey(word.Key))
            {
                queryDocument.TFIDF[word.Key] = word.Value;
            }
        }

        #endregion

        #region  FIND_SEARCH_RESULTS

        List<SearchItem> results = new List<SearchItem>();

        for (int i = 0; i < NumberOfDocuments; i++)
        {
            bool containsInvalidWords = false;
            bool containsValidWords = true;

            foreach (var word in ExcludedWords)
            {
                if (AllDocs[i].WordFrequency.ContainsKey(word))
                {
                    containsInvalidWords = false;
                    break;
                }
            }
            foreach (var word in MandatoryWords)
            {
                if (!AllDocs[i].WordFrequency.ContainsKey(word))
                {
                    containsValidWords = false;
                    break;
                }
            }

            float score = TFIDFAnalyzer.ComputeRelevance(queryDocument, AllDocs[i]);

            if (score == 0.0f)
            {
                continue;
            }

            if (containsValidWords && !containsInvalidWords)
            {
                results.Add(new SearchItem(AllDocs[i].Title, "no snippet", score));
            }
        }

        SearchItem[] items = results.ToArray();

        Array.Sort(items);
        Array.Reverse(items);

        #endregion

        return new SearchResult(items, query);
    }
}
