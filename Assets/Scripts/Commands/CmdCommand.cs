using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CmdCommand : Command
{
    public override bool Run(out string result, ParsedCommand parsedCommand)
    {
        if (!base.Run(out result, parsedCommand))
            return false;
        List<string> helpTexts = new List<string>();
        foreach (Command command in Player.GetCommands())
        {
            if (!command.GetHelpText(out string helpText))
                continue;
            helpTexts.Add(helpText);
        }
        result = string.Join("\n", helpTexts);
        return true;
    }
}
