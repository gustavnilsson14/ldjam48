using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementComponent : EntityComponent
{
    protected override void Run()
    {
        base.Run();
        if (!GetCurrentSensorTarget(out SensorComponent.TargetData target))
            return;
        if (target.lastPosition == entityBody.currentDirectory)
            return;
        Move(target.lastPosition);
    }
    protected virtual bool GetDestinationDirectory(out Directory directory) {
        directory = null;
        return true;
    }
    private void Move(Directory goal)
    {
        List<PathNode> path = entityBody.currentDirectory.FindPath(goal);
        if (path.Count == 0)
            return;
        entityBody.MoveTo(path[0].directory);
    }
    protected override void HandleNoSensor()
    {
        base.HandleNoSensor();
    }
}
