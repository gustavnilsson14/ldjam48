using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class FindCommand : Command
{
    public override bool Run(out string result, ParsedCommand parsedCommand)
    {
        if (!base.Run(out result, parsedCommand))
            return false;
        result = "Find results;\n";
        string search = parsedCommand.arguments[0];
        List<GameObject> results = new List<GameObject>();
        foreach (ComponentWithIP componentWithIP in HostHandler.I.currentHost.GetComponentsInChildren<ComponentWithIP>())
        {
            if (!componentWithIP.name.Contains(search))
                continue;
            if (results.Contains(componentWithIP.gameObject))
                continue;
            results.Add(componentWithIP.gameObject);
            result += $"{componentWithIP.currentDirectory.GetFullPath()}/{componentWithIP.name}\n";
        }
        int damage = 4;
        if (parsedCommand.flags.Contains("--reduceCost"))
        {
            damage = 3;
        }
        damage -= search.Length;
        if (damage < 1)
            return true;
        //Player.I.TakeDamage(damage, "Search");
        return true;
    }
    protected override bool ValidateParsedCommand(out string result, ParsedCommand parsedCommand)
    {
        if (!base.ValidateParsedCommand(out result, parsedCommand))
            return false;
        result = $"{name} requires the first argument to be a substring of what to find";
        if (!parsedCommand.HasArguments())
            return false;
        result = $"{name} cannot be used with more than {maxFlags} flags";
        if (parsedCommand.flags.Count > maxFlags)
            return false;
        return true;
    }
}
