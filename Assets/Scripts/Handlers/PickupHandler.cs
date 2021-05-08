using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PickupHandler : MonoBehaviour
{
    public static PickupHandler I;
    public PickupEntity pickupEntityPrefab;
    private void Awake()
    {
        PickupHandler.I = this;
    }
    public void CreatePickups(ILootDropper lootDropper)
    {
        Debug.Log($"public void CreatePickups(ILootDropper lootDropper) {lootDropper.GetTransform().name}");
        List<IPickup> possiblePickups = lootDropper.GetPickups();
        float currentLootValueTotal = lootDropper.GetChallengeRating() * HostHandler.I.currentHost.lootValueMultiplier;
        Debug.Log(currentLootValueTotal + " " +lootDropper.GetChallengeRating() + " " +possiblePickups.Count);
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
    public PickupEntity CreatePickup(Transform parent, IPickup pickup) {
        PickupEntity result = Instantiate(pickupEntityPrefab, parent) as PickupEntity;
        Debug.Log("public PickupEntity CreatePickup(Transform parent, IPickup pickup) {" + pickup.GetType().ToString() + " " + parent.name);
        result.Init(pickup);
        return result;
    }
    public List<IPickup> GetAllPickups() {
        return new List<IPickup>(HostHandler.I.currentHost.GetComponentsInChildren<IPickup>()).FindAll(p => p.IsActive());
    }

    public void RegisterLootDropper(ILootDropper lootDropper)
    {
        lootDropper.GetLootDropEvent().AddListener(CreatePickups);
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
public interface ILootDropper
{
    float GetChallengeRating();
    List<IPickup> GetPickups();
    LootDropEvent GetLootDropEvent();
    void RegisterWithPickupHandler();
    Transform GetTransform();
}
public class LootDropEvent : UnityEvent<ILootDropper> { }