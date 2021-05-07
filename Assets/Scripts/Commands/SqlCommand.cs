using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SqlCommand : Command
{
    public override bool Run(out string result, ParsedCommand parsedCommand)
    {
        if (!base.Run(out result, parsedCommand))
            return false;
        if (HandleNewFlag(out result, parsedCommand))
            return true;
        if (HandleListFlag(out result, parsedCommand))
            return true;
        switch (parsedCommand.arguments[0])
        {
            case "install":
                return HandleInstallArgument(out result, parsedCommand);
            case "uninstall":
                return HandleUnInstallArgument(out result, parsedCommand);
            case "topup":
                return HandleTopUp(out result, parsedCommand);
            case "expend":
                return HandleExpend(out result, parsedCommand);
        }
        return false;
    }

    private bool HandleExpend(out string result, ParsedCommand parsedCommand)
    {
        if (!HandleGenericConsumable<Expendable>(out result, out StoredObject storedObject, parsedCommand))
            return false;
        result = $"Could not expend key";
        return HandleUseConsumable(out result, storedObject);
    }

    private bool HandleTopUp(out string result, ParsedCommand parsedCommand)
    {
        if (!HandleGenericConsumable<TopUp>(out result, out StoredObject storedObject, parsedCommand))
            return false;
        result = $"Could not use topup code";
        return HandleUseConsumable(out result, storedObject);
    }

    private bool HandleGenericConsumable<T>(out string result, out StoredObject storedObject, ParsedCommand parsedCommand)
    {
        result = "";
        storedObject = null;
        if (!ValidateConsumableArgumentCommand<T>(out result, parsedCommand))
            return false;
        Player.I.GetComponent<SqlComponent>(out SqlComponent sqlComponent);
        sqlComponent.FetchItem(out storedObject, parsedCommand.arguments[1]);
        return true;
    }

    private bool HandleUseConsumable(out string result, StoredObject storedObject)
    {
        result = "";
        Consumable consumable = Player.I.gameObject.AddComponent(storedObject.objectType) as Consumable;
        if (consumable == null)
            return false;
        consumable.Consume(out result);
        return true;
    }

    private bool HandleInstallArgument(out string result, ParsedCommand parsedCommand)
    {
        result = "";
        if (!ValidateInstallArgumentCommand(out result, parsedCommand))
            return false;

        Player.I.GetComponent<SqlComponent>(out SqlComponent sqlComponent);
        sqlComponent.FetchItem(out StoredObject storedObject, parsedCommand.arguments[1]);
        result = $"Could not install component";
        if (!Player.I.InstallComponent(out EntityComponent installedComponent, storedObject))
            return false;
        result = $"Successfully installed {installedComponent.GetCurrentIdentifier()}";
        return true;
    }

    private bool HandleUnInstallArgument(out string result, ParsedCommand parsedCommand)
    {
        result = "";
        if (!ValidateUnInstallArgumentCommand(out result, parsedCommand))
            return false;
        EntityComponent entityComponent = Player.I.GetInstalledComponents().Find(component => component.GetCurrentIdentifier() == parsedCommand.arguments[1]);
        result = $"Successfully uninstalled {entityComponent.GetCurrentIdentifier()}";
        Player.I.UnInstallComponent(entityComponent);
        return true;
    }

    private bool HandleListFlag(out string result, ParsedCommand parsedCommand)
    {
        result = "";
        if (!parsedCommand.flags.Contains("--list"))
            return false;
        result += $"Sql database contains;";
        Player.I.GetComponent<SqlComponent>(out SqlComponent sqlComponent);
        if (sqlComponent.storedComponents.Count == 0)
        {
            result = $"Sql database is empty...";
            return true;
        }
        result += $"\n{sqlComponent.GetStackedListView()}";/*
        List<string> listEntries = new List<string>();
        foreach (StoredObject item in sqlComponent.storedComponents)
        {
            listEntries.Add($"    {item.id["name"]} {item.id["pickupType"]}");
        }
        result += $"\n{string.Join("\n", listEntries)}";*/
        return true;
    }
    private bool HandleNewFlag(out string result, ParsedCommand parsedCommand)
    {
        result = "";
        if (!parsedCommand.flags.Contains("--new"))
            return false;
        Player.I.gameObject.AddComponent<SqlComponent>();
        result = $"Generated a new sql component";
        return true;

    }
    protected override bool ValidateParsedCommand(out string result, ParsedCommand parsedCommand)
    {
        if (!base.ValidateParsedCommand(out result, parsedCommand))
            return false;
        if (parsedCommand.flags.Contains("--new"))
            return true;
        result = $"Your sql component has been deleted. Run 'sql --new' to generate a new one!";
        if (!Player.I.GetComponent<SqlComponent>(out SqlComponent sqlComponent))
            return false;
        return true;
    }

    private bool ValidateInstallArgumentCommand(out string result, ParsedCommand parsedCommand)
    {
        result = $"When passing {parsedCommand.arguments[0]} as the first argument, {name} requires the second argument to be a component stored in your sql database";
        if (parsedCommand.arguments.Count < 2)
            return false;
        result = $"{parsedCommand.arguments[1]} is not a component stored in your sql database";
        Player.I.GetComponent<SqlComponent>(out SqlComponent sqlComponent);
        if (sqlComponent.storedComponents.Find(item => $"{item.id["name"]}" == parsedCommand.arguments[1] && ReflectionUtil.IsSubClassOrClass<EntityComponent>(item.objectType)) == null)
            return false;
        return true;
    }
    private bool ValidateUnInstallArgumentCommand(out string result, ParsedCommand parsedCommand)
    {
        result = $"When passing {parsedCommand.arguments[0]} as the first argument, {name} requires the second argument to be an installed component";
        if (parsedCommand.arguments.Count < 2)
            return false;
        result = $"{parsedCommand.arguments[1]} is not an installed component";
        if (Player.I.GetInstalledComponents().Find(component => component.GetCurrentIdentifier() == parsedCommand.arguments[1]) == null)
            return false;
        return true;
    }

    private bool ValidateConsumableArgumentCommand<T>(out string result, ParsedCommand parsedCommand)
    {
        result = $"When passing {parsedCommand.arguments[0]} as the first argument, {name} requires the second argument to be a topup code stored in your sql database";
        if (parsedCommand.arguments.Count < 2)
            return false;
        result = $"{parsedCommand.arguments[1]} is not a topup code stored in your sql database";
        Player.I.GetComponent<SqlComponent>(out SqlComponent sqlComponent);
        if (sqlComponent.storedComponents.Find(item => item.id["name"] == parsedCommand.arguments[1] && ReflectionUtil.IsSubClassOrClass<T>(item.objectType)) == null)
            return false;
        return true;
    }
}
