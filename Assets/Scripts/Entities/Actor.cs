using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : ComponentWithIP
{
    public int speed = 4;
    public bool isRealTime = false;
    protected float currentMomentum = 0;

    public override void StartRegister()
    {
        base.StartRegister();
        IOTerminal.I.onCommand.AddListener(OnCommand);
        IOTerminal.I.onTerminalTimePast.AddListener(OnTerminalTimePast);
        Player.I.onRealTime.AddListener(OnRealTime);
    }
    protected virtual void OnCommand(Command command, ParsedCommand parsedCommand) { }
    protected virtual void OnTerminalTimePast(int terminalTimePast)
    {
        if (isRealTime)
            return;
        OnTimePast(terminalTimePast);
    }
    protected void OnRealTime()
    {
        if (!isRealTime)
            return;
        OnTimePast(Time.deltaTime);
    }
    protected virtual void OnTimePast(float time) {
        if (speed == 0)
            return;
        currentMomentum += time;
        int turnsToTake = Mathf.FloorToInt((float)currentMomentum / (float)speed);
        currentMomentum -= turnsToTake * speed;
        for (int i = 0; i < turnsToTake; i++)
        {
            Run();
        }
    }
    protected virtual void Run() { }
}
