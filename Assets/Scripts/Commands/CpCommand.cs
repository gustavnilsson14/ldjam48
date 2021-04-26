using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class CpCommand : Command
{
    public override bool Run(out string result, ParsedCommand parsedCommand)
    {
        if (!base.Run(out result, parsedCommand))
            return false;
        List<Entity> entities = Player.I.currentDirectory.GetEntities();

        Entity target = entities.Find(entity => entity.name == parsedCommand.arguments[0]);
        if (target == Player.I)
        {
            result = $"You do not have permission to copy this file";
            return true;
        }

        HostHandler.I.currentHost.GetDirectoryByPath(parsedCommand.arguments[1], out Directory directory);
        Entity copy = Instantiate(target, directory.transform).GetComponent<Entity>();
        copy.currentDirectory = directory;
        result = $"Made a copy of {parsedCommand.arguments[0]} in path {parsedCommand.arguments[1]}";
        return true;
    }
    protected override bool ValidateParsedCommand(out string result, ParsedCommand parsedCommand)
    {
        if (!base.ValidateParsedCommand(out result, parsedCommand))
            return false;
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
