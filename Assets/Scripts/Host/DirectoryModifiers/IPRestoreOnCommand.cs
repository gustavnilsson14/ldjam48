using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IPRestoreOnCommand : DirectoryModifier
{
    public int heal = 1;
    protected override void OnCommand(Command command, ParsedCommand parsedCommand)
    {
        base.OnCommand(command, parsedCommand);
        if (!CheckProcLimit())
            return;
        foreach (Entity entity in entitiesAffected)
        {
            entity.Heal(heal);
        }
    }
    public override string GetDescription()
    {
        return $"{GetSource()}\nEach command you type restores {heal} IP to all entities within. Can restore a total of {currentProcsOnEntities} more IP";
    }
}
