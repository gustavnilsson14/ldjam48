using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class PwdCommand : Command
{
    public override bool Run(out string result, ParsedCommand parsedCommand)
    {
        if (!base.Run(out result, parsedCommand))
            return false;
        result = Player.I.currentDirectory.GetFullPath();
        return true;
    }
}
