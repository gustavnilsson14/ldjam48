using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotModifier : DirectoryModifier, IDamageSource
{
    public int damageBase = 1;
    public float speed = 1;
    public float momentum;
    protected override void OnRealTime()
    {
        base.OnRealTime();
        if (IsDisabled())
            return;
        momentum += Time.deltaTime;
        if (momentum < speed)
            return;
        momentum = 0;
        foreach (Entity entity in GetAffectedEntities())
        {
            entity.TakeHit(this, out int armorDamageTaken, out int bodyDamageTaken);
        }
    }
    public override string GetDescription()
    {
        return $"{GetSource()}\nEach {speed} seconds deals {damageBase} to all entities within. pid: {GetPid()}";
    }

    public int GetDamageBase()
    {
        return damageBase;
    }

    public int GetTotalDamage()
    {
        return damageBase;
    }

    public string GetDamageSourceName()
    {
        return $"{GetType().ToString()} at {GetSource()}";
    }
}
