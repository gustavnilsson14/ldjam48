using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEntity : Entity
{
    public DeathEvent onBossDeath = new DeathEvent();
    
    public override void Die()
    {
        onBossDeath.Invoke();
        base.Die();
    }
    public override void Discover()
    {
        base.Discover();
        IOTerminal.I.AppendTextLine($"---- {StringUtil.ColorWrap("ATTENTION, DANGER", Color.red)} ----");
        IOTerminal.I.AppendTextLine($"{name} encountered!\n{description}");
        IOTerminal.I.AppendTextLine($"---------------------------");

    }
}
