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
    protected List<Entity> entitiesAffected = new List<Entity>();

    private void Start()
    {
        IOTerminal.I.onCommand.AddListener(OnCommand);
        IOTerminal.I.onTerminalTimePast.AddListener(OnTerminalTimePast);
        Player.I.onRealTime.AddListener(OnRealTime);
    }

    internal void DestroyMe()
    {
        foreach (Entity entity in entitiesAffected)
        {
            entity.activeModifiers.Remove(this);
        }
        Destroy(gameObject);
    }

    public void Register() {
        //RegisterAffectedDirectories must be on top
        RegisterAffectedDirectories();

        RegisterAffectedEntities();
        RegisterEvents();
    }
    public void RegisterAffectedDirectories()
    {
        if (!GetMyDirectory(out directory))
        {
            Destroy(this);
            return;
        }
        affectedDirectories.AddRange(directory.GetComponentsInChildren<Directory>());
    }

    private void RegisterAffectedDirectories(Directory arg0, Directory arg1)
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
        entity.onMove.AddListener(RegisterAffectedDirectories);
        return true;
    }

    public virtual string GetDescription()
    {
        return "";
    }

    private void RegisterEvents()
    {
        directory.onEntityEnter.AddListener(OnEntityEnterDirectory);
        directory.onEntityExit.AddListener(OnEntityExitDirectory);
    }
    private void RegisterAffectedEntities()
    {
        entitiesAffected.Clear();
        foreach (Entity entity in GetAffectedEntities())
        {
            entitiesAffected.Add(entity);
            entity.EnteredModifierZone(this);
            if (entity == Player.I)
                IOTerminal.I.AppendTextLine("The permissions feels different in this directory");
        }
    }
    private void OnEntityExitDirectory(Directory from, Directory to, Entity entity)
    {
        if (!directory.GetClosestParent(out Directory parentDirectory))
            return;
        if (to != parentDirectory)
            return;
        entitiesAffected.Remove(entity);
        entity.ExitModifierZone(this);
    }
    private void OnEntityEnterDirectory(Directory from, Directory to, Entity entity)
    {
        if (entitiesAffected.Contains(entity))
            return;
        if (entity == Player.I)
            IOTerminal.I.AppendTextLine("The permissions feels different in this directory");
        entitiesAffected.Add(entity);
        entity.EnteredModifierZone(this);
    }

    protected virtual void OnRealTime() { }
    protected virtual void OnTerminalTimePast(int time) {  }
    protected virtual void OnCommand(Command command, ParsedCommand parsedCommand) { }
    protected virtual List<Entity> GetAffectedEntities() {
        List<Entity> result = new List<Entity>();
        foreach (Directory directory in affectedDirectories)
        {
            result.AddRange(directory.GetEntities());
        }
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
}
