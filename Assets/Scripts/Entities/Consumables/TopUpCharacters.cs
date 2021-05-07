using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopUpCharacters : TopUp
{
    public int extraCharacters = 500;
    public override void Consume(out string result)
    {
        base.Consume(out result);
        Player.I.currentCharacters += extraCharacters;
        result = $"Topup verified, added {extraCharacters} characters to your balance on this server, which is now {Player.I.currentCharacters}";
    }
}
