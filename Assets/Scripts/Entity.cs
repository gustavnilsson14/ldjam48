using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public Directory currentDirectory;
    private void Awake()
    {
        currentDirectory = GetComponentInParent<Directory>();
    }
    private void Start()
    {
        IOTerminal.I.onCommand.AddListener(OnCommand);
        IOTerminal.I.onTerminalTimePast.AddListener(OnTerminalTimePast);
    }
    protected virtual void OnCommand(Command command, ParsedCommand parsedCommand)
    {
        Debug.Log("OnCommand " + command.name + ")");
    }
    protected virtual void OnTerminalTimePast(int terminalTimePast)
    {
        Debug.Log("OnTerminalTimePast " + terminalTimePast + ")");
    }
}
