using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EntityHandler : Handler
{
    public static EntityHandler I;
    private int currentId = 0;
    public string GetUniqueId(Entity entity)
    {
        currentId++;
        return $"{entity.name}-{currentId}";
    }
    public bool GetEntityById(out Entity entity, string id)
    {
        List<Entity> all = new List<Entity>(HostHandler.I.currentHost.GetComponentsInChildren<Entity>());
        entity = all.Find(component => component.uniqueId == id);
        return entity != null;
    }
    public bool CreateChallengeAt(Directory directory, IChallenge challenge, out IChallenge newChallenge) {
        newChallenge = challenge.AddToDirectory(directory);
        return newChallenge != null;
    }
    public bool InstantiateEntity(Directory directory, GameObject entityPrefab, out Entity newEntity) {
        GameObject newInstance = Instantiate(entityPrefab, directory.transform);
        if (!newInstance.TryGetComponent<Entity>(out newEntity))
        {
            Destroy(newInstance);
            return false;
        }
        return true;
    }
}
public interface IChallenge
{
    float GetChallengeRating();
    IChallenge AddToDirectory(Directory directory);
}