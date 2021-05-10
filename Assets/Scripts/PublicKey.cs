using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PublicKey : MonoBehaviour, IPickup
{
    public KeyEntity publicKeyEntityPrefab;
    public bool isAvailable = false;
    protected void Start() {
        Register();
    }
    protected virtual void Register()
    {

    }
    public virtual string GetName() { return name; }
    public virtual string GetUsageDescription() { return ""; }
    public virtual KeyEntity InstantiateEntityKey(Transform parent)
    {
        KeyEntity publicKeyEntity = Instantiate(publicKeyEntityPrefab, parent);
        return publicKeyEntity;
    }

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

    public bool IsActive()
    {
        return true;
    }

    public float GetLootValue()
    {
        return 0;
    }
}
