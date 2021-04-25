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
        List<Entity> entities = Player.I.currentDirectory.GetEntities();
        Entity target = entities.Find(entity => entity.name == parsedCommand.arguments[0]);
        result = ApplyTo(target, parsedCommand);
        return true;
    }
    private string ApplyTo(Entity target, ParsedCommand parsedCommand)
    {
        int damage = GetCurrentDamage(parsedCommand);
        bool stillAlive = target.TakeDamage(damage);
        string result = string.Format("{0} took {1} IP damage", target.name, damage);
        string verboseFlag = parsedCommand.flags.Find(flag => flag == "--verbose");

        if (verboseFlag != null) {
            string verbose = (stillAlive ? "\n{0} still has integrity" : "\n{0} crumbles into bits");
            result += string.Format(verbose, target.name);
        }   
        return result;
    }
    private int GetCurrentDamage(ParsedCommand parsedCommand)
    {
        int result = damageBase;
        if (parsedCommand.flags.Find(flag => flag == "--heavy") != null)
            result *= 2;
        if (parsedCommand.flags.Find(flag => flag == "--strong") != null)
            result += 2;
        return result;

    }
    protected override bool ValidateParsedCommand(out string result, ParsedCommand parsedCommand)
    {
        result = "ddos requires the first argument to be a target file or process id (pid)";
        if (!parsedCommand.HasArguments())
            return false;
        result = parsedCommand.arguments[0] + " is not a file or process id (pid)";
        if (!ArgumentIsEntity(parsedCommand.arguments[0]))
            return false;
        result = string.Format("ddos cannot be used with more than {0} flags", maxFlags);
        if (parsedCommand.flags.Count > maxFlags)
            return false;
        return true;
    }

    public override int GetTerminalTimePast(ParsedCommand parsedCommand)
    {
        if (parsedCommand.flags.Contains("--quick"))
            return 1;
        if (parsedCommand.flags.Contains("--heavy"))
            return speed+1;
        return speed;
    }
}
