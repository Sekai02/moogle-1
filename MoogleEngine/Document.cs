namespace MoogleEngine;

public class Document
{
    public Document(string title = "", string text = "")
    {
        this.Title = title;
        this.Text = text;
        this.LowerizedText = text.ToLower();
        this.Words = LowerizedText.Split(DocumentCatcher.Delims).Select(p => p.Trim()).ToArray();
        this.WordFrequency = new Dictionary<string, int>();
        this.TF = new Dictionary<string, float>();
        this.TFIDF = new Dictionary<string, float>();

        foreach (string word in this.Words)
        {
            if (DocumentCatcher.InvalidWord(word)) continue;

            if (WordFrequency.ContainsKey(word))
            {
                WordFrequency[word]++;
            }
            else
            {
                WordFrequency.Add(word, 1);
            }
        }

        //Safety check
        if (WordFrequency.ContainsKey(""))
        {
            WordFrequency.Remove("");
        }
    }

    //Document constructor when the document is a query (i.e has no title)
    public Document(string text = "")
    {
        this.Title = "";

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
        this.LowerizedText = text.ToLower();

        this.Words = LowerizedText.Split(DocumentCatcher.DelimsQuery).Select(p => p.Trim()).ToArray();
        this.WordFrequency = new Dictionary<string, int>();
        this.TF = new Dictionary<string, float>();
        this.TFIDF = new Dictionary<string, float>();

        foreach (string word in this.Words)
        {
            if (word.Length == 0) continue;

            string newWord = "";
            bool wordNeedsToBeRemoved = (word[0] == '!');
            bool wordNeedsToBeKeeped = (word[0] == '^');
            int numberOfAsters = 0;

            for (int i = 0; i < word.Length; i++)
            {
                char d = word[i];
                bool curIsOperator = DocumentCatcher.IsOperator(d);
                if (d == '*') numberOfAsters++;
                if (curIsOperator) continue;
                newWord += d;
            }

            if (DocumentCatcher.InvalidWord(newWord)) continue;

            //Conditions for keeping information about operators in document(query)
            if (wordNeedsToBeRemoved)
            {
                Update(newWord, Moogle.ExcludedWords);
            }
            if (wordNeedsToBeKeeped)
            {
                Update(newWord, Moogle.MandatoryWords);
            }
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

        //Safety check
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

    public Dictionary<string, float> TF { get; set; }

    public Dictionary<string, float> TFIDF { get; set; }
}
