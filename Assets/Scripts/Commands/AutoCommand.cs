using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoCommand : Command
{
    protected Command currentCommand;
    protected ParsedCommand parsedCommand;
    protected bool runOnTakeDamage = false;
    protected bool runOnMove = false;
    protected bool runOnCommand = false;
    private void Start()
    {
        Player.I.onTakeDamage.AddListener(OnTakeDamage);
        Player.I.onMove.AddListener(OnMove);
        Player.I.onCommand.AddListener(OnCommand);
    }

    public override bool Run(out string result, ParsedCommand parsedCommand)
    {
        if (!base.Run(out result, parsedCommand))
            return false;
        Player.GetCommand(out currentCommand, parsedCommand.arguments[0]);
        this.parsedCommand = new ParsedCommand(parsedCommand.GetCommandString());
        this.parsedCommand.name = this.parsedCommand.arguments[0];
        this.parsedCommand.arguments.RemoveAt(0);
        runOnTakeDamage = parsedCommand.flags.Contains("--onTakeDamage");
        runOnMove= parsedCommand.flags.Contains("--onMove");
        runOnCommand = parsedCommand.flags.Contains("--onCommand");
        result = $"{this.parsedCommand.name} will now run {parsedCommand.flags[0]}";
        return true;
    }
    protected override bool ValidateParsedCommand(out string result, ParsedCommand parsedCommand)
    {
        result = name + " requires the first argument to be another command";
        if (!parsedCommand.HasArguments())
            return false;
        result = parsedCommand.arguments[0] + " command not found";
        if (!Player.GetCommand(out Command command, parsedCommand.arguments[0]))
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
        currentCommand.Run(out string result, this.parsedCommand);
        IOTerminal.I.AppendTextLine($"AutoCommand: {result}");
    }
    private void OnTakeDamage(int damage)
    {
        if (!runOnTakeDamage)
            return;
        RunAutoCommand();
    }
    private void OnMove(Directory directory)
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
}
