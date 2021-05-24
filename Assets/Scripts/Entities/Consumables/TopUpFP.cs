using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopUpFP : TopUp
{
    public override void Consume(out string result)
    {
        base.Consume(out result);
        Player.I.currentFP = Player.I.maxFP;
        result = $"Topup verified, FP restored to the max, which is {Player.I.maxFP}";
    }
}
