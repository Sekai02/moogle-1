namespace MoogleEngine;

//Class to manage search engine logic
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
    //Used to store words with * operator
    public static List<string> MandatoryWords = new List<string>();
    //Used to store words with ! operator
    public static List<string> ExcludedWords = new List<string>();
    public static Dictionary<string, float> NewRelevance = new Dictionary<string, float>();
    public static Dictionary<string, int> NumberOfAsters = new Dictionary<string, int>();

    //For ~ operator
    static float MaxLogDoc;
    static List<Tuple<string, string>> WordsToBeNear = new List<Tuple<string, string>>();

    #endregion

    #region CONSTANTS_FOR_BLAZOR

    public const string INVALID_QUERY = "Su búsqueda debe contener palabras, pruebe cambiar por una válida y reintente.";
    public static bool MOOGLE_LOADING = false;

    #endregion

    #region SNIPPET_CONSTANTS

    const int CHARS_TO_LEFT = 20;
    const int CHARS_TO_RIGHT = 200;

    #endregion

    //Static constructor to initialize the Search Engine
    static Moogle()
    {
        int startTime = Environment.TickCount;
        Console.WriteLine("⏳Moogle is loading...");

        Preproccess();

        Console.WriteLine("⏳Moogle is loaded");
        int endTime = Environment.TickCount;
        float time = (float)(endTime - startTime) / 1000.0f;
        Console.WriteLine("⏰Loading time: {0}s", time);
    }

    //Preproccess all the data needed to run the search
    static void Preproccess()
    {
        AllDocs = DocumentCatcher.ReadDocumentsFromFolder(@"../Content");
        NumberOfDocuments = AllDocs.Length;
        MaxLogDoc = 0.0f;
        DocumentsWithTerm = new Dictionary<string, int>();
        IDFVector = new Dictionary<string, float>();
        queryDocument = new Document("");

        for (int i = 0; i < NumberOfDocuments; i++)
        {
            MaxLogDoc = Math.Max(MaxLogDoc, (float)Math.Log(AllDocs[i].Words.Length));
            AllDocs[i].TF = new Dictionary<string, float>();
            AllDocs[i].TFIDF = new Dictionary<string, float>();
        }

        MaxLogDoc += 1.0f;

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
        WordsToBeNear = new List<Tuple<string, string>>();
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
        //Fix the simbols of the operators to make sure they are removed just as in the documents
        string[] words = queryDocument.LowerizedText.Split(DocumentCatcher.Delims).Select(p => p.Trim()).ToArray();

        //Iterate over each document
        for (int i = 0; i < NumberOfDocuments; i++)
        {
            #region  LOOK_FOR_VALID_AND_INVALID_WORDS

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

            #endregion

            //Compute basic relevance of the document using TFIDF Vectors
            float score = TFIDFAnalyzer.ComputeRelevance(queryDocument, AllDocs[i]);

            //If relevance is 0 then we dont care about that document right?
            if (score == 0.0f)
            {
                continue;
            }

            //If document satisfies ! and ^ operators then we will try to add it to search result
            if (containsValidWords && !containsInvalidWords)
            {
                string snippet = BuildSnippet(words, AllDocs[i].Text, AllDocs[i].LowerizedText);
                if (snippet != "no snippet")    //If no snippet then the document was wrongly suggested
                {
                    //Now calculate extra relevance given by ~ operator
                    #region  CALCULATE_EXTRA_RELEVANCE

                    //Relevance given by nearness of words in the ~ operator
                    float extraRelevance = 0.0f;

                    //Iterate over each pair of words in WordsToBeNear
                    foreach (Tuple<string, string> pair in WordsToBeNear)
                    {
                        string string1 = pair.Item1;
                        string string2 = pair.Item2;

                        //If they both are present then we find their distance
                        if (AllDocs[i].WordPos.ContainsKey(string1) && AllDocs[i].WordPos.ContainsKey(string2))
                        {
                            //Best distance score so far for the current pair of words
                            float bestDistance = 0.0f;

                            //Try all possible combination of positions for the given words
                            foreach (int pos1 in AllDocs[i].WordPos[string1])
                            {
                                foreach (int pos2 in AllDocs[i].WordPos[string2])
                                {
                                    //Try to update bestDistance with current distanceMeasure
                                    float distance = (float)Math.Abs(pos2 - pos1);
                                    float distanceMeasure = MaxLogDoc - (float)Math.Log(distance);
                                    bestDistance = Math.Max(bestDistance, distanceMeasure);
                                }
                            }

                            //Give extraRelevance based on nearness of the given pair
                            extraRelevance += bestDistance;
                        }
                    }

                    #endregion

                    //Add extraRelevance to the current document
                    score += extraRelevance;

                    //Update result of the query with given document
                    results.Add(new SearchItem(AllDocs[i].Title, snippet, score));
                }
                else
                {
                    Console.WriteLine("📑Document found but snippet can't be calculated:");
                    Console.WriteLine(AllDocs[i].Title + "\n");
                }
            }
        }
    }

    static string BuildSuggestion()
    {
        string suggestion = "";

        //Find valid words with the minimum edit distance from query words
        //to replace them in the suggestion
        foreach (string word in queryDocument.Words)
        {
            //Ignore empty words
            if (word.Length == 0) continue;

            //Keep stop words just as they appear
            if (IDFVector.ContainsKey(word) && IDFVector[word] == 0.0f)
            {
                suggestion = suggestion + " " + word;
            }
            else
            {
                int bestDistance = word.Length;
                int charsInCommon = 0;
                string bestSuggestion = word;

                foreach (var vocabWord in IDFVector)
                {
                    if (vocabWord.Key.Length == 0)
                    {
                        continue;
                    }

                    //If the word is closer to be the word in input we update bestSuggestion
                    int curDistance = StringMethods.EditDistance(word, vocabWord.Key);
                    if (curDistance < bestDistance)
                    {
                        bestDistance = curDistance;
                        bestSuggestion = vocabWord.Key;
                    }
                    else if (curDistance == bestDistance)
                    {
                        int curCommonChars = StringMethods.LongestCommonSubsequence(word, vocabWord.Key);

                        //In case of tie usually is better if the word has more letters in common
                        if (curCommonChars > charsInCommon)
                        {
                            bestSuggestion = vocabWord.Key;
                            charsInCommon = curCommonChars;
                        }
                        //If they tie again is better if it starts with the same letter
                        else if (curCommonChars == charsInCommon && vocabWord.Key[0] == word[0])
                        {
                            bestSuggestion = vocabWord.Key;
                        }
                    }
                }

                suggestion = suggestion + " " + bestSuggestion;
            }
        }

        suggestion.Trim();

        return suggestion;
    }

    static string BuildSnippet(string[] words, string originalText, string lowerizedText)
    {
        string snippet = "";

        //Iterate each valid word present on query
        foreach (string word in words)
        {
            if (DocumentCatcher.InvalidWord(word))
            {
                continue;
            }

            //Skip stopWords
            bool isStopWord = false;
            if (IDFVector.ContainsKey(word) && IDFVector[word] == 0f)
            {
                isStopWord = true;
            }
            if (isStopWord) continue;

            //Find if the word exists in the text using Regular Expressions
            string pattern = String.Format(@"\b{0}\b", word);
            var match = System.Text.RegularExpressions.Regex.Match(lowerizedText, pattern);

            //If the word exists then we will build the snippet with the fragment of the text
            //near to the index of that word in the original text
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

        //If we cannot build the snippet
        return "no snippet";
    }

    static List<Tuple<string, string>> FindWordsToBeNear()
    {
        List<Tuple<string, string>> PairsOfWords = new List<Tuple<string, string>>();

        //Absorbs words from query as a list (just to allow removal of blank string(spaces))
        List<string> wordList = queryDocument.Words.ToList();

        bool emptyStringsRemaining = false;

        //Remove all blank strings(spaces) from words
        do
        {
            emptyStringsRemaining = wordList.Remove("");
        }
        while (emptyStringsRemaining);

        //Convert resulting wordList to array (just to allow fast indexation)
        string[] words = wordList.ToArray();

        //Calculate number of pairs of the form: word ~ word (on the array of words)
        for (int i = 0; i < words.Length; i++)
        {
            if (DocumentCatcher.IsAlphanumeric(words[i][0])
            && i + 2 < words.Length
            && words[i + 1] == "~"
            && DocumentCatcher.IsAlphanumeric(words[i + 2][0])
            )
            {
                PairsOfWords.Add(new Tuple<string, string>(words[i], words[i + 2]));
            }
        }

        return PairsOfWords;
    }

    public static SearchResult Query(string query)
    {
        //Checks if the given query is valid
        if (!Document.ValidQuery(query))
        {
            return new SearchResult(new SearchItem[] { }, INVALID_QUERY);
        }

        //Reset queryDocument
        ResetGlobalVariables();
        queryDocument = new Document(query);

        //Pairs of words that should be near for ~ operator
        WordsToBeNear = FindWordsToBeNear();

        //Calculate new TFIDF based on number of * on input
        CalculateNewRelevance();
        //Update previous values
        UpdateNewRelevance();

        List<SearchItem> results = new List<SearchItem>();

        //Calculates results of search based on relevance
        FindSearchResults(results);

        //Sort Results from greater to lesser score
        SearchItem[] items = results.ToArray();
        Array.Sort(items);
        Array.Reverse(items);

        //Build the suggestion based on number of results
        string suggestion = query;
        if (items.Length == 0)
        {
            suggestion = BuildSuggestion();
        }

        return new SearchResult(items, suggestion);
    }
}
