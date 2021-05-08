using System;
using System.Linq;
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

    private float pausedSeconds = 0;

    public CommandEvent onCommand = new CommandEvent();
    public UnityEvent onRealTime = new UnityEvent();
    public EntityComponentEvent onInstall = new EntityComponentEvent();
    public EntityComponentEvent onUnInstall = new EntityComponentEvent();

    private List<string> takeDamageMessage = new List<string>();

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
    }
    public override void StartRegister()
    {
        base.StartRegister();
        IOTerminal.I.onCommand.AddListener(OnCommand);
        commands.AddRange(GetComponentsInChildren<Command>());
        FullRestore();
    }
    private void Update()
    {
        ReduceRealTime();
        if (!test)
            return;
        test = false;
        AttackComponent source = gameObject.AddComponent<AttackComponent>();
        source.StartRegister();
        source.damageBase = 4;
        TakeHit(source, out int armorDamageTaken, out int bodyDamageTaken);
        Destroy(source);
    }
    protected void OnCommand(Command command, ParsedCommand parsedCommand)
    {
        ModifyCharacters(parsedCommand.GetCommandString());
        onCommand.Invoke(command, parsedCommand);
    }

    public List<ICommandDisabler> GetActiveICommandDisablers()
    {
        List<ICommandDisabler> result = new List<ICommandDisabler>();
        foreach (ICommandDisabler mod in currentDirectory.GetModifiers().FindAll(modifier => modifier is CommandDisablerModifier))
        {
            result.Add(mod);
        }
        foreach (ICommandDisabler mod in GetComponents<CommandDisablerCondition>())
        {
            result.Add(mod);
        }
        return result;
    }
    private void ReduceRealTime()
    {
        float timePassed = Time.deltaTime * GetRealTimeMultiplier();
        if (pausedSeconds > 0)
        {
            pausedSeconds -= Time.deltaTime;
            return;
        }
        currentSeconds -= Mathf.Clamp(timePassed, 0, Mathf.Infinity);
        onRealTime.Invoke();
        if (currentSeconds > 0)
            return;
        overTime += Mathf.Clamp(timePassed, 0, Mathf.Infinity);
        if (overTime < 10)
            return;
        overTime = 0;
        int damage = HostHandler.I.currentHost.GetTotalDamage();
        takeDamageMessage.Add($"Your time on this server is up, the system {StringUtil.ColorWrap($"damages your IP by {damage}", Palette.RED)}");
        SurvivedHit(HostHandler.I.currentHost);
        CommitTakeDamageMessage();
    }
    public void ModifyCharacters(string command)
    {
        currentCharacters -= Mathf.FloorToInt((float)command.Length * GetCharacterCostMultiplier());
        if (currentCharacters > 0)
            return;
        int damage = HostHandler.I.currentHost.GetTotalDamage();
        takeDamageMessage.Add($"Your input characters are depleted on this server, the system {StringUtil.ColorWrap($"damages your IP by {damage}", Palette.RED)}");
        SurvivedHit(HostHandler.I.currentHost);
        CommitTakeDamageMessage();
    }

    public void LevelUp()
    {
        maxIP+=2;
        maxCharacters += 50;
        maxSeconds += 120;
    }
    public override bool TakeHit(IDamageSource source, out int armorDamageTaken, out int bodyDamageTaken)
    {
        takeDamageMessage.Add($"{source.GetDamageSourceName()} <color=red>hits you!</color>");
        bool result = base.TakeHit(source, out armorDamageTaken, out bodyDamageTaken);
        CommitTakeDamageMessage();
        return result;
    }
    protected override bool ArmorAbsorbedHit(IDamageSource source, ref int remainingDamage, out int damageTaken)
    {
        bool result = base.ArmorAbsorbedHit(source, ref remainingDamage, out damageTaken);
        if (!GetComponent<ArmorComponent>(out ArmorComponent armor))
            return result;
        if (!armor.IsProtecting(this, source))
            return result;
        string color = result ? "#088" : "#f0f";
        takeDamageMessage.Add($"Your component {armor.GetCurrentIdentifier()} absorbed <color={color}>{damageTaken} IP damage{(!result ? ", and was deleted" : "")}</color>");
        if (result)
            Camera.main.GetComponent<CameraShake>().Shake(CameraShakeType.HIT);
        return result;
    }
    public override bool SurvivedHit(IDamageSource source, ref int remainingDamage, out int damageTaken)
    {
        bool result = base.SurvivedHit(source, ref remainingDamage, out damageTaken);
        Camera.main.GetComponent<CameraShake>().Shake();
        takeDamageMessage.Add($"Your integrity was interrupted by <color=red>{damageTaken} IP damage</color>");
        return result;
    }

    private void CommitTakeDamageMessage()
    {
        IOTerminal.I.AppendTextLine(string.Join("\n", takeDamageMessage));
        takeDamageMessage.Clear();
    }
    public float GetCharacterCostMultiplier()
    {
        float multiplier = 1;
        if (currentDirectory == null)
            return multiplier;
        List<DirectoryModifier> charactersMultipliers = currentDirectory.GetModifiers().FindAll(modifier => modifier is CharactersMultiplier);
        foreach (DirectoryModifier modifier in charactersMultipliers)
        {
            multiplier += (modifier as CharactersMultiplier).multiplier;
        }
        return Mathf.Clamp(multiplier, 0, Mathf.Infinity);
    }
    public float GetRealTimeMultiplier()
    {
        float multiplier = 1;
        if (currentDirectory == null)
            return multiplier;
        List<DirectoryModifier> charactersMultipliers = currentDirectory.GetModifiers().FindAll(modifier => modifier is TimeMultiplier);
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
    protected override void OnEntityEnterMyDirectory(Directory from, Directory current, Entity entity)
    {
        base.OnEntityEnterMyDirectory(from, current, entity);
        if (entity == this)
            return;
        IOTerminal.I.AppendTextLine($"Something just entered this directory from {from.name}");
    }
    protected override void OnEntityExitMyDirectory(Directory current, Directory to, Entity entity)
    {
        base.OnEntityExitMyDirectory(current, to, entity);
        if (entity == this)
            return;
        string entityName = (!entity.isDiscovered) ? "Something" : entity.name;
        IOTerminal.I.AppendTextLine($"{entityName} just left this directory into {to.name}");
    }
    public List<EntityComponent> GetInstalledComponents()
    {
        return new List<EntityComponent>(GetComponents<EntityComponent>());
    }
    public override string GetCatDescription()
    {
        return "This is you\n" + base.GetCatDescription();
    }
    public override void MoveTo(Directory directory)
    {
        if (currentDirectory == null)
        {
            base.MoveTo(directory);
            return;
        }
        List<DirectoryModifier> previousModifiers = currentDirectory.GetModifiers();
        base.MoveTo(directory);
        DisplayPermissionsFeeling(previousModifiers, currentDirectory.GetModifiers());
    }
    public bool InstallComponent(out EntityComponent installedComponent, StoredObject storedObject)
    {
        installedComponent = Player.I.gameObject.AddComponent(storedObject.objectType) as EntityComponent;
        if (installedComponent == null)
            return false;
        ReflectionUtil.ApplyStoredObject(storedObject, installedComponent);
        installedComponent.SetActiveState(true);
        onInstall.Invoke(installedComponent);
        return true;
    }
    public void UnInstallComponent(EntityComponent entityComponent)
    {
        Destroy(entityComponent);
        onUnInstall.Invoke(entityComponent);
    }
    public void PauseRealTime(int time)
    {
        pausedSeconds = time;
    }
    private void DisplayPermissionsFeeling(List<DirectoryModifier> previousModifiers, List<DirectoryModifier> currentModifiers)
    {
        if (previousModifiers.Count == currentModifiers.Count)
            return;
        string feelings = "The chmod feels more <color=#f0f>rigid</color> in this directory";
        if (previousModifiers.Count > currentModifiers.Count)
            feelings = "The chmod feels more <color=#088>flexible</color> in this directory";
        IOTerminal.I.AppendTextLine(feelings);
    }
}
public class MoveEvent : UnityEvent<Directory, Directory> { }
public class EntityComponentEvent : UnityEvent<EntityComponent> { }