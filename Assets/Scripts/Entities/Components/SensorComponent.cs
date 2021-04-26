using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorComponent : EntityComponent
{
    public ComponentWithIP currentTarget;
    public int scanDepth = 0;
    public bool GetCurrentTarget(out ComponentWithIP target)
    {
        target = currentTarget;
        if (target == null)
            return false;
        return true;
    }
    protected override void Run()
    {
        base.Run();
        if (!ShouldFindNewTarget())
            return;
        currentTarget = null;
        EntityFaction myFaction = entityBody.faction;
        List<Directory> directories = entityBody.currentDirectory.GetDirectoriesByDepth(scanDepth);
        List<Entity> targets = new List<Entity>();
        foreach (Directory directory in directories)
        {
            targets.AddRange(directory.GetEntities().FindAll(e => e.faction != myFaction && !targets.Contains(e)));
        }
        if (targets.Contains(Player.I) && Player.I.IsSafeInDirectory(Player.I.currentDirectory))
            targets.Remove(Player.I);
        if (targets.Count == 0)
            return;
        System.Random random = new System.Random();
        int index = random.Next(targets.Count);
        currentTarget = targets[index];
    }

    private bool ShouldFindNewTarget()
    {
        if (currentTarget == null)
            return true;
        if (currentTarget.currentDirectory == entityBody.currentDirectory)
            return false;
        return true;
    }
}
