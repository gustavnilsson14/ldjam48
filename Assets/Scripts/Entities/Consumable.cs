using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consumable : MonoBehaviour, IPickup
{
    [Header("Pickup")]
    [Range(1, 100)]
    public float lootValue = 1;

    [Header("Consumable")]
    private Entity entityBody;
    private bool isActive = true;
    

    private void Start()
    {
        entityBody = GetComponent<Entity>();
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
    public bool IsActive()
    {
        return isActive;
    }
    public Entity GetBody()
    {
        return entityBody;
    }
    public void OnBodyDeath()
    {
        PickupHandler.I.CreatePickup(GetBody().currentDirectory.transform, this);
    }
    public void SetActiveState(bool state)
    {
        isActive = state;
    }

    public float GetLootValue()
    {
        return lootValue;
    }
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