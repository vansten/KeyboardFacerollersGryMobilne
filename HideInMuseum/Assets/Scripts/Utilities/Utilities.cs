using UnityEngine;
using System.Collections.Generic;

public static class Utilities
{
    private static Dictionary<char, int> _hexToDec = new Dictionary<char, int>()
    {
        {'0', 0 },
        {'1', 1 },
        {'2', 2 },
        {'3', 3 },
        {'4', 4 },
        {'5', 5 },
        {'6', 6 },
        {'7', 7 },
        {'8', 8 },
        {'9', 9 },
        {'A', 10 },
        {'a', 10 },
        {'B', 11 },
        {'b', 11 },
        {'C', 12 },
        {'c', 12 },
        {'D', 13 },
        {'d', 13 },
        {'E', 14 },
        {'e', 14 },
        {'F', 15 },
        {'f', 15 },

    };

    public static string ConvertSecondsToTimeText(float secondsLeft)
    {
        string s = "";
        int minutes = (int)(secondsLeft / 60.0f);
        int seconds = (int)(secondsLeft - (minutes * 60));
        s = minutes.ToString("00");
        s += ":";
        s += seconds.ToString("00");
        return s;
    }

    public static Color ColorFromHex(string hex)
    {
        bool alphaIncluded = hex.Length == 8;
        string r = hex.Substring(0, 2);
        string g = hex.Substring(2, 2);
        string b = hex.Substring(4, 2);

        float red = 0.0f;
        float green = 0.0f;
        float blue = 0.0f;

        red = (_hexToDec[r[0]] * 16.0f + _hexToDec[r[1]]) / 255.0f;
        green = (_hexToDec[g[0]] * 16.0f + _hexToDec[g[1]]) / 255.0f;
        blue = (_hexToDec[b[0]] * 16.0f + _hexToDec[b[1]]) / 255.0f;

        float alpha = 1.0f;
        if (alphaIncluded)
        {
            string a = hex.Substring(6, 2);
            alpha = (_hexToDec[a[0]] * 16.0f + _hexToDec[a[1]]) / 255.0f;
        }

        return new Color(red, green, blue, alpha);
    }

    public static float CalculateColorDistance(Color a, Color b)
    {
        float distance = 0.0f;

        distance += Mathf.Abs(a.r - b.r);
        distance += Mathf.Abs(a.g - b.g);
        distance += Mathf.Abs(a.b - b.b);

        return distance;
    }
}
