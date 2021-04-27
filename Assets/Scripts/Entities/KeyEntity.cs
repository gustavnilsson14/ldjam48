using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyEntity : Entity
{
    public PublicKey publicKey;
    public override bool TakeDamage(int amount, string source = "", string overrideTextLine = "")
    {
        return false;
    }
    public override string GetCatDescription()
    {
        List<string> result = new List<string> {
            GetBinaryStatic(),
            $"The key to {publicKey.GetName()} was added to lib!",
            publicKey.GetUsageDescription()
        };
        onCat.Invoke();
        publicKey.isAvailable = true;
        Die();
        return string.Join("\n", result);
    }
}
