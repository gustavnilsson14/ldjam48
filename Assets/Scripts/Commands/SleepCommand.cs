using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SleepCommand : Command
{
    public override bool Run(out string result, ParsedCommand parsedCommand)
    {
        if (!base.Run(out result, parsedCommand))
            return false;
        int.TryParse(parsedCommand.arguments[0], out int time);
        Player.I.PauseRealTime(time);
        return true;
    }
    public override int GetTerminalTimePast(ParsedCommand parsedCommand)
    {
        int.TryParse(parsedCommand.arguments[0], out int argument);
        float speedMultiplier = (float)argument;
        speedMultiplier = Mathf.Clamp(1 + argument/5, 1, Mathf.Infinity);
        int commandSpeed = Mathf.FloorToInt((float)speed * speedMultiplier);
        return Mathf.Clamp(commandSpeed, 1, int.MaxValue);
    }
    protected override bool ValidateParsedCommand(out string result, ParsedCommand parsedCommand)
    {
        if (!base.ValidateParsedCommand(out result, parsedCommand))
            return false;
        result = name + " requires the first argument to be an integer (a number without decimal)";
        if (!parsedCommand.HasArguments())
            return false;
        result = parsedCommand.arguments[0] + " is not an integer (a number without decimal)";
        if (!int.TryParse(parsedCommand.arguments[0], out int argument))
            return false;
        return true;
    }

}
