using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupEntity : Entity
{
    public StoredObject pickup;
    public bool invulnerable = false;
    //public IPickup pickup;
    public void Init(IPickup pickup) {
        ReflectionUtil.GetStoredObject(out this.pickup, pickup, pickup.GetComponentId());
    }
    protected override void RegisterName()
    {
        base.RegisterName();
        name = $"{this.pickup.id["name"].Replace("/","-")}.{this.pickup.id["pickupType"]}";
    }
    public override bool TakeHit(IDamageSource source, out int armorDamageTaken, out int bodyDamageTaken)
    {
        if (!invulnerable)
            return base.TakeHit(source, out armorDamageTaken, out bodyDamageTaken);
        armorDamageTaken = 0;
        bodyDamageTaken = 0;
        return false;
    }
    public override string GetCatDescription()
    {
        if (pickup.objectType.IsSubclassOf(typeof(PublicKey)))
            return GetKeyCatDescription();
        List<string> result = new List<string> {
            GetBinaryStatic(),
            $"{pickup.id["name"]}-{pickup.id["pickupType"]} was added to sql"
        };
        onCat.Invoke();
        Player.I.GetComponent<SqlComponent>(out SqlComponent sqlComponent);
        sqlComponent.AddItem(this.pickup);
        DestroyMe();
        return string.Join("\n", result);
    }

    private string GetKeyCatDescription()
    {
        List<string> result = new List<string> {
            GetBinaryStatic(),
            $"The key to {pickup.id["name"]} was added to your keys!",
        };
        onCat.Invoke();
        if (pickup.objectType == typeof(SshKey))
        {
            SshKey key = HostHandler.I.currentHost.GetComponent<SshKey>();
            HostHandler.I.currentHost.GetComponent<SshKey>().isAvailable = true;
            HostHandler.I.currentHost.keys.Add(key);
            DestroyMe();
            return string.Join("\n", result);
        }
        HostHandler.I.currentHost.RegisterKey(pickup);
        DestroyMe();
        return string.Join("\n", result);
    }

    public void DestroyMe() {
        Destroy(gameObject);
    }

    public bool test = false;
    public EntityComponent debugEntityComponent;

    private void Update()
    {
        if (!test)
            return;
        test = false;
        Init(debugEntityComponent);
    }
}
