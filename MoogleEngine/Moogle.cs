namespace MoogleEngine;

public static class Moogle
{
    static Document[] AllDocs = DocumentCatcher.ReadDocumentsFromFolder("../moogle-1/Content");
    static Dictionary<string, int> DocumentsWithTerm = new Dictionary<string, int>();
    static int NumberOfDocuments = AllDocs.Length;

    public static SearchResult Query(string query)
    {
        SearchItem[] items = new SearchItem[3] {
            new SearchItem("Hello World", "Lorem ipsum dolor sit amet", 0.9f),
            new SearchItem("Hello World", "Lorem ipsum dolor sit amet", 0.5f),
            new SearchItem("Hello World", "Lorem ipsum dolor sit amet", 0.1f),
        };

        return new SearchResult(items, query);
    }
}
