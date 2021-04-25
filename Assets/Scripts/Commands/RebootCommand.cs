using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RebootCommand : Command
{
    public override bool Run(out string result, ParsedCommand parsedCommand)
    {
        if (!base.Run(out result, parsedCommand))
            return false;
        IOTerminal.I.Restart();
        return true;
    }
}
