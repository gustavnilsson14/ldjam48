using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PublicKey : MonoBehaviour, IPickup
{
    public bool isAvailable = false;
    protected void Start() {
        StartRegister();
    }
    protected virtual void StartRegister()
    {
        InitPickup();
    }
    public void InitPickup()
    {
        PickupHandler.I.InitPickup(this);
    }
    public virtual string GetName() { return name; }

    public void OnBodyDeath()
    {
        return;
    }

    public Dictionary<string, string> GetComponentId()
    {
        return new Dictionary<string, string>(){
            {"name", GetName()},
            {"pickupType", "key"}
        };
    }

    public string GetComponentTypeName()
    {
        return GetType().ToString().ToLower();
    }

    public float GetLootValue()
    {
        return 0;
    }

    public virtual string GetShortDescription()
    {
        return "";
    }

    public PickupType GetPickupType() => PickupType.KEY;
}
