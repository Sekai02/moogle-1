namespace MoogleEngine;

public class Document
{
    public Document(string title = "", string text = "")
    {
        this.Title = title;
        this.Words = text.ToLower().Split(DocumentCatcher.Delims).Select(p => p.Trim()).ToArray();
    }

    public string Title { get; private set; }

    public string[] Words { get; private set; }
}
