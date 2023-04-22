using MoogleEngine;

public class Node
{
    const int ALPHABET_SIZE = 256;

    public Node[] children = new Node[ALPHABET_SIZE];
    public bool IsLeaf;
    public int Count;

    public Node()
    {
        for (int i = 0; i < ALPHABET_SIZE; i++)
        {
            this.children[i] = null!;
        }
        this.IsLeaf = false;
        this.Count = 0;
    }
}

public class Trie
{
    public Node root;
    public int index;

    public Trie()
    {
        root = new Node();
    }

    public void InsertWord(string key)
    {
        int idx;

        Node cur = root;

        for (int level = 0; level < key.Length; level++)
        {
            idx = (int)key[level];

            if (cur.children[idx] == null)
            {
                cur.children[idx] = new Node();
            }

            cur = cur.children[idx];
            cur.Count++;
            Console.WriteLine((char)idx + " " + cur.Count);
        }

        cur.IsLeaf = true;
    }

    public string FindLCP(String key)
    {
        int idx;

        Node cur = root;

        string LCP = key, Prefix = "";
        int MaxCommonPrefix = 0;

        for (int level = 0; level < key.Length; level++)
        {
            idx = (int)key[level];

            if (cur.children[idx] == null)
                return LCP;

            cur = cur.children[idx];

            Prefix += key[level];

            if (cur.Count >= MaxCommonPrefix && level + 1 >= 3)
            {
                MaxCommonPrefix = cur.Count;
                LCP = Prefix;
            }
        }

        return LCP;
    }
}