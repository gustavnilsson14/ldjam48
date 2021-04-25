using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PublicKey : MonoBehaviour
{
    public KeyEntity publicKeyEntityPrefab;
    public bool isAvailable = false;
    protected virtual void Awake() {
        GetComponentInParent<Host>().RegisterKey(this);
    }
    public virtual string GetName() { return name; }
    public virtual string GetUsageDescription() { return ""; }

    public virtual KeyEntity InstantiateEntityKey(Transform parent)
    {
        KeyEntity publicKeyEntity = Instantiate(publicKeyEntityPrefab, parent);
        return publicKeyEntity;
    }
}
