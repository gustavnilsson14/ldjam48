using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityComponent : ComponentWithIP
{
    public int speed = 4;
    protected float currentMomentum = 0;
    protected Entity entityBody;

    protected override void Awake()
    {
        base.Awake();
        entityBody = GetComponent<Entity>();
    }
    private void Start()
    {
        IOTerminal.I.onCommand.AddListener(OnCommand);
        IOTerminal.I.onTerminalTimePast.AddListener(OnTerminalTimePast);
    }
    protected virtual bool GetSensorComponent(out SensorComponent sensorComponent) {
        sensorComponent = GetComponent<SensorComponent>();
        if (sensorComponent == null)
            return false;
        return true;
    }
    protected virtual void OnCommand(Command command, ParsedCommand parsedCommand)
    {

    }
    protected virtual void OnTerminalTimePast(int terminalTimePast)
    {
        if (speed == 0)
            return;
        currentMomentum += terminalTimePast;
        int turnsToTake = Mathf.FloorToInt((float)currentMomentum / (float)speed);
        currentMomentum -= turnsToTake * speed;
        for (int i = 0; i < turnsToTake; i++)
        {
            Run();
        }
    }
    protected virtual void Run()
    {
        
    }
}
