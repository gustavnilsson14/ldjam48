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
        result = "move requires the first argument to be an adjacent destination";
        if (!parsedCommand.HasArguments())
            return false;
        result = parsedCommand.arguments[0] + " is not an adjacent directory";
        List<Directory> adjacentDirectories = Player.I.currentDirectory.GetAdjacentDirectories();
        Directory adjacentDirectory = adjacentDirectories.Find(dir => dir.name == parsedCommand.arguments[0]);
        if (adjacentDirectory == null)
            return false;
        return true;
    }
}
