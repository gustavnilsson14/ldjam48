using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LsCommand : Command
{
    public override bool Run(out string result, ParsedCommand parsedCommand)
    {
        if (!base.Run(out result, parsedCommand))
            return false;
        List<Entity> entities = new List<Entity>();
        entities.AddRange(GetLookDirectory(parsedCommand).GetEntities());
        entities.Remove(Player.I);
        if (entities.Count == 0)
        {
            result = "No files in this directory";
            return true;
        }
        List<string> resultStrings = new List<string>();
        foreach (Entity entity in entities)
        {
            resultStrings.Add($"{entity.name} - {entity.description}");
            entity.Discover();
        }
        result = string.Join("\n", resultStrings);
        return true;
    }
    private Directory GetLookDirectory(ParsedCommand parsedCommand) {
        if (!parsedCommand.HasArguments())
            return Player.I.currentDirectory;
        return Player.I.currentDirectory.GetAdjacentDirectories().Find(dir => dir.name == parsedCommand.arguments[0]);
    }
    protected override bool ValidateParsedCommand(out string result, ParsedCommand parsedCommand)
    {
        result = "";
        if (!parsedCommand.HasArguments())
            return true;
        result = parsedCommand.arguments[0] + " is not an adjacent directory";
        if (!ArgumentIsAdjacentDirectory(parsedCommand.arguments[0]))
            return false;
        return true;
    }
}
