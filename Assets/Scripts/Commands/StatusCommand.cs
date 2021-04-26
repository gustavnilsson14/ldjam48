using System.Collections;
using System.Linq;
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
        result += "IP: " + Player.I.GetCurrentIP() + "/" + Player.I.maxIP;
        result += "\nCharacters: " + Player.I.currentCharacters;
        result += "\nTime: " + Player.I.GetTimeLeft();
        result += "\nAdjacencies: " + string.Join(", ", directories);
        result += "\nCurrent modifiers: \n" + string.Join("\n", Player.I.activeModifiers.Select(activeModifier => activeModifier.GetDescription()));
        return true;
    }
}
