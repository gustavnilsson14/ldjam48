﻿using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectoryModifier : MonoBehaviour
{
    public List<Directory> affectedDirectories = new List<Directory>();
    public int currentProcsOnEntities = -1;
    protected Directory directory;

    private void Start()
    {
        IOTerminal.I.onCommand.AddListener(OnCommand);
        IOTerminal.I.onTerminalTimePast.AddListener(OnTerminalTimePast);
        Player.I.onRealTime.AddListener(OnRealTime);
        Register();
    }
    public void Register()
    {
        if (GetMyDirectory(out directory))
            return;
        Destroy(this);
    }
    private void OnMove(Directory arg0, Directory arg1)
    {
        Register();
    }
    private bool GetMyDirectory(out Directory directory)
    {
        directory = GetComponent<Directory>();
        if (directory != null)
            return true;
        Entity entity = GetComponent<Entity>();
        if (entity == null)
            return false;
        directory = entity.currentDirectory;
        entity.onMove.AddListener(OnMove);
        return true;
    }
    public virtual string GetDescription()
    {
        return "";
    }
    protected virtual List<Entity> GetAffectedEntities() {
        List<Entity> result = new List<Entity>();
        result.AddRange(directory.GetComponentsInChildren<Entity>());
        return result;
    }
    protected bool CheckProcLimit()
    {
        if (currentProcsOnEntities == -1)
            return true;
        if (currentProcsOnEntities == 0)
            return false;
        currentProcsOnEntities = Mathf.Clamp(currentProcsOnEntities - GetAffectedEntities().Count, 0, int.MaxValue);
        return true;
    }
    public string GetSource() 
    {
        Entity entity = GetComponent<Entity>();
        if (entity != null)
            return entity.isDiscovered ? entity.name : "Something";
        return directory.GetFullPath();
    }
    protected virtual void OnRealTime() { }
    protected virtual void OnTerminalTimePast(int time) { }
    protected virtual void OnCommand(Command command, ParsedCommand parsedCommand) { }
}
