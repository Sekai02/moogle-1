namespace MoogleEngine;

//Class to represent Documents
public class Document
{
    //Document constructor when the given document is a real document (i.e has a title)
    public Document(string title = "", string text = "")
    {
        //Sets all properties to the default(conveniant) values
        this.Title = title;
        this.Text = text;

        //Lowerize original text
        this.LowerizedText = text.ToLower();

        //Removes all non-alphanumeric characters and builds the array of words
        this.Words = LowerizedText.Split(DocumentCatcher.Delims).Select(p => p.Trim()).ToArray();

        this.WordFrequency = new Dictionary<string, int>();
        this.TF = new Dictionary<string, float>();
        this.TFIDF = new Dictionary<string, float>();
        this.WordPos = new Dictionary<string, List<int>>();

        //Current index of the given document
        int idx = 0;

        //Update position of each word on document
        foreach (string word in this.Words)
        {
            if (DocumentCatcher.InvalidWord(word)) continue;

            if (WordFrequency.ContainsKey(word))
            {
                WordFrequency[word]++;
                WordPos[word].Add(idx);
            }
            else
            {
                WordFrequency.Add(word, 1);
                WordPos.Add(word, new List<int>(new int[] { idx }));
            }

            idx++;
        }

        //Safety checks
        if (WordFrequency.ContainsKey(""))
        {
            WordFrequency.Remove("");
        }
    }

    //Document constructor when the document is a query (i.e has no title)
    public Document(string text = "")
    {
        //Sets all properties to the default(conveniant) values
        this.Title = "";

        //Adjust text for proccessing operators (It inserts blank spaces if
        //* and ~ operators has no spaces between words) [reaccomodates the
        //text to proccess the query with the expected format]
        int pos = 0;
        while (pos < text.Length)
        {
            if (pos > 0)
            {
                char c = text[pos - 1];
                char d = text[pos];

                bool lastIsAlphanumeric = DocumentCatcher.IsAlphanumeric(c);
                bool curIsOperator = DocumentCatcher.IsOperator(d);

                bool curIsAlphanumeric = DocumentCatcher.IsAlphanumeric(d);
                bool lastIsTilde = (c == '~');

                if (lastIsAlphanumeric && curIsOperator)
                {
                    text = text.Insert(pos, " ");
                }
                else if (curIsAlphanumeric && lastIsTilde)
                {
                    text = text.Insert(pos, " ");
                }
            }
            pos++;
        }

        this.Text = text;

        //Lowerize original text
        this.LowerizedText = text.ToLower();

        //Removes all non-query characters and builds the array of words
        this.Words = LowerizedText.Split(DocumentCatcher.DelimsQuery).Select(p => p.Trim()).ToArray();

        this.WordFrequency = new Dictionary<string, int>();
        this.TF = new Dictionary<string, float>();
        this.TFIDF = new Dictionary<string, float>();
        this.WordPos = new Dictionary<string, List<int>>();

        //Proccess ! ^ * operators and transforms the words to only alphanumeric characters
        //[UPDATES IMPORTANT INFORMATION ON Moogle.cs !]
        foreach (string word in this.Words)
        {
            if (word.Length == 0) continue;

            //See if the word has to be removed or keeped
            string newWord = "";
            bool wordNeedsToBeRemoved = (word[0] == '!');
            bool wordNeedsToBeKeeped = (word[0] == '^');
            int numberOfAsters = 0;

            //1-Count the number of Asters in front of words
            //2-And Normalize words to only alphanumeric characters
            for (int i = 0; i < word.Length; i++)
            {
                char d = word[i];
                bool curIsOperator = DocumentCatcher.IsOperator(d);
                if (d == '*') numberOfAsters++;
                if (curIsOperator) continue;
                newWord += d;
            }

            //Ignore invalid words
            if (DocumentCatcher.InvalidWord(newWord)) continue;

            //Conditions for keeping information about operators in document(query)

            //Updates(in Moogle.ExcludedWords) wich words need to be removed
            if (wordNeedsToBeRemoved)
            {
                Update(newWord, Moogle.ExcludedWords);
            }

            //Updates(in Moogle.MandatoryWords) wich words need to be keeped
            if (wordNeedsToBeKeeped)
            {
                Update(newWord, Moogle.MandatoryWords);
            }

            //Updates(in Moogle.NumberOfAsters) the number of Asters for each word
            if (numberOfAsters > 0 && !Moogle.NumberOfAsters.ContainsKey(newWord))
            {
                Moogle.NumberOfAsters.Add(newWord, numberOfAsters);
            }

            //Updating WordFrequency of the document(query)
            if (WordFrequency.ContainsKey(newWord))
            {
                WordFrequency[newWord]++;
            }
            else
            {
                WordFrequency.Add(newWord, 1);
                TF.Add(newWord, 1.0f);
                TFIDF.Add(newWord, 1.0f);
            }
        }

        //Safety checks
        if (WordFrequency.ContainsKey(""))
        {
            WordFrequency.Remove("");
        }
        if (TF.ContainsKey(""))
        {
            TF.Remove("");
        }
        if (TFIDF.ContainsKey(""))
        {
            TFIDF.Remove("");
        }
    }

    //Check if the given query is valid (is not empty or it only contains non-word characters)
    public static bool ValidQuery(string query)
    {
        if (query == null || query.Length == 0) return false;

        string[] words = query.ToLower().Split(DocumentCatcher.Delims).Select(p => p.Trim()).ToArray();

        if (words.Length == 0) return false;

        int validWords = 0;

        foreach (string word in words)
        {
            bool isValid = false;
            foreach (char c in word)
            {
                if (DocumentCatcher.IsAlphanumeric(c))
                {
                    isValid = true;
                    break;
                }
            }
            if (isValid) validWords++;
        }

        if (validWords == 0) return false;

        return true;
    }

    //Utility update function for mandatory and forbidden words
    void Update(string val, List<string> list)
    {
        if (list != null && !DocumentCatcher.InvalidWord(val))
        {
            list.Add(val);
        }
    }

    public string Title { get; private set; }

    public string Text { get; private set; }

    public string LowerizedText { get; private set; }

    public string[] Words { get; private set; }

    public Dictionary<string, int> WordFrequency { get; private set; }

    public Dictionary<string, List<int>> WordPos { get; private set; }

    public Dictionary<string, float> TF { get; set; }

    public Dictionary<string, float> TFIDF { get; set; }
}
