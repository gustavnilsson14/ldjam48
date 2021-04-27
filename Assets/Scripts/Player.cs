using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Player : Entity
{
    public static Player I;

    private List<Command> commands = new List<Command>();

    public int currentCharacters = 0;
    public int maxCharacters = 10000;
    public float currentSeconds = 0;
    private float maxSeconds = 60 * 10;

    public CommandEvent onCommand = new CommandEvent();
    public UnityEvent onRealTime = new UnityEvent();

    public bool test = false;
    private float overTime;

    public List<Command> GetCommands()
    {
        return commands;
    }
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        Player.I = this;
        commands.AddRange(GetComponentsInChildren<Command>());
        FullRestore();
    }
    private void Start()
    {
        IOTerminal.I.onCommand.AddListener(OnCommand);
    }
    private void Update()
    {
        ReduceRealTime();
        if (!test)
            return;
        test = false;
        TakeDamage(1, "");
    }
    protected void OnCommand(Command command, ParsedCommand parsedCommand)
    {
        ModifyCharacters(parsedCommand.GetCommandString());
        onCommand.Invoke(command, parsedCommand);
    }
    private void ReduceRealTime()
    {
        float timePassed = Time.deltaTime * GetRealTimeMultiplier();
        currentSeconds -= Mathf.Clamp(timePassed, 0, Mathf.Infinity);
        onRealTime.Invoke();
        if (currentSeconds > 0)
            return;
        overTime += Mathf.Clamp(timePassed, 0, Mathf.Infinity);
        if (overTime < 10)
            return;
        overTime = 0;
        TakeDamage(1, "", "Your time on this server is up, the system damages your IP by 1");
    }
    public void ClearModifiers()
    {
        activeModifiers.Clear();
    }

    public void LevelUp()
    {
        maxIP+=2;
        maxCharacters += 50;
        maxSeconds += 120;
    }
    public void ModifyCharacters(string command)
    {
        currentCharacters -= Mathf.FloorToInt((float)command.Length * GetCharacterCostMultiplier());
        if (currentCharacters > 0)
            return;
        TakeDamage(1, "", "Your input characters are depleted on this server, the system damages your IP by 1");
    }
    public override bool TakeDamage(int amount, string source = "", string overrideTextLine = "")
    {
        if (alive)
            PrintDamage(amount, source, overrideTextLine);
        Camera.main.GetComponent<CameraShake>().Shake();
        return base.TakeDamage(amount);
    }
    protected void PrintDamage(int amount, string source, string overrideTextLine)
    {
        source = (source == "") ? "Something" : source;
        IOTerminal.I.AppendTextLine((overrideTextLine == "") ? $"{source} interrupts your IP for {amount} damage" : overrideTextLine);
    }
    public float GetCharacterCostMultiplier()
    {
        float multiplier = 1;
        List<DirectoryModifier> charactersMultipliers = activeModifiers.FindAll(modifier => modifier is CharactersMultiplier);
        foreach (DirectoryModifier modifier in charactersMultipliers)
        {
            multiplier += (modifier as CharactersMultiplier).multiplier;
        }
        return Mathf.Clamp(multiplier, 0, Mathf.Infinity);
    }
    public float GetRealTimeMultiplier()
    {
        float multiplier = 1;
        List<DirectoryModifier> charactersMultipliers = activeModifiers.FindAll(modifier => modifier is TimeMultiplier);
        foreach (DirectoryModifier modifier in charactersMultipliers)
        {
            multiplier += (modifier as TimeMultiplier).multiplier;
        }
        return Mathf.Clamp(multiplier, 0, Mathf.Infinity);
    }
    public void FullRestore()
    {
        directoryHistory.Clear();
        currentIP = maxIP;
        currentCharacters = maxCharacters;
        currentSeconds = maxSeconds;
    }

    public static bool GetCommand(out Command command, string commandName, bool onlyAvailable = true)
    {
        command = Player.I.commands.Find(c => c.name == commandName && (!onlyAvailable || c.isAvailable));
        if (command == null)
            return false;
        return true;
    }
    public static List<Command> GetCommands(bool onlyAvailable = true)
    {
        List<Command> commands = new List<Command>();
        commands.AddRange(Player.I.commands.FindAll(c => (!onlyAvailable || c.isAvailable)));
        return commands;
    }
    public string GetTimeLeft()
    {
        TimeSpan t = TimeSpan.FromSeconds(Mathf.FloorToInt(currentSeconds));
        return string.Format("{0:D2}:{1:D2}:{2:D2}", t.Hours,t.Minutes,t.Seconds);
    }
    public override bool IsAllowedInDirectory(Directory directory)
    {
        if (base.IsAllowedInDirectory(directory))
            return true;
        if (HostHandler.I.currentHost.keys.Find(key => key.isAvailable && key.GetName() == directory.GetFullPath()))
            return true;
        return false;
    }
    public bool IsSafeInDirectory(Directory directory)
    {
        return (HostHandler.I.currentHost.keys.Find(key => key.isAvailable && key.GetName() == directory.GetFullPath()) != null);
    }
    public override string GetCatDescription()
    {
        return "This is you\n" + base.GetCatDescription();
    }
}
public class MoveEvent : UnityEvent<Directory, Directory> { }