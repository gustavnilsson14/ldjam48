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

    public bool test = false;
    public string value = "";
    public ParsedPath parsedPath;
    private void Update()
    {
        if (!test)
            return;
        test = false;
        parsedPath = new ParsedPath(Player.I.currentDirectory, value);
        if (parsedPath.error != "")
        {
            Debug.Log(parsedPath.error);
            return;
        }
        Debug.Log($"START {string.Join(" - ", parsedPath.directories.Select(d => d.name))} END");
    }
}
