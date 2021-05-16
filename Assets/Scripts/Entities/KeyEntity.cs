using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyEntity : PickupEntity
{
    public PublicKey publicKey;
    public override List<string> FormatCatDescription(List<string> catDescription)
    {
        return catDescription;
        /*
        catDescription.AddRange(
            new List<string> {
                $"The key to {publicKey.GetName()} was added to lib!",
                publicKey.GetUsageDescription()
            }
        );
        publicKey.isAvailable = true;
        return catDescription;
        */
    }
}
