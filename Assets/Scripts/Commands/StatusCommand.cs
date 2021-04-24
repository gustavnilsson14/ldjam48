using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusCommand : Command
{
    public override bool Run(out string result, ParsedCommand parsedCommand)
    {
        if (!base.Run(out result, parsedCommand))
            return false;
        List<string> directories = new List<string>();
        foreach (Directory directory in Player.I.currentDirectory.GetAdjacentDirectories())
        {
            directories.Add(directory.name);
        }
        result = string.Join(", ", directories);
        return true;
    }
}
