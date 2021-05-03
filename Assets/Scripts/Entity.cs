using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum EntityFaction
{
    HACKER,
    SECURITY,
    VIRUS
}

public class Entity : ComponentWithIP
{
    public EntityFaction faction;
    public List<Directory> directoryHistory = new List<Directory>();

    [Header("Events")]
    public DirectoryMoveEvent onEntityEnterMyDirectory = new DirectoryMoveEvent();
    public DirectoryMoveEvent onEntityExitMyDirectory = new DirectoryMoveEvent();
    public MoveEvent onMove = new MoveEvent();
    public CatEvent onCat = new CatEvent();
    public DiscoverEvent onDiscover = new DiscoverEvent();
    public AttackEvent onAttack = new AttackEvent();
    public PlayerEscapeEvent onPlayerEscape = new PlayerEscapeEvent();

    public bool isDiscovered = false;

    protected override void Awake()
    {
        base.Awake();
        currentDirectory = GetComponentInParent<Directory>();
    }
    public virtual void MoveTo(Directory directory)
    {
        Directory previousDirectory = currentDirectory;
        currentDirectory = directory;
        directoryHistory.Insert(0, previousDirectory);
        transform.parent = currentDirectory.transform;
        onMove.Invoke(currentDirectory, previousDirectory);
        if (previousDirectory != null) {
            previousDirectory.EntityExit(currentDirectory, this);
            previousDirectory.onEntityEnter.RemoveListener(OnEntityEnterMyDirectory);
            previousDirectory.onEntityExit.RemoveListener(OnEntityExitMyDirectory);
        }
        currentDirectory.EntityEnter(previousDirectory, this);
        currentDirectory.onEntityEnter.AddListener(OnEntityEnterMyDirectory);
        currentDirectory.onEntityExit.AddListener(OnEntityExitMyDirectory);
    }

    protected virtual void OnEntityEnterMyDirectory(Directory from, Directory current, Entity entity)
    {
        onEntityEnterMyDirectory.Invoke(from, current, entity);
    }
    protected virtual void OnEntityExitMyDirectory(Directory current, Directory to, Entity entity)
    {
        onEntityExitMyDirectory.Invoke(current, to, entity);
    }

    public virtual string GetCatDescription()
    {
        List<string> result = new List<string> {
            GetBinaryStatic(),
            string.Format("IP: {0}", currentIP),
            description,
            GetEntityComponentsDescriptions()

        };
        onCat.Invoke();
        return string.Join("\n", result);
    }

    protected virtual string GetEntityComponentsDescriptions()
    {
        string result = "Components;";
        foreach (EntityComponent entityComponent in GetComponents<EntityComponent>())
        {
            result += $"\n    {entityComponent.GetDescription()}";
        }
        return result;
    }

    public virtual void Attack()
    {
        onAttack.Invoke();
    }

    public virtual void Discover()
    {
        if (isDiscovered)
            return;

        isDiscovered = true;
        onDiscover.Invoke();
        Player.I.onMove.AddListener(HandlePlayerMovement);
    }

    private void HandlePlayerMovement(Directory target, Directory origin)
    {
        isDiscovered = false;
        onPlayerEscape.Invoke();
        Player.I.onMove.RemoveListener(HandlePlayerMovement);
    }

    protected string GetBinaryStatic()
    {
        List<string> result = new List<string>();
        var rand = new System.Random();
        for (int i = 0; i < 10; i++)
        {
            result.Add(Convert.ToString(rand.Next(1024), 2).PadLeft(10, '0'));
        }
        return string.Join("\n", result);
    }
    public virtual bool IsAllowedInDirectory(Directory directory) {
        return (!directory.bannedFactions.Contains(faction));
    }
    public float GetDamageMultiplier()
    {
        float multiplier = 1;
        List<DirectoryModifier> charactersMultipliers = currentDirectory.GetModifiers().FindAll(modifier => modifier is DamageMultiplier);
        foreach (DirectoryModifier modifier in charactersMultipliers)
        {
            multiplier += (modifier as DamageMultiplier).multiplier;
        }
        return Mathf.Clamp(multiplier, 0, Mathf.Infinity);
    }
    public List<Condition> GetAllConditions() {
        return new List<Condition>(GetComponents<Condition>()).FindAll(condition => condition.IsActiveCondition());
    }
}
public class CatEvent : UnityEvent { }
public class DiscoverEvent : UnityEvent { }
public class PlayerEscapeEvent : UnityEvent { }
public class AttackEvent : UnityEvent { }
