using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringUtil
{
    public static string ColorWrap(string s, Color color)
    {
        string colorHex = ColorUtility.ToHtmlStringRGB(color);
        return $"<color=#{colorHex}>{s}</color>";
    }
    public static string ColorWrap(string s, string color)
    {
        return $"<color=#{color}>{s}</color>";
    }
}
