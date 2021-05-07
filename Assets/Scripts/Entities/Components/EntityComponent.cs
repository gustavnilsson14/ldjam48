using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityComponent : Actor, IPickup
{
    public float lootValue = 5;
    protected Entity entityBody;
    private bool isActive = true;
    public override void StartRegister()
    {
        base.StartRegister();
        entityBody = GetComponent<Entity>();
    }
    protected virtual bool GetCurrentSensorTarget(out SensorComponent.TargetData target)
    {
        target = null;
        if (!TryGetComponent<SensorComponent>(out SensorComponent sensorComponent)) {
            HandleNoSensor();
            return false;
        }
        if (!sensorComponent.GetCurrentTarget(out target))
            return false;
        return true;
    }
    protected virtual bool GetSensorComponent(out SensorComponent sensorComponent) {
        sensorComponent = GetComponent<SensorComponent>();
        if (sensorComponent == null) { }
            return false;
        return true;
    }
    protected virtual void HandleNoSensor() { }
    public virtual Dictionary<string, string> GetComponentId()
    {
        return new Dictionary<string, string>(){
            {"name", GetComponentTypeName()},
            {"pickupType", "component"}
        };
    }
    public virtual string GetComponentTypeName()
    {
        return GetType().ToString().ToLower().Replace("component", "");
    }
    public virtual string GetCurrentIdentifier()
    {
        return $"{Array.IndexOf(GetComponents<EntityComponent>(),this)}-{GetComponentTypeName()}";
    }
    public override string GetName()
    {
        return $"{GetCurrentIdentifier()} on {name}";
    }
    public override string GetDescription()
    {
        return $"{GetCurrentIdentifier()}: {base.GetDescription()}";
    }
    public override void Die()
    {
        alive = false;
        onDeath.Invoke();
        GameObject.Destroy(this);
    }
    public virtual void OnBodyDeath()
    {
        PickupHandler.I.CreatePickup(GetBody().currentDirectory.transform, this);
    }

    public void SetActiveState(bool state)
    {
        isActive = state;
    }

    public bool IsActive()
    {
        return isActive;
    }

    public Entity GetBody() {
        return entityBody;
    }

    public float GetLootValue()
    {
        return lootValue;
    }
}
