using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentHandler : Handler
{
    public static ComponentHandler I;

    protected override void StartRegister()
    {
        base.StartRegister();
        IOTerminal.I.onTerminalTimePast.AddListener(OnTerminalTimePast);
    }
    private void OnTerminalTimePast(int timePast)
    {
        new List<IComponentIO>(HostHandler.I.currentHost.GetComponentsInChildren<IComponentIO>()).ForEach(io => UpdateIOInstance(io, timePast)); 
    }

    private void UpdateIOInstance(IComponentIO io, int timePast)
    {
        if (io.hackedTime <= 0)
            return;
        io.hackedTime = Mathf.Clamp(io.hackedTime - timePast,0,int.MaxValue);
    }
}
