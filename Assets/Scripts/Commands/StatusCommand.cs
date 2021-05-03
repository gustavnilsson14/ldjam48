using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;

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
        IEnumerable<string> modifierDescriptions = Player.I.currentDirectory.GetModifiers().Select(activeModifier => activeModifier.GetDescription());
        if (GetStatusList(out string modifiersDescription, modifierDescriptions))
        {
            result += "\nCurrent modifiers: ";
            result += modifiersDescription;
        }
        IEnumerable<string> conditionDescriptions = Player.I.GetAllConditions().Select(condition => condition.GetDescription());
        if (GetStatusList(out string conditionsDescription, conditionDescriptions))
        {
            result += "\nCurrent conditions: ";
            result += conditionsDescription;
        }
        return true;
    }

    private bool GetStatusList(out string result, IEnumerable<string> strings)
    {
        result = "";
        if (strings.Count() == 0)
            return false;
        result = "\n    ";
        result += string.Join("\n    ", strings);
        return true;
    }
}
