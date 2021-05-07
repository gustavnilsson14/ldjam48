using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackComponent : EntityComponent, IDamageSource
{
    public int damageBase = 1;
    public int range = 0;

    protected override void Run()
    {
        base.Run();
        if (!GetCurrentSensorTarget(out SensorComponent.TargetData targetData))
            return;
        Attack(targetData);
    }
    protected override void HandleNoSensor()
    {
        base.HandleNoSensor();
    }

    protected virtual void Attack(SensorComponent.TargetData targetData)
    {
        if (!IsDirectoryInRange(targetData.lastPosition))
            return;
        if (!targetData.lastPosition.GetEntity(out Entity target, targetData.targetId))
            return;
        entityBody.Attack();
        DealDamage(target);
    }

    protected virtual void DealDamage(Entity target)
    {
        target.TakeHit(this);
    }

    protected virtual bool IsDirectoryInRange(Directory targetDirectory)
    {
        return entityBody.currentDirectory.GetDirectoriesByDepth(range).Contains(targetDirectory);
    }
    
    public int GetDamageBase()
    {
        return damageBase;
    }

    public int GetTotalDamage()
    {
        return Mathf.FloorToInt((float)damageBase * entityBody.GetDamageMultiplier());
    }
    public string GetDamageSourceName()
    {
        if (!entityBody.isDiscovered)
            return "Something";
        return $"The {GetCurrentIdentifier()}-component on {name}";
    }
}
