using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CdCommand : Command
{
    public override bool Run(out string result, ParsedCommand parsedCommand)
    {
        if (!base.Run(out result, parsedCommand))
            return false;

        GetTargetDirectory(out Directory directory, out result, parsedCommand);
        Player.I.MoveTo(directory);
        result = "";
        return true;
    }
    protected override bool ValidateParsedCommand(out string result, ParsedCommand parsedCommand)
    {
        if (!base.ValidateParsedCommand(out result, parsedCommand))
            return false;
        result = name + " requires the first argument to be a path";
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
                result = "Cannot use cd - with no movement history";
                if (Player.I.directoryHistory.Count == 0)
                    return false;
                directory = Player.I.directoryHistory[0];
                break;
            case "..":
                result = "Cannot use cd .. from root directory";
                List<Directory> parents = Player.I.currentDirectory.GetAllParents();
                if (parents.Count < 2)
                    return false;
                directory = parents[parents.Count - 2];
                break;
            default:
                result = parsedCommand.arguments[0] + " is not a directory";
                if (parsedCommand.arguments[0].Contains("/"))
                {
                    if (!HostHandler.I.currentHost.GetDirectoryByPath(parsedCommand.arguments[0], out directory))
                        return false;
                    break;
                }

                List<Directory> children = new List<Directory>();
                children.AddRange(HostHandler.I.currentHost.GetComponentsInChildren<Directory>());
                directory = children.Find(x => x.name == parsedCommand.arguments[0]);
                break;
        }
        result = "General error, no directory";
        if (directory == null)
            return false;
        return true;
    }
}
