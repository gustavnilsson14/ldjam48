using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookCommand : Command
{
    public Entity currentEntity;
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
        currentEntity = GetCurrentEntity(entities);
        currentEntity.Discover();
        result = $"{currentEntity.name}";
        return true;
    }
    private Directory GetLookDirectory(ParsedCommand parsedCommand) {
        if (!parsedCommand.HasArguments())
            return Player.I.currentDirectory;
        return Player.I.currentDirectory.GetAdjacentDirectories().Find(dir => dir.name == parsedCommand.arguments[0]);
    }

    private Entity GetCurrentEntity(List<Entity> entities) 
    {
        if (currentEntity == null)
            return entities[0];
        int entityIndex = entities.IndexOf(currentEntity);
        if (entityIndex == -1)
            return entities[0];
        entityIndex += 1;
        if (entityIndex < entities.Count)
            return entities[entityIndex];
        return entities[0];
    }
    protected override bool ValidateParsedCommand(out string result, ParsedCommand parsedCommand)
    {
        result = "";
        if (!parsedCommand.HasArguments())
            return true;
        result = parsedCommand.arguments[0] + " is not an adjacent directory";
        if (!GetAdjacentDirectory(parsedCommand.arguments[0], out Directory adjacentDirectory))
            return false;
        return true;
    }
}
