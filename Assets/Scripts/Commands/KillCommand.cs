using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;

public class KillCommand : Command
{
    public override bool Run(out string result, ParsedCommand parsedCommand)
    {
        if (!base.Run(out result, parsedCommand))
            return false;
        ProcessHandler.I.DisableProcess(parsedCommand.arguments[0]);
        result = $"The process {parsedCommand.arguments[0]} was terminated";
        return true;
    }

    protected override bool ValidateParsedCommand(out string result, ParsedCommand parsedCommand)
    {
        if (!base.ValidateParsedCommand(out result, parsedCommand))
            return false;
        result = name + " requires the first argument to be a process id (pid)";
        if (!parsedCommand.HasArguments())
            return false;
        result = parsedCommand.arguments[0] + " is not a process id (pid)";
        if (!ArgumentIsPid(parsedCommand.arguments[0]))
            return false;
        return true;
    }

}
