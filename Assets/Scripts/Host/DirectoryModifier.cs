using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectoryModifier : MonoBehaviour, IProcess, IChallenge
{
    [Header("Challenge")]
    [Range(1, 100)]
    public float challengeRating = 1;

    [Header("DirectoryModifier")]
    public int currentProcsOnEntities = -1;
    protected Directory directory;
    private string pid;
    private bool isDisabled = false;

    protected virtual void Start()
    {
        IOTerminal.I.onCommand.AddListener(OnCommand);
        IOTerminal.I.onTerminalTimePast.AddListener(OnTerminalTimePast);
        Player.I.onRealTime.AddListener(OnRealTime);
        Register();
    }
    public virtual void Register()
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

    public void SetPid(string pid)
    {
        this.pid = pid;
    }

    public string GetPid()
    {
        return this.pid;
    }

    public void Disable()
    {
        isDisabled = true;
    }

    public bool IsDisabled()
    {
        return isDisabled;
    }

    public float GetChallengeRating()
    {
        return challengeRating;
    }

    public IChallenge AddToDirectory(Directory directory)
    {
        return directory.gameObject.AddComponent(this.GetType()) as IChallenge;
    }

    public bool RequiresPrefab()
    {
        return false;
    }
}
