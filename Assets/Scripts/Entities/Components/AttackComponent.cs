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
        if (parsedInput == null)
            return;
        Attack();
    }
    protected virtual void Attack()
    {
        if (!IsDirectoryInRange(parsedInput.GetLastDirectory()))
            return;
        if (!parsedInput.GetLastDirectory().GetEntities().Contains(parsedInput.entity))
            return;
        entityBody.Attack();
        DealDamage(parsedInput.entity);
    }

    protected virtual void DealDamage(Entity target)
    {
        if (!target.TakeHit(this))
            return;
        parsedInput = null;
    }

    protected virtual bool IsDirectoryInRange(Directory targetDirectory)
    {
        if (targetDirectory == null)
            return false;
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
