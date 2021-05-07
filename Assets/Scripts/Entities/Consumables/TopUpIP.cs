﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopUpIP : TopUp
{
    public int extraIP = 20;
    public override void Consume(out string result)
    {
        base.Consume(out result);
        int restoration = Mathf.Clamp(extraIP, 0, Player.I.maxIP - Player.I.GetCurrentIP());
        Player.I.Heal(restoration);
        result = $"Topup verified, added {restoration} IP to your balance, which is now {Player.I.GetCurrentIP()} out of {Player.I.maxIP}";
    }
}
