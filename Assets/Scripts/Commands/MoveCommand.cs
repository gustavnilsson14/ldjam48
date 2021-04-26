using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCommand : Command
{
    public override bool Run(out string result, ParsedCommand parsedCommand)
    {
        if (!base.Run(out result, parsedCommand))
            return false;
        GetTargetDirectory(out Directory directory, out result, parsedCommand);
        Player.I.MoveTo(directory);
        result = "Now in " + Player.I.currentDirectory.name;
        return true;
    }
    protected override bool ValidateParsedCommand(out string result, ParsedCommand parsedCommand)
    {
        if (!base.ValidateParsedCommand(out result, parsedCommand))
            return false;
        result = name + " requires the first argument to be an adjacent directory";
        if (!parsedCommand.HasArguments())
            return false;
        if (!GetTargetDirectory(out Directory directory, out result, parsedCommand))
            return false;
        result = $"You have no access to {directory.GetFullPath()}";
        if (!Player.I.IsAllowedInDirectory(directory))
            return false;
        return true;
    }
    public bool GetTargetDirectory(out Directory directory, out string result, ParsedCommand parsedCommand)
    {
        directory = null;
        result = "";
        switch (parsedCommand.arguments[0])
        {
            case "-":
                result = "Cannot use move - with no movement history";
                if (Player.I.directoryHistory.Count == 0)
                    return false;
                directory = Player.I.directoryHistory[0];
                break;
            case "..":
                result = "Cannot use move .. from root directory";
                List<Directory> parents = Player.I.currentDirectory.GetAllParents();
                if (parents.Count < 2)
                    return false;
                directory = parents[parents.Count-2];
                break;
            default:
                result = parsedCommand.arguments[0] + " is not an adjacent directory";
                if (!GetAdjacentDirectory(parsedCommand.arguments[0], out directory))
                    return false;
                break;
        }
        result = "General error, no directory";
        if (directory == null)
            return false;
        return true;
    }
}
