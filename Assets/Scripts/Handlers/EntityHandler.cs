using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EntityHandler : MonoBehaviour
{
    public static EntityHandler I;
    private int currentId = 0;
    private void Awake()
    {
        EntityHandler.I = this;
    }
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
}

public interface IChallenge
{
    float GetChallengeRating();
}