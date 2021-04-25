using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackComponent : EntityComponent
{
    public int damageBase = 1;
    public int range = 0;

    protected override void Run()
    {
        base.Run();
        if (!GetSensorComponent(out SensorComponent sensorComponent))
        {
            HandleNoSensor();
            return;
        }
        if (!sensorComponent.GetCurrentTarget(out ComponentWithIP target))
            return;
        Attack(target);
    }

    protected void Attack(ComponentWithIP target) {
        if (!IsTargetInRange(target))
            return;
        target.TakeDamage(damageBase);
    }

    private bool IsTargetInRange(ComponentWithIP target)
    {
        List<Directory> directories = entityBody.currentDirectory.GetDirectoriesByDepth(range);
        List<Entity> targets = new List<Entity>();
        foreach (Directory directory in directories)
        {
            targets.AddRange(directory.GetEntities());
        }
        if (targets.Contains(target))
            return true;
        return false;
    }

    private void HandleNoSensor()
    {
        
    }
}
