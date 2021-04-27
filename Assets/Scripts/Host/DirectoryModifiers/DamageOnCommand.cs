using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOnCommand : DirectoryModifier
{
    public int damage;
    protected override void OnCommand(Command command, ParsedCommand parsedCommand)
    {
        base.OnCommand(command, parsedCommand);
        foreach (Entity entity in GetAffectedEntities())
        {
            if (entity.TakeDamage(damage))
                continue;
        }

    }
    public override string GetDescription()
    {
        return $"{GetSource()}\nEach command you type damages all entities within by {damage}";
    }
}
