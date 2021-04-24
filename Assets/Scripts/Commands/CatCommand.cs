using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatCommand : Command
{
    public override bool Run(out string result, ParsedCommand parsedCommand)
    {
        Debug.Log("CatCommand RUN");
        if (!base.Run(out result, parsedCommand))
            return false;
        List<Entity> entities = Player.I.currentDirectory.GetEntities();
        Entity target = entities.Find(entity => entity.name == parsedCommand.arguments[0]);
        result = target.GetCatDescription();
        return true;
    }
    protected override bool ValidateParsedCommand(out string result, ParsedCommand parsedCommand)
    {
        result = name + " requires the first argument to be an entity";
        if (!parsedCommand.HasArguments())
            return false;
        result = parsedCommand.arguments[0] + " is not an entity";
        if (!ArgumentIsEntity(parsedCommand.arguments[0]))
            return false;
        return true;
    }
}
