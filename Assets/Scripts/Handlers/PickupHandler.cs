using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupHandler : MonoBehaviour
{
    public static PickupHandler I;
    public PickupEntity pickupEntityPrefab;
    private void Awake()
    {
        PickupHandler.I = this;
    }
    public PickupEntity CreatePickup(Transform parent, IPickup pickup) {
        PickupEntity result = Instantiate(pickupEntityPrefab, parent) as PickupEntity;
        result.Init(pickup);
        return result;
    }
    public List<IPickup> GetAllPickups() {
        return new List<IPickup>(HostHandler.I.currentHost.GetComponentsInChildren<IPickup>()).FindAll(p => p.IsActive());
    }
}
public interface IPickup
{
    void OnBodyDeath();
    Dictionary<string, string> GetComponentId();
    string GetComponentTypeName();
    void SetActiveState(bool state);
    bool IsActive();
    float GetLootValue();
}