namespace MoogleEngine;

public class SearchItem : IComparable
{
    public SearchItem(string title, string snippet, float score)
    {
        this.Title = title;
        this.Snippet = snippet;
        this.Score = score;
    }

    public int CompareTo(object obj)
    {
        if (obj is SearchItem)
        {
            return this.Score.CompareTo((obj as SearchItem).Score);
        }
        throw new ArgumentException("Object is not a Search Item");
    }

    public string Title { get; private set; }

    public string Snippet { get; private set; }

    public float Score { get; private set; }
}
