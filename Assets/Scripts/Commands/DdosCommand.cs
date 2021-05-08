using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DdosCommand : Command, IDamageSource
{
    public int damageBase = 1;
    private int currentTotalDamage;

    public override bool Run(out string result, ParsedCommand parsedCommand)
    {
        if (!base.Run(out result, parsedCommand))
            return false;
        ComponentWithIP target = GetTarget(parsedCommand);
        result = ApplyTo(target, parsedCommand);
        return true;
    }
    private string ApplyTo(ComponentWithIP target, ParsedCommand parsedCommand)
    {
        currentTotalDamage = GetCurrentDamage(parsedCommand);
        bool isDead = target.TakeHit(this, out int armorDamageTaken, out int bodyDamageTaken);
        bool isVerbose = (parsedCommand.flags.Find(flag => flag == "--verbose") != null);
        if (isDead)
            IOTerminal.I.destroyedEntities.Add(target.name);
        return GetOutput(isVerbose, isDead, target, armorDamageTaken, bodyDamageTaken);
    }

    private string GetOutput(bool isVerbose, bool isDead, ComponentWithIP target, int armorDamageTaken, int bodyDamageTaken)
    {
        string result = $"{target.GetName()} recieved a force of {StringUtil.ColorWrap($"{currentTotalDamage} IP damage", Palette.RED)}";
        if (!isVerbose)
            return result;
        if (armorDamageTaken > 0)
            result += $"\nThe IP armor of {target.GetName()} was {StringUtil.ColorWrap($"interrupted by {armorDamageTaken}", Palette.TEAL)}";
        if (bodyDamageTaken > 0)
            result += $"\nThe IP of {target.GetName()} was {StringUtil.ColorWrap($"interrupted by {bodyDamageTaken}", Palette.MAGENTA)}";
        result += (isDead ? $"\n{target.GetName()} {StringUtil.ColorWrap($"crumbles into bits", Palette.RED)}" : $"\n{target.GetName()} still has integrity");
        return result;
    }

    private int GetCurrentDamage(ParsedCommand parsedCommand)
    {
        int result = Mathf.FloorToInt((float)damageBase * Player.I.GetDamageMultiplier());
        if (parsedCommand.flags.Find(flag => flag == "--heavy") != null)
            result *= 2;
        if (parsedCommand.flags.Find(flag => flag == "--strong") != null)
            result += 2;
        return result;
    }
    private ComponentWithIP GetTarget(ParsedCommand parsedCommand) {
        List<Entity> entities = Player.I.currentDirectory.GetEntities();
        Entity targetEntity = entities.Find(entity => entity.name == parsedCommand.arguments[0]);
        if (parsedCommand.arguments.Count == 1)
            return targetEntity;
        ArgumentIsEntityComponent(parsedCommand.arguments[1], targetEntity, out EntityComponent targetComponent);
        return targetComponent;
    }
    protected override bool ValidateParsedCommand(out string result, ParsedCommand parsedCommand)
    {
        if (!base.ValidateParsedCommand(out result, parsedCommand))
            return false;
        result = $"{name} requires the first argument to be a target file or process id (pid)";
        if (!parsedCommand.HasArguments())
            return false;
        result = parsedCommand.arguments[0] + " is not a file or process id (pid)";
        if (!ArgumentIsEntity(parsedCommand.arguments[0], out Entity targetEntity))
            return false;
        result = $"{name} cannot be used with more than {maxFlags} flags";
        if (parsedCommand.flags.Count > maxFlags)
            return false;
        if (parsedCommand.arguments.Count == 1)
            return true;
        result = $"{parsedCommand.arguments[1]} is not a component of the target";
        if (!ArgumentIsEntityComponent(parsedCommand.arguments[1], targetEntity))
            return false;
        return true;
    }

    public override int GetTerminalTimePast(ParsedCommand parsedCommand)
    {
        int speed = this.speed;
        if (parsedCommand.flags.Contains("--quick"))
            speed -= 2;
        if (parsedCommand.flags.Contains("--heavy"))
            speed += 1;
        return Mathf.Clamp(speed, 1, int.MaxValue);
    }

    public int GetDamageBase()
    {
        return damageBase;
    }

    public int GetTotalDamage()
    {
        return currentTotalDamage;
    }

    public string GetDamageSourceName()
    {
        return "A ddos command";
    }
}
