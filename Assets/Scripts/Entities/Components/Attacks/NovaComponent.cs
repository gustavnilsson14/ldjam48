using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NovaComponent : AttackComponent
{
    public int startWaveDistance = 0;
    public int maxWaveDistance = 0;
    private Directory novaOrigin;
    private int currentWaveDistance = 0;

    protected override void Run()
    {
        if (novaOrigin == null)
        {
            StartNova();
            return;
        }
        ExpandNova();
    }
    private void StartNova()
    {
        currentWaveDistance = startWaveDistance;
        novaOrigin = entityBody.currentDirectory;
        if (!entityBody.discovered)
            return;
        IOTerminal.I.AppendTextLine($"Nova started at {novaOrigin.GetFullPath()}!");
    }
    private void ExpandNova()
    {
        if (currentWaveDistance > maxWaveDistance)
        {
            DissipateNova();
            return;
        }
        foreach (Directory directory in novaOrigin.GetDirectoriesAtDepth(currentWaveDistance))
        {
            HitDirectory(directory);
        }
        currentWaveDistance++;
    }

    private void HitDirectory(Directory directory)
    {
        IOTerminal.I.AppendTextLine($"Nova hits at {directory.GetFullPath()}!");
        foreach (Entity entity in directory.GetEntities())
        {
            if (entity == entityBody)
                continue;
            DealDamage(entity);
        }
    }

    private void DissipateNova()
    {
        novaOrigin = null;
        IOTerminal.I.AppendTextLine($"Nova dissipated");
        currentWaveDistance = startWaveDistance;
    }
}
