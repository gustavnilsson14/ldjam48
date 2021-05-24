using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum Conversion
{
    NONE,
    IP_TO_FP, IP_TO_CHARACTERS, IP_TO_TIME,
    FP_TO_IP, FP_TO_CHARACTERS, FP_TO_TIME,
    CHARACTERS_TO_IP, CHARACTERS_TO_FP, CHARACTERS_TO_TIME,
    TIME_TO_IP, TIME_TO_FP, TIME_TO_CHARACTERS
}

public class ConvertCommand : Command, IDamageSource
{
    public float ipValue = 1;
    public float fpValue = 4;
    public float characterValue = 0.01f;
    public float timeValue = 0.01f;
    public float conversionRatio = 0.8f;
    private int totalDamage;

    public override bool Run(out string result, ParsedCommand parsedCommand)
    {
        if (!base.Run(out result, parsedCommand))
            return false;
        Convert(out result, parsedCommand.arguments[0], parsedCommand.arguments[1], parsedCommand.arguments[2]);
        return true;
    }

    private void Convert(out string result, string from, string to, string amountString)
    {
        result = "";
        if (!int.TryParse(amountString, out int inputAmount))
            return;
        int fromAmount = 0;
        int toAmount = 0;
        switch (GetConversionType(from,to))
        {
            case Conversion.IP_TO_FP:
                GetConversionValues(inputAmount, Player.I.currentIP, ipValue, fpValue, out fromAmount, out toAmount);
                totalDamage = fromAmount;
                DamageHandler.I.TakeDirectDamage(Player.I, this);
                Player.I.currentFP += toAmount;
                break;
            case Conversion.IP_TO_CHARACTERS:
                GetConversionValues(inputAmount, Player.I.currentIP, ipValue, characterValue, out fromAmount, out toAmount);
                totalDamage = fromAmount;
                DamageHandler.I.TakeDirectDamage(Player.I, this);
                Player.I.currentCharacters += toAmount;
                break;
            case Conversion.IP_TO_TIME:
                GetConversionValues(inputAmount, Player.I.currentIP, ipValue, timeValue, out fromAmount, out toAmount);
                totalDamage = fromAmount;
                DamageHandler.I.TakeDirectDamage(Player.I, this);
                Player.I.currentSeconds += toAmount;
                break;

            case Conversion.FP_TO_IP:
                GetConversionValues(inputAmount, Player.I.currentFP, fpValue, ipValue, out fromAmount, out toAmount);
                Player.I.currentFP -= fromAmount;
                DamageHandler.I.Heal(Player.I, toAmount);
                break;
            case Conversion.FP_TO_CHARACTERS:
                GetConversionValues(inputAmount, Player.I.currentFP, fpValue, characterValue, out fromAmount, out toAmount);
                Player.I.currentFP -= fromAmount;
                Player.I.currentCharacters += toAmount;
                break;
            case Conversion.FP_TO_TIME:
                GetConversionValues(inputAmount, Player.I.currentFP, fpValue, timeValue, out fromAmount, out toAmount);
                Player.I.currentFP -= fromAmount;
                Player.I.currentSeconds += toAmount;
                break;

            case Conversion.CHARACTERS_TO_IP:
                GetConversionValues(inputAmount, Player.I.currentCharacters, characterValue, ipValue, out fromAmount, out toAmount);
                Player.I.currentCharacters -= fromAmount;
                DamageHandler.I.Heal(Player.I, toAmount);
                break;
            case Conversion.CHARACTERS_TO_FP:
                GetConversionValues(inputAmount, Player.I.currentCharacters, characterValue, fpValue, out fromAmount, out toAmount);
                Player.I.currentCharacters -= fromAmount;
                Player.I.currentFP += toAmount;
                break;
            case Conversion.CHARACTERS_TO_TIME:
                GetConversionValues(inputAmount, Player.I.currentCharacters, characterValue, timeValue, out fromAmount, out toAmount);
                Player.I.currentCharacters -= fromAmount;
                Player.I.currentSeconds += toAmount;
                break;

            case Conversion.TIME_TO_IP:
                GetConversionValues(inputAmount, (int)Player.I.currentSeconds, timeValue, ipValue, out fromAmount, out toAmount);
                Player.I.currentSeconds -= fromAmount;
                DamageHandler.I.Heal(Player.I, toAmount);
                break;
            case Conversion.TIME_TO_FP:
                GetConversionValues(inputAmount, (int)Player.I.currentSeconds, timeValue, fpValue, out fromAmount, out toAmount);
                Player.I.currentSeconds -= fromAmount;
                Player.I.currentFP += toAmount;
                break;
            case Conversion.TIME_TO_CHARACTERS:
                GetConversionValues(inputAmount, (int)Player.I.currentSeconds, timeValue, characterValue, out fromAmount, out toAmount);
                Player.I.currentSeconds -= fromAmount;
                Player.I.currentCharacters += toAmount;
                break;
            default:
                break;
        }
        result = $"Converted {amountString} {from} into {toAmount} {to}";
    }

    private void GetConversionValues(int inputAmount, int amountAvailable, float fromMultipler, float toMultipler, out int fromAmount, out int toAmount) {
        fromAmount = Mathf.Clamp(inputAmount, 0, amountAvailable);
        float toFloat = (((float)fromAmount * fromMultipler) / toMultipler) * conversionRatio;
        toAmount = Mathf.FloorToInt(toFloat);
    }

    protected override bool ValidateParsedCommand(out string result, ParsedCommand parsedCommand)
    {
        result = "";
        if (!base.ValidateParsedCommand(out result, parsedCommand))
            return false;
        result = $"{name} requires the first argument to be ip, fp, characters, or time";
        if (!parsedCommand.HasArguments())
            return false;
        result = parsedCommand.arguments[0] + " is not ip, fp, characters, or time";
        if (!ArgumentIsOperation(parsedCommand.arguments[0]))
            return false;
        result = $"{name} requires the second argument to be ip, fp, characters, or time";
        if (parsedCommand.arguments.Count < 2)
            return false;
        result = parsedCommand.arguments[1] + " is not ip, fp, characters, or time";
        if (!ArgumentIsOperation(parsedCommand.arguments[1]))
            return false;

        result = $"Cannot convert from {parsedCommand.arguments[0]} to {parsedCommand.arguments[1]}";
        if (GetConversionType(parsedCommand.arguments[0], parsedCommand.arguments[1]) == Conversion.NONE)
            return false;

        result = $"{name} requires the third argument to be an integer number";
        if (parsedCommand.arguments.Count < 3)
            return false;
        result = parsedCommand.arguments[2] + " is not an integer number";
        if (!int.TryParse(parsedCommand.arguments[2], out int amount))
            return false;
        return true;
    }

    private bool ArgumentIsOperation(string operation) {
        if (operation == "ip")
            return true;
        if (operation == "fp")
            return true;
        if (operation == "characters")
            return true;
        if (operation == "time")
            return true;
        return false;
    }

    private Conversion GetConversionType(string from, string to)
    {
        if (from == "ip" && to == "fp")
            return Conversion.IP_TO_FP;
        if (from == "ip" && to == "characters")
            return Conversion.IP_TO_CHARACTERS;
        if (from == "ip" && to == "time")
            return Conversion.IP_TO_TIME;

        if (from == "fp" && to == "ip")
            return Conversion.FP_TO_IP;
        if (from == "fp" && to == "characters")
            return Conversion.FP_TO_CHARACTERS;
        if (from == "fp" && to == "time")
            return Conversion.FP_TO_TIME;

        if (from == "characters" && to == "ip")
            return Conversion.CHARACTERS_TO_IP;
        if (from == "characters" && to == "fp")
            return Conversion.CHARACTERS_TO_FP;
        if (from == "characters" && to == "time")
            return Conversion.CHARACTERS_TO_TIME;

        if (from == "time" && to == "ip")
            return Conversion.TIME_TO_IP;
        if (from == "time" && to == "fp")
            return Conversion.TIME_TO_FP;
        if (from == "time" && to == "characters")
            return Conversion.TIME_TO_CHARACTERS;
        return Conversion.NONE;
    }

    public int GetDamageBase() => 0;

    public int GetTotalDamage() => totalDamage;

    public string GetDamageSourceName() => "ConvertCommand";
}
