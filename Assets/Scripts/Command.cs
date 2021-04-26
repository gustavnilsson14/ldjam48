using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Command : MonoBehaviour
{
    public bool isAvailable = false;
    [TextArea(2, 10)]
    public string helpText;
    [TextArea(2, 20)]
    public string extendedHelpText;
    public int speed = 2;
    public int maxFlags = 1;

    public virtual bool Run(out string result, ParsedCommand parsedCommand)
    {
        return ValidateParsedCommand(out result, parsedCommand);
    }
    protected virtual bool ValidateParsedCommand(out string result, ParsedCommand parsedCommand) {
        result = "";
        bool validationResult = true;
        List<DirectoryModifier> modifiers = Player.I.activeModifiers.FindAll(mod => mod is CommandDeactivator);
        foreach (DirectoryModifier modifier in modifiers)
        {
            CommandDeactivator deactivator = modifier as CommandDeactivator;
            if (deactivator.command != this)
                continue;
            result += result = $"{deactivator.GetSource()} deactivates {name}\n";
            validationResult = false;
        }
        return validationResult;
    }

    public bool GetHelpText(out string helpText) {
        helpText = "";
        if (this.helpText == "")
            return false;
        helpText = name + " - " + this.helpText;
        return true;
    }
    public virtual string GetExtendedHelpText()
    {
        return helpText + "\n" + extendedHelpText;
    }
    public virtual int GetTerminalTimePast(ParsedCommand parsedCommand)
    {
        return speed;
    }
    protected bool GetAdjacentDirectory(string argument, out Directory adjacentDirectory)
    {
        List<Directory> adjacentDirectories = Player.I.currentDirectory.GetAdjacentDirectories();
        adjacentDirectory = adjacentDirectories.Find(dir => dir.name == argument);
        if (adjacentDirectory == null)
            return false;
        return true;
    }
    protected bool ArgumentIsEntity(string argument)
    {
        List<Entity> entities = Player.I.currentDirectory.GetEntities();
        Entity target = entities.Find(entity => entity.name == argument);
        if (target == null)
            return false;
        return true;
    }
    protected bool ArgumentIsUserHostPair(string argument)
    {
        if (HostHandler.I.currentHost.keys.Find(key => key.GetName() == argument) != null)
            return true;
        return false;
    }

    protected bool ArgumentIsPathFromRoot(string argument) {
        return HostHandler.I.currentHost.GetDirectoryByPath(argument, out Directory directory);
    }

    public CommandEntity InstantiateEntity(Transform parent)
    {
        return Instantiate(HostHandler.I.commandEntityPrefab, parent);
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
        foreach(string segment in commandSegments)
        {
            if (segment.Length < 2)
            {
                arguments.Add(segment);
                continue;
            }
            if(segment.Substring(0, 2) != "--")
                arguments.Add(segment);
        }
        //arguments.AddRange(commandSegments.FindAll(segment => segment.Substring(0, 2) != "--"));
    }
    private void ParseFlags(List<string> commandSegments)
    {
        foreach (string segment in commandSegments)
        {
            if (segment.Length < 2)
                continue;

            if (segment.Substring(0, 2) == "--")
                flags.Add(segment);
        }
        //flags.AddRange(commandSegments.FindAll(segment => segment.Substring(0, 2) == "--"));
    }
    public bool HasArguments()
    {
        return (arguments.Count > 0);
    }
    public bool HasFlags()
    {
        return (flags.Count > 0);
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