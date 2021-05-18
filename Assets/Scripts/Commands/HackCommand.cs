using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HackCommand : Command, IComponentIO
{
    public int hackedTime { get; set; }

    public override bool Run(out string result, ParsedCommand parsedCommand)
    {
        if (!base.Run(out result, parsedCommand))
            return false;

        ArgumentIsEntity(parsedCommand.arguments[0], out Entity targetEntity);
        ArgumentIsEntityComponent(parsedCommand.arguments[1], targetEntity, out EntityComponent entityComponent);

        Debug.Log($"entityComponent {parsedCommand.arguments[1]} {entityComponent} ");
        if (entityComponent == null)
            return false;
        entityComponent.onOutput.AddListener(OnInput);
        entityComponent.OnInput(this, parsedCommand.arguments[2]);
        entityComponent.hackedTime = 20;
        result = $"Manual entry of \"{parsedCommand.arguments[2]}\" to {entityComponent.GetCurrentIdentifier()} on {targetEntity.GetName()}";
        return true;
    }

    protected override bool ValidateParsedCommand(out string result, ParsedCommand parsedCommand)
    {

        if (!base.ValidateParsedCommand(out result, parsedCommand))
            return false;
        result = $"{name} requires the first argument to be a target file";
        if (!parsedCommand.HasArguments())
            return false;
        result = parsedCommand.arguments[0] + " is not a file";
        if (!ArgumentIsEntity(parsedCommand.arguments[0], out Entity targetEntity))
            return false;

        result = $"{name} requires the second argument to be an installed component";
        if (parsedCommand.arguments.Count < 2)
            return false;
        result = $"{parsedCommand.arguments[1]} is not an installed component on {targetEntity.GetName()}";
        if (!ArgumentIsEntityComponent(parsedCommand.arguments[1], targetEntity))
            return false;

        result = $"{name} requires the third argument to be a path, with optional file ending";
        if (parsedCommand.arguments.Count < 3)
            return false;
        return true;
    }
    public void OnInput(IComponentIO source, string input)
    {
        IOTerminal.I.AppendTextLine(input);
        source.GetOnOutputEvent().RemoveListener(OnInput);
    }
    public IOEvent GetOnOutputEvent()
    {
        return null;
    }
}
