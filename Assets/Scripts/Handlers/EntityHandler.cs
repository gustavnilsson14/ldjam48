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
    public int GetUniqueId()
    {
        currentId++;
        return currentId;
    }
    public Entity GetEntityById(int id)
    {
        List<Entity> all = new List<Entity>(HostHandler.I.currentHost.GetComponentsInChildren<Entity>());
        return all.Find(component => component.uniqueId == id);
    }
}
