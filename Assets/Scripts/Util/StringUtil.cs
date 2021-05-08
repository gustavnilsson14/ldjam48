using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringUtil
{
    public static string ColorWrap(string s, Palette color)
    {
        string colorHex = PaletteUtil.Get(color);
        return $"<color={colorHex}>{s}</color>";
    }
}