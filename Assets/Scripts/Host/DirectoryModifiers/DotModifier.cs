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
        foreach (Entity entity in entitiesAffected)
        {
            entity.TakeDamage(damage);
        }
    }
    public override string GetDescription()
    {
        return $"{directory.GetFullPath()}\nEach {speed} seconds deals {damage} to all entities within";
    }
}
