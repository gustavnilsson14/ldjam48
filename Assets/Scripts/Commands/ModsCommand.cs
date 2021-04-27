using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class ModsCommand : Command
{
    public override bool Run(out string result, ParsedCommand parsedCommand)
    {
        if (!base.Run(out result, parsedCommand))
            return false;

        GetTargetDirectory(out Directory directory, out result, parsedCommand);
        result = $"Modifiers in {directory.GetFullPath()};\n";
        foreach (DirectoryModifier directoryModifier in directory.GetModifiers())
        {
            result += directoryModifier.GetDescription() + "\n";
        }
        return true;
    }

    protected override bool ValidateParsedCommand(out string result, ParsedCommand parsedCommand)
    {
        if (!base.ValidateParsedCommand(out result, parsedCommand))
            return false;
        if (!GetTargetDirectory(out Directory directory, out result, parsedCommand))
            return false;
        return true;
    }
    public bool GetTargetDirectory(out Directory directory, out string result, ParsedCommand parsedCommand)
    {
        result = "";
        directory = Player.I.currentDirectory;
        if (!parsedCommand.HasArguments())
            return true;
        directory = null;
        result = parsedCommand.arguments[0] + " is not a directory";
        if (parsedCommand.arguments[0].Contains("/"))
        {
            if (!HostHandler.I.currentHost.GetDirectoryByPath(parsedCommand.arguments[0], out directory))
                return false;
            return true;
        }
        directory = Player.I.currentDirectory.GetAdjacentDirectories().Find(x => x.name == parsedCommand.arguments[0]);
        if (directory == null)
            return false;
        return true;
    }
}
