using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyEntity : Entity
{
    public PublicKey publicKey;
    public override bool TakeHit(IDamageSource source, out int armorDamageTaken, out int bodyDamageTaken)
    {
        armorDamageTaken = 0;
        bodyDamageTaken = 0;
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
