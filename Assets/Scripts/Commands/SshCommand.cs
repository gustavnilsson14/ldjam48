using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SshCommand : Command
{
    public override bool Run(out string result, ParsedCommand parsedCommand)
    {
        if (!base.Run(out result, parsedCommand))
            return false;
        return true;
    }
    protected override bool ValidateParsedCommand(out string result, ParsedCommand parsedCommand)
    {
        result = name + " requires the first argument to be a \"username@hostname\" pair for which you have aquired a public key";
        if (!parsedCommand.HasArguments())
            return true;
        result = parsedCommand.arguments[0] + " is not a \"username@hostname\" pair";
        if (!ArgumentIsUserHostPair(parsedCommand.arguments[0]))
            return false;
        return true;
    }
}
