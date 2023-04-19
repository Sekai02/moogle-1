namespace MoogleEngine;

//Class to represent items of SearchResult
public class SearchItem : IComparable
{
    public SearchItem(string title, string snippet, float score)
    {
        this.Title = title;
        this.Snippet = snippet;
        this.Score = score;
    }

    //Function to compare objects of this class
    public int CompareTo(object? obj)
    {
        if (obj is null) return 1;

        SearchItem? otherSearchItem = obj as SearchItem;
        if (otherSearchItem is not null)
        {
            return this.Score.CompareTo(otherSearchItem.Score);
        }
        else throw new ArgumentException("Object is not a SearchItem");
    }

    public string Title { get; private set; }

    public string Snippet { get; private set; }

    public float Score { get; private set; }
}
