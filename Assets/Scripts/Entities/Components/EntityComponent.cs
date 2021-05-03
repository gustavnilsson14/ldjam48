using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityComponent : Actor
{
    protected Entity entityBody;

    protected override void Start()
    {
        base.Start();
        entityBody = GetComponent<Entity>();
    }
    protected virtual bool GetSensorComponent(out SensorComponent sensorComponent) {
        sensorComponent = GetComponent<SensorComponent>();
        if (sensorComponent == null)
            return false;
        return true;
    }
    public virtual string GetComponentId() {
        return $"{Array.IndexOf(GetComponents<EntityComponent>(),this)}-{GetType().ToString().ToLower().Replace("component","")}";
    }

    public override string GetName()
    {
        return $"{GetComponentId()} on {name}";
    }
    public override string GetDescription()
    {
        return $"{GetComponentId()}: {base.GetDescription()}";
    }
    public override void Die()
    {
        alive = false;
        onDeath.Invoke();
        GameObject.Destroy(this);
    }
}
