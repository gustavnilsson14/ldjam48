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
        result += $"IP: {Player.I.currentIP}/{Player.I.maxIP}\n";
        result += $"Characters: {Player.I.currentCharacters}\n";
        result += $"Time: {Player.I.GetTimeLeft()}\n";
        result += $"Adjacencies: {string.Join(", ", directories)}\n";

        IEnumerable<string> modifierDescriptions = Player.I.currentDirectory.GetModifiers().Select(activeModifier => activeModifier.GetDescription());
        if (GetStatusList(out string modifiersDescription, modifierDescriptions))
            result += $"Current modifiers:\n{modifiersDescription}\n";

        IEnumerable<string> conditionDescriptions = Player.I.GetAllConditions().Select(condition => condition.GetDescription());
        if (GetStatusList(out string conditionsDescription, conditionDescriptions))
            result += $"Current conditions:\n{conditionsDescription}\n";

        IEnumerable<string> componentIdentifiers = Player.I.GetInstalledComponents().Select(component => component.GetCurrentIdentifier());
        if (GetStatusList(out string componentIdentifiersDescription, componentIdentifiers))
            result += $"Installed components:\n{componentIdentifiersDescription}\n";
        return true;
    }

    private bool GetStatusList(out string result, IEnumerable<string> strings)
    {
        result = $"    {string.Join("\n    ", strings)}";
        if (strings.Count() == 0)
            return false;
        return true;
    }
}
