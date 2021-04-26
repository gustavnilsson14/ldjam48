using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOnCommand : DirectoryModifier
{
    public int damage;
    protected override void OnCommand(Command command, ParsedCommand parsedCommand)
    {
        base.OnCommand(command, parsedCommand);
        foreach (Entity entity in entitiesAffected)
        {
            entity.TakeDamage(damage);
        }
    }
}
