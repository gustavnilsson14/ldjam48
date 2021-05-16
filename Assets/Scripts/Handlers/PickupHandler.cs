using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum PickupType
{ 
    CONSUMABLE, COMMAND, KEY, COMPONENT
}

public class PickupHandler : Handler
{
    public static PickupHandler I;
    public PickupEntity pickupEntityPrefab;
    public List<Type> allPickupTypes;

    [Header("Prefabs")]
    public GameObject consumablePrefab;
    public GameObject keyPrefab;
    public GameObject componentPrefab;
    public GameObject commandPrefab;
    protected override void StartRegister()
    {
        base.StartRegister();
        allPickupTypes = ReflectionUtil.GetAllImplementationsOfInterface<IPickup>();
    }
    public void InitLootDropper(ILootDropper lootDropper)
    {
        lootDropper.onLootDrop = new LootDropEvent();
    }
    public void InitPickup(IPickup pickup)
    {
        
    }
    public void InitLootable(ILootable lootable) {

    }
    public void DropLoot(ILootDropper lootDropper)
    {
        lootDropper.onLootDrop.Invoke(lootDropper);
        CreatePickups(lootDropper);
    }

    public void CreatePickups(ILootDropper lootDropper)
    {
        List<IPickup> possiblePickups = lootDropper.GetPickups();
        float currentLootValueTotal = lootDropper.GetChallengeRating() * HostHandler.I.currentHost.lootValueMultiplier;
        bool canAfford = true;
        while (canAfford)
        {
            canAfford = false;
            if (possiblePickups.Count == 0)
                continue;
            IPickup pickup = possiblePickups[UnityEngine.Random.Range(0, possiblePickups.Count)];
            if (pickup.GetLootValue() > currentLootValueTotal)
                continue;
            canAfford = true;
            currentLootValueTotal -= pickup.GetLootValue();
            possiblePickups.Remove(pickup);
            CreatePickup(lootDropper.GetTransform().parent, pickup);
        }
    }

    public GameObject GetPickupPrefab(PickupEntity pickupEntity)
    {
        switch (pickupEntity.pickupType)
        {
            case PickupType.CONSUMABLE:
                return consumablePrefab;
            case PickupType.COMMAND:
                return commandPrefab;
            case PickupType.KEY:
                return keyPrefab;
            case PickupType.COMPONENT:
                return componentPrefab;
        }
        return null;
    }

    public PickupEntity CreatePickup(Transform parent, IPickup pickup, bool invulnerable = false)
    {
        PickupEntity result = Instantiate(pickupEntityPrefab, parent) as PickupEntity;
        result.Init(pickup);
        return result;
    }
    public List<IPickup> GetAllPickups() {
        return new List<IPickup>(HostHandler.I.currentHost.GetComponentsInChildren<IPickup>());
    }
    public bool Pickup(ILootable pickupEntity)
    {
        if (pickupEntity.pickupType == PickupType.KEY)
            return AddKey(pickupEntity);
        return AddToSql(pickupEntity);
    }

    private bool AddKey(ILootable pickupEntity)
    {
        if (pickupEntity.loot.objectType == typeof(DirectoryKey))
        {
            HostHandler.I.currentHost.RegisterKey(pickupEntity.loot);
            return true;
        }
        HostHandler.I.currentHost.GetComponent<SshKey>().isAvailable = true;
        return true;
    }

    private bool AddToSql(ILootable pickupEntity) {
        if (!Player.I.TryGetComponent<SqlComponent>(out SqlComponent sqlComponent))
            return false;
        sqlComponent.AddItem(pickupEntity.loot);
        return true;
    }

    /*
     
     
        if (pickup.objectType.IsSubclassOf(typeof(PublicKey)))
            return GetKeyCatDescription();
        List<string> result = new List<string> {
            GetBinaryStatic(),
            $"{pickup.id["name"]}-{pickup.id["pickupType"]} was added to sql"
        };
        onCat.Invoke();
        DestroyMe();
        return string.Join("\n", result);
     
     */
}
public interface IPickup
{
    void InitPickup();
    void OnBodyDeath();
    Dictionary<string, string> GetComponentId();
    string GetComponentTypeName();
    float GetLootValue();
    string GetShortDescription();
    PickupType GetPickupType();
}
public interface ILootDropper
{
    void InitLootDropper();
    float GetChallengeRating();
    List<IPickup> GetPickups();
    Transform GetTransform();
    LootDropEvent onLootDrop { get; set; }
}
public interface ILootable {
    void InitLootable();
    StoredObject loot { get; set; }
    string pickupDescription { get; set; }
    PickupType pickupType { get; set; }
}
public class LootDropEvent : UnityEvent<ILootDropper> { }