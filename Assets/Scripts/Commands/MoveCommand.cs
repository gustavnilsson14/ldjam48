using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCommand : Command
{
    public override bool Run(out string result, ParsedCommand parsedCommand)
    {
        if (!base.Run(out result, parsedCommand))
            return false;
        Player.I.MoveTo(Player.I.currentDirectory.GetAdjacentDirectories().Find(dir => dir.name == parsedCommand.arguments[0]));
        result = "Now in " + Player.I.currentDirectory.name;
        return true;
    }
    protected override bool ValidateParsedCommand(out string result, ParsedCommand parsedCommand)
    {
        result = name + " requires the first argument to be an adjacent directory";
        if (!parsedCommand.HasArguments())
            return false;
        result = parsedCommand.arguments[0] + " is not an adjacent directory";
        if (!GetAdjacentDirectory(parsedCommand.arguments[0], out Directory adjacentDirectory))
            return false;
        result = $"You have no access to {adjacentDirectory.GetFullPath()}";
        if (!Player.I.IsAllowedInDirectory(adjacentDirectory))
            return false;
        return true;
    }
}
