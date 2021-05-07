using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupEntity : Entity
{
    public StoredObject pickup;
    //public IPickup pickup;
    public void Init(IPickup pickup) {
        ReflectionUtil.GetStoredObject(out this.pickup, pickup, pickup.GetComponentId());
        gameObject.name = $"{this.pickup.id["name"]}-{this.pickup.id["pickupType"]}";
    }
    public override string GetCatDescription()
    {
        List<string> result = new List<string> {
            GetBinaryStatic(),
            $"{this.pickup.id["name"]}-{this.pickup.id["pickupType"]} was added to sql"
        };
        onCat.Invoke();
        Player.I.GetComponent<SqlComponent>(out SqlComponent sqlComponent);
        sqlComponent.AddItem(this.pickup);
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
