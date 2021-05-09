using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum EntityFaction
{
    NONE,
    HACKER,
    SECURITY,
    VIRUS
}

public class Entity : ComponentWithIP, ILootDropper, IAutoCompleteObject, IChallenge
{
    [Header("Challenge")]
    [Range(1, 100)]
    public float challengeRating;

    [Header("Entity")]
    public string uniqueId;
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
    public LootDropEvent onLootDrop = new LootDropEvent();


    public bool isDiscovered = false;
    
    public override void StartRegister()
    {
        base.StartRegister();
        uniqueId = EntityHandler.I.GetUniqueId(this);
        currentDirectory = GetComponentInParent<Directory>();
        RegisterComponentConnections();
        RegisterWithPickupHandler();
    }
    protected virtual void RegisterComponentConnections()
    {
        if (!TryGetComponent<SensorComponent>(out SensorComponent sensorComponent))
            return;
        if (TryGetComponent<AttackComponent>(out AttackComponent attackComponent)) {
            ConnectComponentIO(sensorComponent, attackComponent);
        }
        if (TryGetComponent<MovementComponent>(out MovementComponent movementComponent))
            ConnectComponentIO(sensorComponent, movementComponent);
    }
    public void ConnectComponentIO(EntityComponent output, EntityComponent input)
    {
        output.onOutput.AddListener(input.OnInput);
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
    public virtual int GetNewTargetWeight(Entity newTarget) {
        if (newTarget.faction != faction)
            return 5;
        return 0;
    }
    public virtual string GetCatDescription()
    {
        List<string> result = new List<string> {
            GetBinaryStatic(),
            $"IP: {currentIP}",
            $"UserGroup: {faction.ToString()}",
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
    public override void Die()
    {
        onLootDrop.Invoke(this);
        base.Die();
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
    public List<Condition> GetAllConditions() {
        return new List<Condition>(GetComponents<Condition>()).FindAll(condition => condition.IsActiveCondition());
    }

    public float GetChallengeRating()
    {
        return challengeRating;
    }

    public List<IPickup> GetPickups()
    {
        return new List<IPickup>(GetComponents<IPickup>());
    }

    public LootDropEvent GetLootDropEvent()
    {
        return onLootDrop;
    }

    public void RegisterWithPickupHandler()
    {
        PickupHandler.I.RegisterLootDropper(this);
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public bool die = false;
    private void Update()
    {
        if (!die)
            return;
        die = false;
        Die();
    }

    public IChallenge AddToDirectory(Directory directory)
    {
        if (!EntityHandler.I.InstantiateEntity(directory, gameObject, out Entity newEntity))
            return null;
        return newEntity as IChallenge;
    }

    public bool RequiresPrefab()
    {
        return true;
    }
}

public class CatEvent : UnityEvent { }
public class DiscoverEvent : UnityEvent { }
public class PlayerEscapeEvent : UnityEvent { }
public class AttackEvent : UnityEvent { }
