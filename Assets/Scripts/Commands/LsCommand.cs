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
        List<string> resultStrings = new List<string>();

        foreach (Directory directory in Player.I.currentDirectory.GetAdjacentDirectories())
        {
            resultStrings.Add(StringUtil.ColorWrap(directory.name, Palette.BLUE));
        }

        foreach (Entity entity in entities)
        {
            resultStrings.Add(entity.name);
            entity.Discover();
        }
        result = string.Join(" ", resultStrings);
        return true;
    }
    private Directory GetLookDirectory(ParsedCommand parsedCommand) {
        if (!parsedCommand.HasArguments())
            return Player.I.currentDirectory;
        return Player.I.currentDirectory.GetAdjacentDirectories().Find(dir => dir.name == parsedCommand.arguments[0]);
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
