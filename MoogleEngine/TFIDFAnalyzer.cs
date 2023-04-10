namespace MoogleEngine;

public static class TFIDFAnalyzer
{
    public static void CalculateDocumentsWithTerm()
    {
        for (int i = 0; i < Moogle.NumberOfDocuments; i++)
        {
            foreach (var wordFrequency in Moogle.AllDocs[i].WordFrequency)
            {
                if (!Moogle.DocumentsWithTerm.ContainsKey(wordFrequency.Key))
                {
                    Moogle.DocumentsWithTerm.Add(wordFrequency.Key, 1);
                }
                else
                {
                    Moogle.DocumentsWithTerm[wordFrequency.Key]++;
                }
            }
        }
    }

    public static void CalculateIDFVector()
    {
        for (int i = 0; i < Moogle.NumberOfDocuments; i++)
        {
            foreach (var wordFrequency in Moogle.AllDocs[i].WordFrequency)
            {
                if (!Moogle.IDFVector.ContainsKey(wordFrequency.Key))
                {
                    float IDFScore = 0.0f;

                    if (Moogle.DocumentsWithTerm.ContainsKey(wordFrequency.Key)
                        && Moogle.DocumentsWithTerm[wordFrequency.Key] != 0)
                    {
                        IDFScore = (float)(Math.Log((double)Moogle.NumberOfDocuments /
                        Moogle.DocumentsWithTerm[wordFrequency.Key]));
                    }

                    Moogle.IDFVector.Add(wordFrequency.Key, IDFScore);
                }
            }
        }
    }

    public static void CalculateTFVectors()
    {
        for (int i = 0; i < Moogle.NumberOfDocuments; i++)
        {
            foreach (var wordFrequency in Moogle.AllDocs[i].WordFrequency)
            {
                if (!Moogle.TFVectors[i].ContainsKey((wordFrequency.Key)))
                {
                    float TFScore = 0.0f;

                    if (Moogle.AllDocs[i].Words.Length > 0)
                    {
                        TFScore = (float)wordFrequency.Value / Moogle.AllDocs[i].Words.Length;
                    }

                    Moogle.TFVectors[i].Add(wordFrequency.Key, TFScore);
                }
            }
        }
    }

    public static void CalculateTFIDFVectors()
    {
        for (int i = 0; i < Moogle.NumberOfDocuments; i++)
        {
            foreach (var wordFrequency in Moogle.TFVectors[i])
            {
                if (Moogle.TFVectors[i].ContainsKey(wordFrequency.Key)
                && Moogle.IDFVector.ContainsKey(wordFrequency.Key))
                {
                    float TFScore = Moogle.TFVectors[i][wordFrequency.Key];
                    float IDFScore = Moogle.IDFVector[wordFrequency.Key];

                    Moogle.TFIDFVectors[i].Add(wordFrequency.Key, TFScore * IDFScore);
                }
                else
                {
                    Moogle.TFIDFVectors[i].Add(wordFrequency.Key, 0);
                }
            }
        }
    }
}