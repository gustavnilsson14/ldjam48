using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatCommand : Command
{
    public override bool Run(out string result, ParsedCommand parsedCommand)
    {
        if (!base.Run(out result, parsedCommand))
            return false;
        ArgumentIsDiscoverable(parsedCommand.arguments[0], out IDiscoverable discoverable);
        result = DiscoveryHandler.I.GetCatDescription(discoverable);
        return true;
    }
    protected override bool ValidateParsedCommand(out string result, ParsedCommand parsedCommand)
    {
        if (!base.ValidateParsedCommand(out result, parsedCommand))
            return false;
        result = name + " requires the first argument to be an entity";
        if (!parsedCommand.HasArguments())
            return false;
        result = parsedCommand.arguments[0] + " is not available for cat";
        if (!ArgumentIsDiscoverable(parsedCommand.arguments[0]))
            return false;
        return true;
    }
}