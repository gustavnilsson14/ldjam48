using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookCommand : Command
{
    public IDiscoverable currentDiscovery;
    public override bool Run(out string result, ParsedCommand parsedCommand)
    {
        if (!base.Run(out result, parsedCommand))
            return false;
        List<IDiscoverable> discoverables = new List<IDiscoverable>(GetTargetDirectory(parsedCommand).GetDiscoverables());
        if (discoverables.Count == 0)
        {
            result = "No files in this directory";
            return true;
        }
        currentDiscovery = GetCurrentDiscoverable(discoverables);
        DiscoveryHandler.I.Discover(currentDiscovery);
        result = $"{currentDiscovery.GetName()}";
        return true;
    }
    private Directory GetTargetDirectory(ParsedCommand parsedCommand) {
        if (!parsedCommand.HasArguments())
            return Player.I.currentDirectory;
        return Player.I.currentDirectory.GetAdjacentDirectories().Find(dir => dir.name == parsedCommand.arguments[0]);
    }

    private IDiscoverable GetCurrentDiscoverable(List<IDiscoverable> discoverables) 
    {
        if (currentDiscovery == null)
            return discoverables[0];
        int discoveryIndex = discoverables.IndexOf(currentDiscovery);
        if (discoveryIndex == -1)
            return discoverables[0];
        discoveryIndex += 1;
        if (discoveryIndex < discoverables.Count)
            return discoverables[discoveryIndex];
        return discoverables[0];
    }
    protected override bool ValidateParsedCommand(out string result, ParsedCommand parsedCommand)
    {
        if (!base.ValidateParsedCommand(out result, parsedCommand))
            return false;
        result = "";
        if (!parsedCommand.HasArguments())
            return true;
        result = parsedCommand.arguments[0] + " is not an adjacent directory";
        if (!GetAdjacentDirectory(parsedCommand.arguments[0], out Directory adjacentDirectory))
            return false;
        return true;
    }
}
