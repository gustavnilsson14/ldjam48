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
        List<Entity> entities = Player.I.currentDirectory.GetEntities();
        if (entities.Count == 0)
        {
            result = "No files in this directory";
            return true;
        }
        currentEntity = GetCurrentEntity(entities);
        result = currentEntity.name;
        return true;
    }

    private Entity GetCurrentEntity(List<Entity> entities) 
    {
        if (currentEntity == null)
        {
            return entities[0];
        }
        int entityIndex = entities.IndexOf(currentEntity);
        if (entityIndex == -1)
        {
            return entities[0];
        }
        entityIndex += 1;
        if (entityIndex < entities.Count)
        {
            return entities[entityIndex];
        }
        return entities[0];
    }
}
