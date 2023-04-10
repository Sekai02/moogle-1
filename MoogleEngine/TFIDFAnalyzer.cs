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

    public static void CalculateTFVector(ref Document doc)
    {
        for (int i = 0; i < Moogle.NumberOfDocuments; i++)
        {
            foreach (var wordFrequency in doc.WordFrequency)
            {
                if (!doc.TF.ContainsKey((wordFrequency.Key)))
                {
                    float TFScore = 0.0f;

                    if (doc.Words.Length > 0)
                    {
                        TFScore = (float)wordFrequency.Value / doc.Words.Length;
                    }

                    doc.TF.Add(wordFrequency.Key, TFScore);
                }
            }
        }
    }

    public static void CalculateTFIDFVector(ref Document doc)
    {
        foreach (var wordFrequency in doc.TF)
        {
            if (doc.TF.ContainsKey(wordFrequency.Key)
            && Moogle.IDFVector.ContainsKey(wordFrequency.Key))
            {
                float TFScore = doc.TF[wordFrequency.Key];
                float IDFScore = Moogle.IDFVector[wordFrequency.Key];

                doc.TFIDF.Add(wordFrequency.Key, TFScore * IDFScore);
            }
            else
            {
                doc.TFIDF.Add(wordFrequency.Key, 0);
            }
        }
    }

    public static float Norm(Dictionary<string, float> vec)
    {
        float ret = 0.0f;

        foreach (var tfidf in vec)
        {
            float score = tfidf.Value * tfidf.Value;
            ret += score;
        }

        return (float)Math.Sqrt(ret);
    }

    public static float ComputeRelevance(Document query, Document doc)
    {
        float numerator = 0.0f;

        foreach (var word in query.TFIDF)
        {
            float queryWordScore = query.TFIDF[word.Key];
            float docWordScore = 0.0f;

            if (doc.TFIDF.ContainsKey(word.Key))
            {
                docWordScore = doc.TFIDF[word.Key];
            }

            numerator += queryWordScore * docWordScore;
        }

        float denominator = Norm(query.TFIDF) * Norm(doc.TFIDF);

        if (denominator != 0)
        {
            return numerator / denominator;
        }
        else
        {
            return 0.0f;
        }
    }
}