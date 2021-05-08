using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementComponent : EntityComponent
{
    protected override void Run()
    {
        base.Run();
        if (parsedInput == null)
            return;
        Move();
    }
    private void Move()
    {
        Directory goal = parsedInput.GetLastDirectory();
        if (goal == null)
            return;
        if (goal == entityBody.currentDirectory)
            return;
        List<PathNode> path = entityBody.currentDirectory.FindPath(goal);
        if (path.Count == 0)
            return;
        entityBody.MoveTo(path[0].directory);
        if (goal != entityBody.currentDirectory)
            return;
        parsedInput = null;
    }
}
