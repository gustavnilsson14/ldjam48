using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ComponentCommand : Command, IComponentIO
{
    public enum OperationType { 
        NONE, MANUAL, BINDING, CLEAR
    }
    public override bool Run(out string result, ParsedCommand parsedCommand)
    {
        if (!base.Run(out result, parsedCommand))
            return false;
        ArgumentIsEntityComponent(parsedCommand.arguments[0], Player.I, out EntityComponent entityComponent);
        ArgumentIsOperation(parsedCommand.arguments[1], out OperationType operationType);
        switch (operationType)
        {
            case OperationType.MANUAL:
                if (!HandleManualOperation(out result, entityComponent, parsedCommand))
                    return false;
                break;
            case OperationType.BINDING:
                if (!HandleBindingOperation(out result, entityComponent, parsedCommand))
                    return false;
                break;
            case OperationType.CLEAR:
                HandleClearOperation(out result, entityComponent, parsedCommand);
                break;
        }
        return true;
    }

    private bool HandleManualOperation(out string result, EntityComponent entityComponent, ParsedCommand parsedCommand)
    {
        if (!ValidateManualOperation(out result, parsedCommand))
            return false;
        entityComponent.onOutput.AddListener(OnInput);
        entityComponent.OnInput(this, parsedCommand.arguments[2]);
        result = $"Manual entry of \"{parsedCommand.arguments[2]}\" to {entityComponent.GetCurrentIdentifier()}";
        return true;
    }

    private bool HandleBindingOperation(out string result, EntityComponent entityComponent, ParsedCommand parsedCommand)
    {
        if (!ValidateBindingOperation(out result, parsedCommand))
            return false;
        ArgumentIsEntityComponent(parsedCommand.arguments[2], Player.I, out EntityComponent recieverEntityComponent);
        Player.I.ConnectComponentIO(entityComponent, recieverEntityComponent);
        result = $"Connected input socket of {recieverEntityComponent.GetCurrentIdentifier()} to listen to output socket of {entityComponent.GetCurrentIdentifier()}";
        return true;
    }

    private bool HandleClearOperation(out string result, EntityComponent entityComponent, ParsedCommand parsedCommand)
    {
        result = $"Removed all listeners to the output of {entityComponent.GetCurrentIdentifier()}";
        entityComponent.onOutput.RemoveAllListeners();
        return true;
    }

    private bool ValidateManualOperation(out string result, ParsedCommand parsedCommand)
    {
        result = $"{name} requires the third argument to be a path, with optional file ending";
        if (parsedCommand.arguments.Count < 3)
            return false;
        return true;
    }
    private bool ValidateBindingOperation(out string result, ParsedCommand parsedCommand)
    {
        result = $"{name} requires the third argument to be the installed recieving component";
        if (parsedCommand.arguments.Count < 3)
            return false;
        result = $"{parsedCommand.arguments[2]} is not an installed component";
        if (!ArgumentIsEntityComponent(parsedCommand.arguments[2], Player.I))
            return false;
        return true;
    }

    protected override bool ValidateParsedCommand(out string result, ParsedCommand parsedCommand)
    {
        if (!base.ValidateParsedCommand(out result, parsedCommand))
            return false;
        result = $"{name} requires the first argument to be an installed component";
        if (!parsedCommand.HasArguments())
            return false;
        result = $"{parsedCommand.arguments[0]} is not an installed component";
        if (!ArgumentIsEntityComponent(parsedCommand.arguments[0], Player.I))
            return false;

        result = $"{name} requires the second argument to be an operation";
        if (parsedCommand.arguments.Count < 2)
            return false;
        result = $"{parsedCommand.arguments[1]} is not an operation of the component command";
        if (!ArgumentIsOperation(parsedCommand.arguments[1]))
            return false;
        return true;
    }

    private bool ArgumentIsOperation(string argument)
    {
        return ArgumentIsOperation(argument, out OperationType operationType);
    }
    private bool ArgumentIsOperation(string argument, out OperationType operationType)
    {
        operationType = (OperationType)Enum.Parse(typeof(OperationType), argument.ToUpper());
        return operationType != OperationType.NONE;
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
