using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Command : MonoBehaviour
{
    public bool isAvailable = false;
    [TextArea(2, 10)]
    public string helpText;

    public virtual bool Run(out string result, ParsedCommand parsedCommand)
    {
        return ValidateParsedCommand(out result, parsedCommand);
    }

    protected virtual bool ValidateParsedCommand(out string result, ParsedCommand parsedCommand) {
        result = "";
        return false;
    }

    public bool GetHelpText(out string helpText) {
        helpText = "";
        if (this.helpText == "")
            return false;
        helpText = name + " - " + this.helpText;
        return true;
    }

}
public class ParsedCommand
{
    public string name;
    public List<string> arguments = new List<string>();
    public List<string> flags = new List<string>();

    public ParsedCommand(string value) {
        List<string> commandSegments = new List<string>();
        commandSegments.AddRange(value.Split(new string[] { " " }, StringSplitOptions.None));
        if (commandSegments.Count == 0)
            return;
        ParseName(commandSegments);
        ParseArguments(commandSegments);
        ParseFlags(commandSegments);
    }
    private void ParseName(List<string> commandSegments)
    {
        name = commandSegments[0];
        commandSegments.RemoveAt(0);
    }
    private void ParseArguments(List<string> commandSegments)
    {
        arguments.AddRange(commandSegments.FindAll(segment => segment.Substring(0, 2) != "--"));
    }
    private void ParseFlags(List<string> commandSegments)
    {
        flags.AddRange(commandSegments.FindAll(segment => segment.Substring(0, 2) == "--"));
    }
    public string GetCommandString()
    {
        List<string> allStrings = new List<string>();
        allStrings.Add(name);
        allStrings.AddRange(arguments);
        allStrings.AddRange(flags);
        return string.Join(" ", allStrings);
    }
}