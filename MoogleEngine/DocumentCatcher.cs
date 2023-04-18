namespace MoogleEngine;

public static class DocumentCatcher
{
    public static char[] Delims = IgnoreASCII().ToCharArray();
    public static char[] DelimsQuery = IgnoreASCIIQuery().ToCharArray();

    public static string IgnoreASCII()
    {
        string ignoreASCII = "";
        for (char c = (char)0; c < 255; c++)
        {
            if (c >= 'A' && c <= 'Z') continue;
            else if (c >= 'a' && c <= 'z') continue;
            else if (c >= '0' && c <= '9') continue;
            else ignoreASCII += c;
        }
        return ignoreASCII;
    }

    public static string IgnoreASCIIQuery()
    {
        string ignoreASCII = "";
        for (char c = (char)0; c < 255; c++)
        {
            if (c >= 'A' && c <= 'Z') continue;
            else if (c >= 'a' && c <= 'z') continue;
            else if (c >= '0' && c <= '9') continue;
            else if (c == '!' || c == '^' || c == '*' || c == '~') continue;
            else ignoreASCII += c;
        }
        return ignoreASCII;
    }

    public static bool IsAlphanumeric(char c)
    {
        if (c >= 'A' && c <= 'Z') return true;
        else if (c >= 'a' && c <= 'z') return true;
        else if (c >= '0' && c <= '9') return true;
        return false;
    }

    public static bool IsOperator(char c)
    {
        return (c == '!' || c == '^' || c == '*' || c == '~');
    }

    public static Document[] ReadDocumentsFromFolder(string folder)
    {
        var txtFiles = Directory.GetFiles(folder, "*.txt");

        Document[] allDocuments = new Document[txtFiles.Length];
        int currentIndex = 0;

        foreach (var file in txtFiles)
        {
            if (File.Exists(file))
            {
                string currentText = File.ReadAllText(file);
                string currentTitle = Path.GetFileName(file);
                allDocuments[currentIndex] = new Document(currentTitle, currentText);
                currentIndex++;
            }
        }

        return allDocuments;
    }
}