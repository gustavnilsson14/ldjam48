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
    public int level = 1;

    public GameObject effectPrefab;

    public virtual bool Run(out string result, ParsedCommand parsedCommand)
    {
        bool validation = ValidateParsedCommand(out result, parsedCommand);
        if (validation)
            RenderEffect();
        return validation;
    }
    private void RenderEffect() {

        if (effectPrefab == null)
            return;
        GameObject newEffect = Instantiate(effectPrefab, EntityWorldHandler.I.commandEffectTransform);
        Destroy(newEffect, 10);
    }

    public virtual void LevelUp()
    {
        level++;
        maxFlags++;
        speed--;
    }
    protected virtual bool ValidateParsedCommand(out string result, ParsedCommand parsedCommand) {
        result = "";
        bool validationResult = true;
        List<ICommandDisabler> disablers = Player.I.GetActiveICommandDisablers();
        foreach (ICommandDisabler disabler in disablers)
        {
            if (disabler.GetCommand() != this)
                continue;
            result += result = $"{disabler.GetSource()} deactivates {name}\n";
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
        return Mathf.Clamp(speed,1,int.MaxValue);
    }
    protected bool GetAdjacentDirectory(string argument, out Directory adjacentDirectory)
    {
        List<Directory> adjacentDirectories = Player.I.currentDirectory.GetAdjacentDirectories();
        adjacentDirectory = adjacentDirectories.Find(dir => dir.name == argument);
        if (adjacentDirectory == null)
            return false;
        return true;
    }
    public CommandEntity InstantiateEntity(Transform parent)
    {
        return Instantiate(HostHandler.I.commandEntityPrefab, parent);
    }
    protected bool ArgumentIsEntity(string argument)
    {
        return ArgumentIsEntity(argument, out Entity entity);
    }
    protected bool ArgumentIsEntity(string argument, out Entity entity)
    {
        List<Entity> entities = Player.I.currentDirectory.GetEntities();
        entity = entities.Find(e => e.name == argument);
        return (entity != null);
    }
    protected bool ArgumentIsEntityComponent(string argument, Entity targetEntity)
    {
        return ArgumentIsEntityComponent(argument, targetEntity, out EntityComponent targetComponent);
    }
    protected bool ArgumentIsEntityComponent(string argument, Entity targetEntity, out EntityComponent targetComponent)
    {
        targetComponent = null;
        foreach (EntityComponent entityComponent in targetEntity.GetComponents<EntityComponent>())
        {
            if (entityComponent.GetComponentId() != argument)
                continue;
            targetComponent = entityComponent;
        }
        return (targetComponent != null);
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
    protected bool ArgumentIsPid(string argument)
    {
        return ProcessHandler.I.GetProcess(out IProcess process, argument);
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