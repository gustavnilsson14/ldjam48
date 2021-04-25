using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Player : Entity
{
    public static Player I;

    private List<Command> commands = new List<Command>();

    public int currentCharacters = 10000;
    private float currentSeconds = 60 * 60;

    [Header("Events")]
    public MoveEvent onMove = new MoveEvent();

    // Start is called before the first frame update
    protected override void Awake()
    {
        Player.I = this;
        currentIP = maxIP;
        commands.AddRange(GetComponentsInChildren<Command>());
    }
    private void Start()
    {
        IOTerminal.I.onCommand.AddListener(OnCommand);
    }
    protected void OnCommand(Command command, ParsedCommand parsedCommand)
    {
        currentCharacters -= parsedCommand.GetCommandString().Length;
    }
    private void Update()
    {
        currentSeconds -= Time.deltaTime;
    }
    public void MoveTo(Directory directory)
    {
        Player.I.currentDirectory = directory;
        onMove.Invoke(directory);
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
}
public class MoveEvent : UnityEvent<Directory> { }