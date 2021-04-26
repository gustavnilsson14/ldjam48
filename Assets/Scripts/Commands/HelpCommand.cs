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
        if (!parsedCommand.HasArguments())
            return true;
        Player.GetCommand(out Command command, parsedCommand.arguments[0]);
        result = command.GetExtendedHelpText();
        return true;

    }
    protected override bool ValidateParsedCommand(out string result, ParsedCommand parsedCommand)
    {
        if (!base.ValidateParsedCommand(out result, parsedCommand))
            return false;
        result = "";
        if (!parsedCommand.HasArguments())
            return true;
        result = parsedCommand.arguments[0] + " command not found";
        if (!Player.GetCommand(out Command command, parsedCommand.arguments[0]))
            return false;
        result = parsedCommand.arguments[0] + " cannot be the help command";
        if (command == this)
            return false;
        return true;
    }
}
