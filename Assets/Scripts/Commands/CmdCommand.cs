using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CmdCommand : Command
{
    public override void Run()
    {
        base.Run();
        Debug.Log("CmdCommand ");
        foreach (Command command in Player.GetCommands())
        {
            if (!command.GetHelpText(out string helpText))
                continue;
            IOTerminal.I.AppendTextLine(helpText);
        }
    }
}
