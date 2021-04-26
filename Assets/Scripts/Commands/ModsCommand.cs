using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class ModsCommand : Command
{
    public override bool Run(out string result, ParsedCommand parsedCommand)
    {
        if (!base.Run(out result, parsedCommand))
            return false;
        foreach (DirectoryModifier directoryModifier in Player.I.activeModifiers)
        {
            result += directoryModifier.GetDescription();
        }
        return true;
    }
}
