using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class MvCommand : Command
{
    public override bool Run(out string result, ParsedCommand parsedCommand)
    {
        if (!base.Run(out result, parsedCommand))
            return false;
        List<Entity> entities = Player.I.currentDirectory.GetEntities();
        Entity target = entities.Find(entity => entity.name == parsedCommand.arguments[0]);
        HostHandler.I.currentHost.GetDirectoryByPath(parsedCommand.arguments[1], out Directory directory);
        target.MoveTo(directory);
        result = $"Moved {parsedCommand.arguments[0]} to path {parsedCommand.arguments[1]}";
        return true;
    }
    protected override bool ValidateParsedCommand(out string result, ParsedCommand parsedCommand)
    {
        result = name +" requires the first argument to be a target file or process id (pid)";
        if (!parsedCommand.HasArguments())
            return false;
        result = parsedCommand.arguments[0] + " is not a file or process id (pid)";
        if (!ArgumentIsEntity(parsedCommand.arguments[0]))
            return false;
        result = name + " requires the second argument to be a target path from root";
        if (parsedCommand.arguments.Count < 2)
            return false;
        result = parsedCommand.arguments[1] + " is not a valid path from root";
        if (!ArgumentIsPathFromRoot(parsedCommand.arguments[1]))
            return false;

        return true;
    }
}
