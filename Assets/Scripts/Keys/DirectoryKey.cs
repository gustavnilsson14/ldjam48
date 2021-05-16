using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectoryKey : PublicKey
{
    public string path;

    public override string GetName()
    {
        return $"{path.Replace("/","_")}";
    }
    public override string GetShortDescription()
    {
        return $"You are now completely safe in {path}";
    }
}
