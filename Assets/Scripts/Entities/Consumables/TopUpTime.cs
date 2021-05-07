using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopUpTime : TopUp
{
    public int extraSeconds = 300;
    public override void Consume(out string result)
    {
        base.Consume(out result);
        Player.I.currentSeconds += extraSeconds;
        result = $"Topup verified, added {extraSeconds} seconds to your balance on this server, which is now {Player.I.GetTimeLeft()}";
    }
}
