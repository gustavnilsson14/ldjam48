using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageMultiplier : DirectoryModifier
{
    public float multiplier;
    public override string GetDescription()
    {
        string direction = (multiplier > 0) ? "increased" : "decreased";
        return $"{GetSource()}\nThe multiplier for any damage is {direction} by {multiplier}";
    }
}
