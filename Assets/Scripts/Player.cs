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

    // Start is called before the first frame update
    private void Awake()
    {
        Player.I = this;
        commands.AddRange(GetComponentsInChildren<Command>());
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
}
