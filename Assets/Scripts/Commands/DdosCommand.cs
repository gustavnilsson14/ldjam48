using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DdosCommand : Command
{
    public int damageBase = 1;
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
        int damage = GetCurrentDamage(parsedCommand);
        bool stillAlive = target.TakeDamage(damage);
        string result = $"{target.GetName()} took {damage} IP damage";
        string verboseFlag = parsedCommand.flags.Find(flag => flag == "--verbose");

        if (verboseFlag != null) {
            string verbose = (stillAlive ? "\n{0} still has integrity" : "\n{0} crumbles into bits");
            result += (stillAlive ? $"\n{target.GetName()} still has integrity" : $"\n{target.GetName()} crumbles into bits");
        }
        if (!stillAlive)
            IOTerminal.I.destroyedEntities.Add(target.name);
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
}
