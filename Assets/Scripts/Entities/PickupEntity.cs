using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupEntity : MonoBehaviour, IDiscoverable, IWorldPositionObject, IAutoCompleteObject, ILootable
{
    public GameObject prefab;
    public string description;
    public bool initWithComponent = false;

    private void Start()
    {
        InitDiscoverable();
        RegisterEventListeners();
        if (initWithComponent)
            InitWithComponent();
    }
    public void InitDiscoverable()
    {
        DiscoveryHandler.I.InitDiscoverable(this);
    }
    protected virtual void InitWithComponent()
    {
        Init(GetComponent<IPickup>());
    }
    public void Init(IPickup pickup)
    {
        name = $"{pickup.GetComponentId()["name"]}.{pickup.GetComponentId()["pickupType"]}";
        pickupDescription = pickup.GetShortDescription();
        pickupType = pickup.GetPickupType();
        ReflectionUtil.GetStoredObject(out StoredObject lootObject, pickup, pickup.GetComponentId());
        loot = lootObject;
    }
    protected virtual void RegisterEventListeners()
    {
        onDiscover.AddListener(OnDiscover);
        onForget.AddListener(OnForget);
    }
    public virtual void OnDiscover(IDiscoverable arg0, bool arg1)
    {
        prefab = PickupHandler.I.GetPickupPrefab(this);
        WorldPositionHandler.I.CreateWorldPositionObject(this, out GameObject worldObjectInstance);
        instance = worldObjectInstance;
    }
    public void OnForget(IDiscoverable arg0, bool arg1)
    {
        WorldPositionHandler.I.PlayAnimation(this, "Out");
        Destroy(instance, 1f);
    }
    public virtual List<string> FormatCatDescription(List<string> catDescription)
    {
        Destroy(gameObject, 1f);
        WorldPositionHandler.I.PlayAnimation(this, "Out");
        PickupHandler.I.Pickup(this);
        catDescription.Add(pickupDescription);
        return catDescription;
    }
    public string GetFileName() => name;

    public GameObject GetGameObject() => gameObject;

    public string GetName() => name;

    public string GetShortDescription() => description;

    public GameObject GetWorldObjectPrefab() => prefab;

    public WorldPositionType GetWorldPositionType() => WorldPositionType.PICKUP;

    public void InitLootable()
    {
        PickupHandler.I.InitLootable(this);
    }

    public DiscoveryEvent onDiscover { get; set; }
    public DiscoveryEvent onForget { get; set; }
    public CatEvent onCat { get; set; }
    public bool discovered { get; set; }
    public GameObject instance { get; set; }
    public Directory currentDirectory { get; set; }
    public StoredObject loot { get; set; }
    public string pickupDescription { get; set; }
    public PickupType pickupType { get; set; }
}