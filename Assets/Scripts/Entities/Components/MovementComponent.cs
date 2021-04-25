using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementComponent : EntityComponent
{
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
        Move(target);
    }
    private void Move(ComponentWithIP target)
    {
        List<PathNode> path = entityBody.currentDirectory.FindPath(target.currentDirectory);
        if (path.Count == 0)
            return;
        entityBody.MoveTo(path[0].directory);
    }

    private void HandleNoSensor()
    {

    }
}
