using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EntityComponent : Actor, IPickup, IComponentIO
{
    [Header("Pickup")]
    [Range(1, 100)]
    public float lootValue = 5;

    [Header("EntityComponent")]
    public ParsedPath parsedInput;
    protected Entity entityBody;
    private bool isActive = true;

    public IOEvent onOutput = new IOEvent();

    public override void StartRegister()
    {
        base.StartRegister();
        entityBody = GetComponent<Entity>();
    }
    public virtual void OnInput(IComponentIO source, string input) {
        parsedInput = new ParsedPath(entityBody.currentDirectory, input);
        if (!HandleInputError(source, parsedInput.error))
            return;
        parsedInput = null;
    }
    public virtual bool HandleInputError(object source, string error)
    {
        onOutput.Invoke(this, GetInputReply(source, error));
        if (error == string.Empty)
            return false;
        return true;
    }
    protected virtual string GetInputReply(object source, string error) {
        if (error == string.Empty)
            return $"Input to {this.GetCurrentIdentifier()} accepted";
        return $"Error in input from {source.GetType().ToString().ToLower()}: {error}";
    }
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

    public IOEvent GetOnOutputEvent()
    {
        return onOutput;
    }
}
public class IOEvent : UnityEvent<IComponentIO, string> { }
public interface IComponentIO {
    IOEvent GetOnOutputEvent();
    void OnInput(IComponentIO source, string input);
}