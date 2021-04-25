using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityComponent : ComponentWithIP
{
    public int speed;
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
    protected virtual void OnCommand(Command command, ParsedCommand parsedCommand)
    {

    }
    protected virtual void OnTerminalTimePast(int terminalTimePast)
    {
        Debug.Log("protected virtual void OnTerminalTimePast(int "+ terminalTimePast+")");
        currentMomentum += terminalTimePast;
        Debug.Log("currentMomentum " + currentMomentum);
        int turnsToTake = Mathf.FloorToInt((float)currentMomentum / (float)speed);
        Debug.Log("turnsToTake " + turnsToTake);
        currentMomentum -= turnsToTake * 2;
        Debug.Log("currentMomentum " + currentMomentum);
        for (int i = 0; i < turnsToTake; i++)
        {
            Run();
        }
    }
    private void Run()
    {
        Debug.Log("private void Run()");
    }
}
