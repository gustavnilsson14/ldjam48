using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectoryKey : PublicKey
{
    public Directory targetDirectory;
    public override string GetName()
    {
        return targetDirectory.GetFullPath();
    }
    public override string GetUsageDescription()
    {
        return $"You are now completely safe in {targetDirectory.GetFullPath()}";
    }
}
