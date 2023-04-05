namespace MoogleEngine;

public class Document
{
    public Document(string title = "", string text = "")
    {
        this.Title = title;
        this.Words = text.ToLower().Split(DocumentCatcher.Delims).Select(p => p.Trim()).ToArray();
        this.WordFrequency = new Dictionary<string, int>();
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

    public string Title { get; private set; }

    public string[] Words { get; private set; }

    public Dictionary<string, int> WordFrequency { get; private set; }
}
