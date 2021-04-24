using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCommand : Command
{
    public override bool Run(out string result, ParsedCommand parsedCommand)
    {
        if (!base.Run(out result, parsedCommand))
            return false;
        IOTerminal.I.ClearOutput();
        return true;
    }
}
