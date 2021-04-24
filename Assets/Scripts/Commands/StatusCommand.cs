using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusCommand : Command
{
    public override void Run(ParsedCommand parsedCommand)
    {
        base.Run(parsedCommand);
        List<string> directories = new List<string>();
        foreach (Directory directory in Player.I.currentDirectory.GetAdjacentDirectories())
        {
            directories.Add(directory.name);
        }
        IOTerminal.I.AppendTextLine(string.Join(", ", directories));
    }
}
