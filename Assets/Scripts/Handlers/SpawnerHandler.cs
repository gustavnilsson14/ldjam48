using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SpawnerHandler : Handler
{
    public static SpawnerHandler I;

    protected override void StartRegister()
    {
        base.StartRegister();
    }

    public void InitSpawner(ISpawner spawner)
    {
        spawner.offspring = new List<GameObject>();
        spawner.currentDirectory = spawner.GetGameObject().GetComponentInParent<Directory>();
        spawner.currentMomentum = spawner.GetSpawnFrequency();
        IOTerminal.I.onTerminalTimePast.AddListener(spawner.OnTerminalTimePast);

        if (spawner is IEntitySpawner)
        {
            InitEntitySpawner(spawner as IEntitySpawner);
            return;
        }
        InitEntityComponentSpawner(spawner as IEntityComponentSpawner);
    }

    public void InitEntitySpawner(IEntitySpawner entitySpawner)
    {
        entitySpawner.onSpawn = new EntitySpawnEvent();
    }

    public void InitEntityComponentSpawner(IEntityComponentSpawner entityComponentSpawner)
    {
        entityComponentSpawner.onSpawn = new EntityComponentSpawnEvent();
        entityComponentSpawner.offspring = new List<GameObject>();
    }

    public bool SpawnEntity(out List<Entity> entities, IEntitySpawner spawner)
    {
        entities = new List<Entity>();
        if (!CanHaveMoreOffspring(spawner))
            return false;
        if (!spawner.GetSpawnPrefabs(out List<GameObject> prefabs))
            return false;
        foreach (GameObject prefab in prefabs)
        {
            entities.Add(Instantiate(prefab, spawner.currentDirectory.transform).GetComponent<Entity>());
        }
        spawner.offspring.AddRange(entities.Select(e => e.GetGameObject()));
        return entities.Count > 0;
    }

    public bool SpawnEntityComponent(out List<EntityComponent> entityComponents, IEntityComponentSpawner spawner, Entity recipient = null)
    {
        entityComponents = new List<EntityComponent>();
        if (!CanHaveMoreOffspring(spawner))
            return false;
        if (recipient == null)
        {
            //CREATE PICKUP
            return true;
        }
        foreach (EntityComponent component in spawner.GetEntityComponents())
        {
            EntityComponent newEntityComponent = recipient.GetGameObject().AddComponent(component.GetType()) as EntityComponent;
            ReflectionUtil.CopyObjectValues(component, newEntityComponent);
            spawner.onSpawn.Invoke(spawner, newEntityComponent);
            entityComponents.Add(newEntityComponent);
        }
        return entityComponents.Count > 0;
    }

    private bool CanHaveMoreOffspring(ISpawner spawner)
    {
        return spawner.offspring.FindAll(o => o != null).Count < spawner.GetMaxOffspring() || spawner.GetMaxOffspring() == 0;
    }
}

public interface ISpawner
{
    void InitSpawner();
    GameObject GetGameObject();
    int GetSpawnFrequency();
    void OnTerminalTimePast(int time);
    int GetMaxOffspring();
    List<GameObject> offspring { get; set; }
    Directory currentDirectory { get; set; }
    int currentMomentum { get; set; }
}
public interface IEntitySpawner : ISpawner
{
    EntitySpawnEvent onSpawn { get; set; }
    bool GetSpawnPrefabs(out List<GameObject> result);
}

public interface IEntityComponentSpawner : ISpawner
{
    EntityComponentSpawnEvent onSpawn { get; set; }
    List<EntityComponent> GetEntityComponents();
}

public class EntitySpawnEvent : UnityEvent<IEntitySpawner, Entity> { }
public class EntityComponentSpawnEvent : UnityEvent<IEntityComponentSpawner, EntityComponent> { }