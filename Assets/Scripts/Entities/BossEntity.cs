using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEntity : Entity
{
    public DeathEvent onBossDeath = new DeathEvent();
    public override void OnDiscover(IDiscoverable arg0, bool arg1)
    {
        base.OnDiscover(arg0, arg1);
        IOTerminal.I.AppendTextLine($"---- {StringUtil.ColorWrap("ATTENTION, DANGER", Palette.RED)} ----");
        IOTerminal.I.AppendTextLine($"{name} encountered!\n{"description"}");
        IOTerminal.I.AppendTextLine($"---------------------------");
    }
}
