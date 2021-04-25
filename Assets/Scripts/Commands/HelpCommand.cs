using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpCommand : Command
{
    [TextArea(1,20)]
    public string help;
    public override bool Run(out string result, ParsedCommand parsedCommand)
    {
        if (!base.Run(out result, parsedCommand))
            return false;
        result = string.Format(help, Player.I.GetCurrentIP(), Player.I.currentCharacters, Player.I.GetTimeLeft());
        return true;
    }
}
