namespace MoogleEngine;

public static class StringMethods
{
    public static int EditDistance(string s1, string s2)
    {
        int[,] editDistance = new int[s1.Length + 1, s2.Length + 1];

        for (int i = 0; i <= s1.Length; i++)
        {
            for (int j = 0; j <= s2.Length; j++)
            {
                if (i == 0) editDistance[i, j] = j;
                else if (j == 0) editDistance[i, j] = i;
                else if (s1[i - 1] == s2[j - 1])
                {
                    editDistance[i, j] = editDistance[i - 1, j - 1];
                }
                else
                {
                    editDistance[i, j] = 1 + Math.Min(editDistance[i - 1, j - 1],
                    Math.Min(editDistance[i - 1, j], editDistance[i, j - 1]));
                }
            }
        }

        return editDistance[s1.Length, s2.Length];
    }

    public static int LongestCommonSubsequence(string s1, string s2)
    {
        int[,] longestCommonSubsequence = new int[s1.Length + 1, s2.Length + 1];

        for (int i = 0; i <= s1.Length; i++)
        {
            for (int j = 0; j <= s2.Length; j++)
            {
                if (i == 0 || j == 0)
                {
                    longestCommonSubsequence[i, j] = 0;
                }
                else if (s1[i - 1] == s2[j - 1])
                {
                    longestCommonSubsequence[i, j] = 1 + longestCommonSubsequence[i - 1, j - 1];
                }
                else
                {
                    longestCommonSubsequence[i, j] = Math.Max(longestCommonSubsequence[i - 1, j], longestCommonSubsequence[i, j - 1]);
                }
            }
        }

        return longestCommonSubsequence[s1.Length, s2.Length];
    }
}