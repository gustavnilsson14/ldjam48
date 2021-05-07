using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UsermodCommand : Command
{
    public override bool Run(out string result, ParsedCommand parsedCommand)
    {
        if (!base.Run(out result, parsedCommand))
            return false;
        if (HandleListFlag(out result, parsedCommand))
            return true;
        ArgumentIsEntity(parsedCommand.arguments[0], out Entity targetEntity);
        ArgumentIsFaction(parsedCommand.arguments[1], out EntityFaction entityFaction);
        targetEntity.faction = entityFaction;
        result = $"{targetEntity.name} now belongs to the {entityFaction.ToString()} faction";
        return true;
    }
    private bool HandleListFlag(out string result, ParsedCommand parsedCommand) {
        result = "";
        if (!parsedCommand.flags.Contains("--list"))
            return false;
        result += $"Usergroups present in {HostHandler.I.currentHost.name};\n";
        string factionsList = string.Join("\n    ", HostHandler.I.currentHost.GetPresentFactions().Select(f => f.ToString()));
        result += $"\n    {factionsList}\n";
        return true;
    }
    protected override bool ValidateParsedCommand(out string result, ParsedCommand parsedCommand)
    {
        result = "";
        if (parsedCommand.flags.Contains("--list"))
            return true;
        if (!base.ValidateParsedCommand(out result, parsedCommand))
            return false;
        result = $"{name} requires the first argument to be a target file";
        if (!parsedCommand.HasArguments())
            return false;
        result = parsedCommand.arguments[0] + " is not a file";
        if (!ArgumentIsEntity(parsedCommand.arguments[0]))
            return false;
        result = $"{name} requires the second argument to be a usergroup";
        if (parsedCommand.arguments.Count < 2)
            return false;
        result = parsedCommand.arguments[1] + " is not a usergroup";
        if (!ArgumentIsFaction(parsedCommand.arguments[1]))
            return false;
        return true;
    }
}
