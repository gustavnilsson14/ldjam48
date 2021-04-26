using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Directory))]
public class DirectoryModifier : MonoBehaviour
{
    public List<Directory> affectedDirectories = new List<Directory>();
    public string description;
    public int currentProcsOnEntities = -1;
    private Directory directory;
    protected List<Entity> entitiesAffected = new List<Entity>();

    private void Start()
    {
        IOTerminal.I.onCommand.AddListener(OnCommand);
        IOTerminal.I.onTerminalTimePast.AddListener(OnTerminalTimePast);
        Player.I.onRealTime.AddListener(OnRealTime);
    }
    public void Register() {
        //RegisterAffectedDirectories must be on top
        RegisterAffectedDirectories();

        RegisterAffectedEntities();
        RegisterEvents();
    }
    public void RegisterAffectedDirectories()
    {
        affectedDirectories.AddRange(GetComponentsInChildren<Directory>());
        directory = GetComponent<Directory>();
    }
    private void RegisterEvents()
    {
        directory.onEntityEnter.AddListener(OnEntityEnterDirectory);
        directory.onEntityExit.AddListener(OnEntityExitDirectory);
    }
    private void RegisterAffectedEntities()
    {
        foreach (Entity entity in GetAffectedEntities())
        {
            entitiesAffected.Add(entity);
            entity.EnteredModifierZone(this);
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
