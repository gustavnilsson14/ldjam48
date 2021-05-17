using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoCommand : Command
{
    protected Command currentCommand;
    protected ParsedCommand parsedCommand;
    protected bool runOnArmorDamage = false;
    protected bool runOnBodyDamage = false;
    protected bool runOnMove = false;
    protected bool runOnCommand = false;
    protected bool runOnEntityEnterMyDirectory = false;
    private float lastExecution;

    private void Start()
    {
        Player.I.onArmorDamage.AddListener(OnArmorDamage);
        Player.I.onBodyDamage.AddListener(OnBodyDamage);
        Player.I.onMove.AddListener(OnMove);
        Player.I.onCommand.AddListener(OnCommand);
        Player.I.onEntityEnterMyDirectory.AddListener(OnEntityEnterMyDirectory);
    }

    public override bool Run(out string result, ParsedCommand parsedCommand)
    {
        Reset();
        if (!base.Run(out result, parsedCommand))
            return false;
        Player.GetCommand(out currentCommand, parsedCommand.arguments[0]);
        this.parsedCommand = new ParsedCommand(parsedCommand.GetCommandString());
        this.parsedCommand.name = this.parsedCommand.arguments[0];
        this.parsedCommand.arguments.RemoveAt(0);
        runOnArmorDamage = parsedCommand.flags.Contains("--onArmorDamage");
        runOnBodyDamage = parsedCommand.flags.Contains("--onBodyDamage");
        runOnMove = parsedCommand.flags.Contains("--onMove");
        runOnCommand = parsedCommand.flags.Contains("--onCommand");
        runOnEntityEnterMyDirectory = parsedCommand.flags.Contains("--onEntityEnterMyDirectory");
        result = $"{this.parsedCommand.name} will now run {string.Join(", ", parsedCommand.flags)}";
        return true;
    }

    private void Reset()
    {
        currentCommand = null;
        parsedCommand = null;
        runOnArmorDamage = false;
        runOnBodyDamage = false;
        runOnMove = false;
        runOnCommand = false;
        runOnEntityEnterMyDirectory = false;
    }

    protected override bool ValidateParsedCommand(out string result, ParsedCommand parsedCommand)
    {
        if (!base.ValidateParsedCommand(out result, parsedCommand))
            return false;
        result = name + " requires the first argument to be another command";
        if (!parsedCommand.HasArguments())
            return false;
        result = parsedCommand.arguments[0] + " command not found";
        if (!Player.GetCommand(out Command command, parsedCommand.arguments[0]))
            return false;
        result = "auto requires at least 1 flag";
        if (parsedCommand.flags.Count == 0)
            return false;
        result = string.Format("auto cannot be used with more than {0} flags", maxFlags);
        if (parsedCommand.flags.Count > maxFlags)
            return false;
        result = parsedCommand.arguments[0] + " cannot be the auto command";
        if (command == this)
            return false;
        return true;
    }
    private void RunAutoCommand()
    {
        if (PreventRecursion())
            return;
        currentCommand.Run(out string result, this.parsedCommand);
        IOTerminal.I.AppendTextLine($"AutoCommand: {currentCommand.name}");
        IOTerminal.I.AppendTextLine($"{result}");
        IOTerminal.I.AppendTextLine($"AutoCommand End");
    }

    private bool PreventRecursion()
    {
        float now = Time.time;
        if (now - 1 < lastExecution)
            return true;
        lastExecution = now;
        return false;
    }
    private void OnArmorDamage(ArmorComponent arg0, bool arg1, int arg2)
    {
        if (!runOnArmorDamage)
            return;
        RunAutoCommand();
    }
    private void OnBodyDamage(bool arg0, int arg1)
    {
        if (!runOnBodyDamage)
            return;
        RunAutoCommand();
    }
    private void OnMove(Directory directory, Directory prevDir)
    {
        if (!runOnMove)
            return;
        RunAutoCommand();
    }

    private void OnCommand(Command command, ParsedCommand parsedCommand)
    {
        if (!runOnCommand)
            return;
        RunAutoCommand();
    }
    
    private void OnEntityEnterMyDirectory(Directory arg0, Directory arg1, Entity arg2)
    {
        if (!runOnEntityEnterMyDirectory)
            return;
        RunAutoCommand();
    }

    public override float GetRarity() => 5;

}
