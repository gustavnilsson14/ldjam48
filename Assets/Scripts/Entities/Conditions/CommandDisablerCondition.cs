using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandDisablerCondition : Condition, ICommandDisabler
{
    public string commandName;
    public Command command;

    protected override void Start()
    {
        base.Start();
        RegisterCommand();
    }
    public override void Init(IConditionOrigin origin, Condition clone = null)
    {
        base.Init(origin, clone);
        if (clone == null)
            return;
        commandName = (clone as CommandDisablerCondition).commandName;
        RegisterCommand();
    }
    private void RegisterCommand()
    {
        List<Command> commands = new List<Command>();
        commands.AddRange(Player.I.GetComponentsInChildren<Command>());
        command = commands.Find(c => c.name == commandName);
        if (command != null)
            return;
        int index = UnityEngine.Random.Range(0, commands.Count);
        command = commands[index];
    }
    public override string GetDescription()
    {
        return $"{GetSource()}: Deactives the use of the {command.name} while this condition is applied";
    }

    public Command GetCommand()
    {
        return command;
    }
}
