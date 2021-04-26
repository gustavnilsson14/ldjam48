using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandDeactivator : DirectoryModifier
{
    public Command command;
    public string commandName = "";
    private void Start()
    {
        RegisterCommand();
    }

    private void RegisterCommand()
    {
        List<Command> commands = new List<Command>();
        commands.AddRange(Player.I.GetComponentsInChildren<Command>());
        command = commands.Find(c => c.name == commandName);
        if (command != null)
            return;
        int index = UnityEngine.Random.Range(0, commands.Count);
        command = commands[index];
    }
    public override string GetDescription()
    {
        return $"{GetSource()}\nDeactives the use of the {command.name} while this modifier is active";
    }
}
