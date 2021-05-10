using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectoryKey : PublicKey
{
    public string path;

    public override string GetName()
    {
        return path;
    }
    public override string GetUsageDescription()
    {
        return $"You are now completely safe in {path}";
    }
}
