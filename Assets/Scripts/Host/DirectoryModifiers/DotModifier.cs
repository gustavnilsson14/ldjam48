using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotModifier : DirectoryModifier
{
    public int damage = 1;
    public float speed = 1;
    public float momentum;
    protected override void OnRealTime()
    {
        base.OnRealTime();
        momentum += Time.deltaTime;
        if (momentum < speed)
            return;
        momentum = 0;
        List<Entity> deadEntities = new List<Entity>();
        foreach (Entity entity in entitiesAffected)
        {
            if (entity.TakeDamage(damage))
                continue;
            deadEntities.Add(entity);
        }
        entitiesAffected.RemoveAll(entity => deadEntities.Contains(entity));
    }
    public override string GetDescription()
    {
        return $"{GetSource()}\nEach {speed} seconds deals {damage} to all entities within";
    }
}
