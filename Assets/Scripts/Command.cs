﻿using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Command : MonoBehaviour
{
    public bool isAvailable = false;
    [TextArea(2, 10)]
    public string helpText;
    public int speed = 2;
    public virtual bool Run(out string result, ParsedCommand parsedCommand)
    {
        return ValidateParsedCommand(out result, parsedCommand);
    }

    protected virtual bool ValidateParsedCommand(out string result, ParsedCommand parsedCommand) {
        result = "";
        return true;
    }

    public bool GetHelpText(out string helpText) {
        helpText = "";
        if (this.helpText == "")
            return false;
        helpText = name + " - " + this.helpText;
        return true;
    }

    public int GetTerminalTimePast(ParsedCommand parsedCommand)
    {
        return speed;
    }
    protected bool ArgumentIsAdjacentDirectory(string argument)
    {
        List<Directory> adjacentDirectories = Player.I.currentDirectory.GetAdjacentDirectories();
        Directory adjacentDirectory = adjacentDirectories.Find(dir => dir.name == argument);
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