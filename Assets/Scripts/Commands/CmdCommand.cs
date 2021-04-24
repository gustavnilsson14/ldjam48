using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CmdCommand : Command
{
    public override void Run(ParsedCommand parsedCommand)
    {
        base.Run(parsedCommand);
        foreach (Command command in Player.GetCommands())
        {
            if (!command.GetHelpText(out string helpText))
                continue;
            IOTerminal.I.AppendTextLine(helpText);
        }
    }
}
