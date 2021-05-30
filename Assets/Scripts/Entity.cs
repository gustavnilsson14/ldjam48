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
public class Entity : MonoBehaviour, IDamageable, ILootDropper, IAutoCompleteObject, IChallenge, IGeneratedHostInhabitant, IDiscoverable, IWorldPositionObject
{
    [Header("Challenge")]
    [Range(1, 100)]
    public float challengeRating;

    [Header("GeneratedHostInhabitant")]
    public bool generatesInLeafDirectory = true;
    public bool generatesInBranchDirectory = true;
    public bool generatesInPriorityDirectory = true;
    [Range(1, 100)]
    public float rarity = 1;

    [Header("Discoverable")]
    public string description;

    [Header("WorldPositionObject")]
    public GameObject worldObjectPrefab;

    [Header("Damageable")]
    public int maxIP;

    [Header("Entity")]
    public string uniqueId;
    public EntityFaction faction;
    public List<Directory> directoryHistory = new List<Directory>();
    public bool staticName = false;

    [Header("Events")]
    public DirectoryMoveEvent onEntityEnterMyDirectory = new DirectoryMoveEvent();
    public DirectoryMoveEvent onEntityExitMyDirectory = new DirectoryMoveEvent();
    public MoveEvent onMove = new MoveEvent();
    public AttackEvent onAttack = new AttackEvent();

    private void Start()
    {
        StartRegister();
    }

    public virtual void StartRegister()
    {
        InitDamageable();
        InitLootDropper();
        InitDiscoverable();

        uniqueId = EntityHandler.I.GetUniqueId(this);
        currentDirectory = GetComponentInParent<Directory>();

        RegisterComponentConnections();
        RegisterName();
        RegisterEventListeners();
    }
    public virtual void InitDamageable() => DamageHandler.I.InitDamageable(this);
    public virtual void InitLootDropper() => PickupHandler.I.InitLootDropper(this);
    public virtual void InitDiscoverable() => DiscoveryHandler.I.InitDiscoverable(this);
    protected virtual void RegisterEventListeners()
    {
        onHitTaken.AddListener(OnHitTaken);
        onDeath.AddListener(OnDeath);
        onDiscover.AddListener(OnDiscover);
        onForget.AddListener(OnForget);
    }
    public virtual void OnDiscover(IDiscoverable arg0, bool arg1)
    {
        WorldPositionHandler.I.CreateWorldPositionObject(this, out GameObject worldObjectInstance);
        instance = worldObjectInstance;
    }
    public void OnForget(IDiscoverable arg0, bool arg1)
    {
        WorldPositionHandler.I.PlayAnimation(this, "Out");
        Destroy(instance, 1f);
    }

    protected void OnDeath(IDamageable target)
    {
        Destroy(instance, 2f);
        WorldPositionHandler.I.PlayAnimation(this, "Die");
        PickupHandler.I.DropLoot(this);
    }

    private void OnHitTaken(IDamageSource source, bool isDead, int armorDamage, int bodyDamage)
    {
        WorldPositionHandler.I.PlayAnimation(this, GetHitAnimationName(isDead, armorDamage, bodyDamage));
    }
    private string GetHitAnimationName(bool isDead, int armorDamage, int bodyDamage) {

        if (isDead)
            return "Death";
        if (bodyDamage > 0)
            return "BodyDamage";
        if (armorDamage > 0)
            return "ArmorDamage";
        return "";
    }

    protected virtual void RegisterName()
    {
        name = name.Replace("(Clone)", "").Trim();
        if (staticName)
            return;
        name = NameUtil.I.GetEntityName(name);
    }

    protected virtual void RegisterComponentConnections()
    {
        if (!TryGetComponent<SensorComponent>(out SensorComponent sensorComponent))
            return;
        if (TryGetComponent<AttackComponent>(out AttackComponent attackComponent)) {
            ConnectComponentIO(sensorComponent, attackComponent);
            attackComponent.onRun.AddListener(OnAttack);
        }
        if (TryGetComponent<MovementComponent>(out MovementComponent movementComponent)) {
            ConnectComponentIO(sensorComponent, movementComponent);
        }
    }
    public virtual void OnAttack(Actor actor)
    {
        if (!alive)
            return;
        if (!(actor is AttackComponent))
            return;
        WorldPositionHandler.I.PlayAnimation(this, "Attack");
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
    public virtual List<string> FormatCatDescription(List<string> catDescription)
    {
        catDescription.AddRange(
            new List<string> {
                $"IP: {currentIP}",
                $"UserGroup: {faction.ToString()}",
                GetEntityComponentsDescriptions()
            }
        );
        return catDescription;
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

    public virtual bool IsAllowedInDirectory(Directory directory) {
        return (!directory.bannedFactions.Contains(faction));
    }
    public List<Condition> GetAllConditions() {
        return new List<Condition>(GetComponents<Condition>()).FindAll(condition => condition.IsActiveCondition());
    }

    public float GetChallengeRating() => challengeRating;

    public List<IPickup> GetPickups()
    {
        return new List<IPickup>(GetComponents<IPickup>());
    }
    public IChallenge AddToDirectory(Directory directory)
    {
        if (!EntityHandler.I.InstantiateEntity(directory, gameObject, out Entity newEntity))
            return null;
        return newEntity as IChallenge;
    }

    public GameObject instance { get; set; }
    public string GetName() => name;
    public string GetShortDescription() => description;
    public string GetFileName() => name;
    public bool GeneratesInLeafDirectory() => generatesInLeafDirectory;
    public bool GeneratesInBranchDirectory() => generatesInBranchDirectory;
    public bool GeneratesInPriorityDirectory() => generatesInPriorityDirectory;
    public float GetRarity() => rarity;
    public WorldPositionType GetWorldPositionType() => WorldPositionType.ENTITY;
    public GameObject GetWorldObjectPrefab() => worldObjectPrefab;
    public Transform GetTransform() => transform;
    public int GetMaxIP() => maxIP;
    public GameObject GetGameObject() => gameObject;

    public bool IsBaseComponent() => true;

    public bool alive { get; set; }
    public int currentIP { get; set; }
    public ArmorDamageEvent onArmorDamage { get; set; }
    public BodyDamageEvent onBodyDamage { get; set; }
    public HealEvent onHeal { get; set; }
    public DeathEvent onDeath { get; set; }
    public LootDropEvent onLootDrop { get; set; }
    public TakeHitEvent onTakeHit { get; set; }
    public HitTakenEvent onHitTaken { get; set; }
    public DiscoveryEvent onDiscover { get; set; }
    public DiscoveryEvent onForget { get; set; }
    public bool discovered { get; set; }
    public DirectDamageEvent onDirectDamage { get; set; }
    public CatEvent onCat { get; set; }
    public Directory currentDirectory { get; set; }
}

public class AttackEvent : UnityEvent { }
