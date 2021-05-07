using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOnCommand : DirectoryModifier, IDamageSource
{
    public int damageBase;
    protected override void OnCommand(Command command, ParsedCommand parsedCommand)
    {
        base.OnCommand(command, parsedCommand);
        if (IsDisabled())
            return;
        foreach (Entity entity in GetAffectedEntities())
        {
            if (entity.TakeHit(this, out int armorDamageTaken, out int bodyDamageTaken))
                continue;
        }

    }
    public override string GetDescription()
    {
        return $"{GetSource()}\nEach command you type damages all entities within by {damageBase}. pid: {GetPid()}";
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
