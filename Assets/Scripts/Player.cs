using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Player : Entity
{
    public static Player I;

    private List<Command> commands = new List<Command>();

    public int currentCharacters = 0;
    public int maxCharacters = 10000;
    public float currentSeconds = 0;
    private float maxSeconds = 60 * 60;

    public CommandEvent onCommand = new CommandEvent();

    public bool test = false;
    private float overTime;

    public List<Command> GetCommands()
    {
        return commands;
    }
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        Player.I = this;
        commands.AddRange(GetComponentsInChildren<Command>());
        FullRestore();
    }
    private void Start()
    {
        IOTerminal.I.onCommand.AddListener(OnCommand);
    }
    private void Update()
    {
        ReduceRealTime();
        if (!test)
            return;
        test = false;
        TakeDamage(1);
    }
    protected void OnCommand(Command command, ParsedCommand parsedCommand)
    {
        ReduceCharacters(parsedCommand.GetCommandString());
        onCommand.Invoke(command, parsedCommand);
    }
    private void ReduceRealTime()
    {
        currentSeconds -= Time.deltaTime;
        if (currentSeconds > 0)
            return;
        overTime += Time.deltaTime;
        if (overTime < 10)
            return;
        overTime = 0;
        TakeDamage(1);
    }
    private void ReduceCharacters(string command)
    {
        currentCharacters -= command.Length;
        if (currentCharacters > 0)
            return;
        TakeDamage(1);
    }

    public void FullRestore()
    {
        directoryHistory.Clear();
        currentIP = maxIP;
        currentCharacters = maxCharacters;
        currentSeconds = maxSeconds;
    }

    public static bool GetCommand(out Command command, string commandName, bool onlyAvailable = true)
    {
        command = Player.I.commands.Find(c => c.name == commandName && (!onlyAvailable || c.isAvailable));
        if (command == null)
            return false;
        return true;
    }
    public static List<Command> GetCommands(bool onlyAvailable = true)
    {
        List<Command> commands = new List<Command>();
        commands.AddRange(Player.I.commands.FindAll(c => (!onlyAvailable || c.isAvailable)));
        return commands;
    }
    public string GetTimeLeft()
    {
        TimeSpan t = TimeSpan.FromSeconds(Mathf.FloorToInt(currentSeconds));
        return string.Format("{0:D2}:{1:D2}:{2:D2}", t.Hours,t.Minutes,t.Seconds);
    }
    public override bool IsAllowedInDirectory(Directory directory)
    {
        if (base.IsAllowedInDirectory(directory))
            return true;
        if (HostHandler.I.currentHost.keys.Find(key => key.isAvailable && key.GetName() == directory.GetFullPath()))
            return true;
        return false;
    }
    public bool IsSafeInDirectory(Directory directory)
    {
        return (HostHandler.I.currentHost.keys.Find(key => key.isAvailable && key.GetName() == directory.GetFullPath()) != null);
    }
}
public class MoveEvent : UnityEvent<Directory, Directory> { }