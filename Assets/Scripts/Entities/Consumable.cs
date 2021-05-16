using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consumable : MonoBehaviour, IPickup, IGeneratedHostInhabitant, IWorldPositionObject
{
    public string description;

    [Header("Pickup")]
    [Range(1, 100)]
    public float lootValue = 1;

    [Header("GeneratedHostInhabitant")]
    public bool generatesInLeafDirectory = false;
    public bool generatesInBranchDirectory = false;
    public bool generatesInPriorityDirectory = true;
    [Range(1, 100)]
    public float rarity = 1;

    [Header("Consumable")]
    private Entity entityBody;
    private GameObject prefab;

    public GameObject instance { get; set; }

    private void Start()
    {
        entityBody = GetComponent<Entity>();
        InitPickup();
    }
    public void InitPickup()
    {
        PickupHandler.I.InitPickup(this);
    }
    public virtual void Consume( out string result) {
        Destroy(this);
        result = "Syntax error, consumable not implemented, wierd usage";
    }
    public virtual Dictionary<string, string> GetComponentId()
    {
        return null; 
    }
    public virtual string GetComponentTypeName()
    {
        return $"{GetType().ToString().ToLower()}";
    }
    public Entity GetBody()
    {
        return entityBody;
    }
    public void OnBodyDeath()
    {
        PickupHandler.I.CreatePickup(GetBody().currentDirectory.transform, this);
    }

    public float GetLootValue() => lootValue;
    public bool GeneratesInLeafDirectory() => generatesInLeafDirectory;
    public bool GeneratesInBranchDirectory() => generatesInBranchDirectory;
    public bool GeneratesInPriorityDirectory() => generatesInPriorityDirectory;
    public float GetRarity() => rarity;
    public bool GetAnimator(out Animator animator)
    {
        animator = instance.GetComponentInChildren<Animator>();
        return animator != null;
    }
    public bool GetRenderer(out Renderer renderer)
    {
        renderer = instance.GetComponentInChildren<Renderer>();
        return renderer != null;
    }
    public GameObject GetWorldObjectPrefab() => prefab;
    public WorldPositionType GetWorldPositionType() => WorldPositionType.PICKUP;

    public string GetShortDescription()
    {
        return description;
    }

    public PickupType GetPickupType() => PickupType.CONSUMABLE;

}
public class TopUp : Consumable {

    public override Dictionary<string, string> GetComponentId()
    {
        return new Dictionary<string, string>(){
            {"name", GetComponentTypeName()},
            {"pickupType", "topup"}
        };
    }
    public override string GetComponentTypeName()
    {
        return $"{GetType().ToString().ToLower().Replace("topup","")}";
    }
}
public class Expendable : Consumable {
    public override Dictionary<string, string> GetComponentId()
    {
        return new Dictionary<string, string>(){
            {"name", GetComponentTypeName()},
            {"pickupType", "expendable"}
        };
    }
    public override string GetComponentTypeName()
    {
        return $"{GetType().ToString().ToLower().Replace("expendable", "")}";
    }
}