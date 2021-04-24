using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player I;
    
    public int currentIP = 10;
    public int maxIP = 10;
    private List<Command> commands = new List<Command>();
    public Directory currentDirectory;
    public int currentCharacters = 10000;
    private float currentSeconds = 60 * 60;

    // Start is called before the first frame update
    private void Awake()
    {
        Player.I = this;
        commands.AddRange(GetComponentsInChildren<Command>());
    }
    private void Start()
    {
        IOTerminal.I.onCommand.AddListener(OnCommand);
    }

    private void OnCommand(Command command, ParsedCommand parsedCommand)
    {
        currentCharacters -= parsedCommand.GetCommandString().Length;
    }

    private void Update()
    {
        currentSeconds -= Time.deltaTime;
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
