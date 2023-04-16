namespace MoogleEngine;

public class Document
{
    public Document(string title = "", string text = "")
    {
        this.Title = title;
        this.Words = text.ToLower().Split(DocumentCatcher.Delims).Select(p => p.Trim()).ToArray();
        this.WordFrequency = new Dictionary<string, int>();
        this.TF = new Dictionary<string, float>();
        this.TFIDF = new Dictionary<string, float>();

        foreach (string word in this.Words)
        {
            if (WordFrequency.ContainsKey(word))
            {
                WordFrequency[word]++;
            }
            else
            {
                WordFrequency.Add(word, 1);
            }
        }
    }

    public Document(string text = "")
    {
        this.Title = "";

        int pos = 0;
        while (pos < text.Length)
        {
            if (pos > 0)
            {
                char c = text[pos - 1];

                bool lastIsAlphanumeric = ((c >= 'a' && c <= 'z')
                || (c >= '0' && c <= '9')
                || (c >= 'A' && c <= 'Z'));

                char d = text[pos];

                bool curIsOperator = DocumentCatcher.IsOperator(d);

                if (lastIsAlphanumeric && curIsOperator)
                {
                    text = text.Insert(pos, " ");
                }
            }
            pos++;
        }

        this.Words = text.ToLower().Split(DocumentCatcher.DelimsQuery).Select(p => p.Trim()).ToArray();
        this.WordFrequency = new Dictionary<string, int>();
        this.TF = new Dictionary<string, float>();
        this.TFIDF = new Dictionary<string, float>();

        foreach (string word in this.Words)
        {
            if (word.Length == 0) continue;
            string newWord = "";
            bool wordNeedsToBeRemoved = (word[0] == '!');
            bool wordNeedsToBeKeeped = (word[0] == '^');

            for (int i = 0; i < word.Length; i++)
            {
                char d = word[i];
                bool curIsOperator = DocumentCatcher.IsOperator(d);
                if (curIsOperator) continue;
                newWord += d;
            }

            if (wordNeedsToBeRemoved)
            {
                Update(newWord, Moogle.ExcludedWords);
            }
            if (wordNeedsToBeKeeped)
            {
                Update(newWord, Moogle.MandatoryWords);
            }

            if (WordFrequency.ContainsKey(newWord))
            {
                WordFrequency[newWord]++;
            }
            else
            {
                WordFrequency.Add(newWord, 1);
            }
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
                if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9'))
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
        if (list != null && val != null)
        {
            list.Add(val);
        }
    }

    public string Title { get; private set; }

    public string[] Words { get; private set; }

    public Dictionary<string, int> WordFrequency { get; private set; }

    public Dictionary<string, float> TF { get; set; }

    public Dictionary<string, float> TFIDF { get; set; }
}
