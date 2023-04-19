namespace MoogleEngine;

//Class to proccess documents and some utility functions to deal with texts and files
public static class DocumentCatcher
{
    //List of forbidden characters for documents
    public static char[] Delims = IgnoreASCII().ToCharArray();
    //List of forbidden characters for query
    public static char[] DelimsQuery = IgnoreASCIIQuery().ToCharArray();

    //Generate forbidden characters for documents
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

    //Generate forbidden characters for query
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

    //Check if a char is a number of a letter of the english alphabet
    public static bool IsAlphanumeric(char c)
    {
        if (c >= 'A' && c <= 'Z') return true;
        else if (c >= 'a' && c <= 'z') return true;
        else if (c >= '0' && c <= '9') return true;
        return false;
    }

    //Check if a char is a query operator
    public static bool IsOperator(char c)
    {
        return (c == '!' || c == '^' || c == '*' || c == '~');
    }

    //Check if a word is invalid (empty or non alphanumeric)
    public static bool InvalidWord(string word)
    {
        return (word == null || word.Length == 0 || word == ""
        || word == " " || !IsAlphanumeric(word[0]));
    }

    //Reads all documents(.txt) from given folder and converts them into 
    //Document[] (an array of documents)
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