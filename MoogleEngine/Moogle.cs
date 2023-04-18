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
    public static Document queryDocument = new Document("");
    public static List<string> MandatoryWords = new List<string>();
    public static List<string> ExcludedWords = new List<string>();
    public static Dictionary<string, float> NewRelevance = new Dictionary<string, float>();
    public static Dictionary<string, int> NumberOfAsters = new Dictionary<string, int>();

    #endregion

    #region CONSTANTS_FOR_BLAZOR

    public const string INVALID_QUERY = "Su búsqueda debe contener palabras, pruebe cambiar por una válida y reintente.";
    public static bool MOOGLE_LOADING = false;

    #endregion

    #region SNIPPET_CONSTANTS

    const int CHARS_TO_LEFT = 20;
    const int CHARS_TO_RIGHT = 200;


    #endregion

    static Moogle()
    {
        int startTime = Environment.TickCount;
        Console.WriteLine("⏳Moogle ha comenzado a cargar");

        Preproccess();

        Console.WriteLine("⏳Moogle ha terminado de cargar");
        int endTime = Environment.TickCount;
        float time = (float)(endTime - startTime) / 1000.0f;
        Console.WriteLine("⏰Tiempo de carga {0}s", time);
    }

    static void Preproccess()
    {
        AllDocs = DocumentCatcher.ReadDocumentsFromFolder(@"../Content");
        NumberOfDocuments = AllDocs.Length;
        DocumentsWithTerm = new Dictionary<string, int>();
        IDFVector = new Dictionary<string, float>();
        queryDocument = new Document("");

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
        }
    }

    static void ResetGlobalVariables()
    {
        MandatoryWords = new List<string>();
        ExcludedWords = new List<string>();
        NumberOfAsters = new Dictionary<string, int>();
        NewRelevance = new Dictionary<string, float>();
    }

    static void CalculateNewRelevance()
    {
        foreach (var word in NumberOfAsters)
        {
            if (DocumentCatcher.InvalidWord(word.Key)) continue;

            int numberOfAsters = word.Value;

            if (numberOfAsters > 0)
            {
                float newRelevance = 1.0f;

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
    }

    static void UpdateNewRelevance()
    {
        foreach (var word in NewRelevance)
        {
            if (DocumentCatcher.InvalidWord(word.Key)) continue;

            if (queryDocument.TFIDF.ContainsKey(word.Key))
            {
                queryDocument.TFIDF[word.Key] = word.Value;
            }
        }
    }

    static void FindSearchResults(List<SearchItem> results)
    {
        string[] words = queryDocument.LowerizedText.Split(DocumentCatcher.Delims).Select(p => p.Trim()).ToArray();

        for (int i = 0; i < NumberOfDocuments; i++)
        {
            bool containsInvalidWords = false;
            bool containsValidWords = true;

            foreach (var word in ExcludedWords)
            {
                if (AllDocs[i].WordFrequency.ContainsKey(word))
                {
                    containsInvalidWords = true;
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
                string snippet = BuildSnippet(words, AllDocs[i].Text, AllDocs[i].LowerizedText);
                if (snippet != "no snippet")
                {
                    results.Add(new SearchItem(AllDocs[i].Title, snippet, score));
                }
            }
        }
    }

    static string BuildSnippet(string[] words, string originalText, string lowerizedText)
    {
        string snippet = "";

        foreach (string word in words)
        {
            if (DocumentCatcher.InvalidWord(word))
            {
                continue;
            }

            string pattern = String.Format(@"\b{0}\b", word);
            var match = System.Text.RegularExpressions.Regex.Match(lowerizedText, pattern);

            if (match.Success)
            {
                int indexOfWord = match.Index;

                int l = Math.Max(0, indexOfWord - CHARS_TO_LEFT);
                for (int i = l; i < indexOfWord; i++)
                {
                    snippet += originalText[i];
                }

                int r = Math.Max(0, Math.Min(originalText.Length - 1, indexOfWord + CHARS_TO_RIGHT - 1));
                for (int i = indexOfWord; i <= r; i++)
                {
                    snippet += originalText[i];
                }

                return snippet;
            }
        }

        return "no snippet";
    }

    static void FindWordsToBeNear()
    {
        List<string> wordList = queryDocument.Words.ToList();

        bool emptyStringsRemaining = false;

        do
        {
            emptyStringsRemaining = wordList.Remove("");
        }
        while (emptyStringsRemaining);

        foreach (string word in wordList)
        {
            Console.WriteLine(word);
        }
        Console.WriteLine();
    }

    public static SearchResult Query(string query)
    {
        //Checks if the given query is valid
        if (!Document.ValidQuery(query))
        {
            return new SearchResult(new SearchItem[] { }, INVALID_QUERY);
        }

        ResetGlobalVariables();
        queryDocument = new Document(query);    //Reset queryDocument

        Console.WriteLine("Words");
        foreach (var word in queryDocument.Words)
        {
            Console.WriteLine(word);
        }
        Console.WriteLine();

        Console.WriteLine(value: "Word Frequency");
        foreach (var word in queryDocument.WordFrequency)
        {
            Console.WriteLine("{0} {1}", word.Key, word.Value);
        }
        Console.WriteLine();

        Console.WriteLine("TFIDF");
        foreach (var word in queryDocument.TFIDF)
        {
            Console.WriteLine("{0} {1}", word.Key, word.Value);
        }
        Console.WriteLine();

        FindWordsToBeNear();

        CalculateNewRelevance();                //Calculate new TFIDF based on number of * on input
        UpdateNewRelevance();                   //Update previous values

        List<SearchItem> results = new List<SearchItem>();

        FindSearchResults(results);             //Calculates results of search based on relevance
        SearchItem[] items = results.ToArray();
        Array.Sort(items);
        Array.Reverse(items);

        return new SearchResult(items, query);
    }
}
