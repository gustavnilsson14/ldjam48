using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactersMultiplier : DirectoryModifier
{
    public float multiplier;
    public override string GetDescription()
    {
        string direction = (multiplier > 0) ? "increased" : "decreased";
        return $"{GetSource()}\nThe multiplier for command length cost is {direction} by {multiplier}";
    }
}
