using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EntityComponent : Actor, IPickup, IComponentIO, IGeneratedHostInhabitant
{
    [Header("Pickup")]
    [Range(1, 100)]
    public float lootValue = 5;

    [Header("GeneratedHostInhabitant")]
    public bool generatesInLeafDirectory = false;
    public bool generatesInBranchDirectory = false;
    public bool generatesInPriorityDirectory = true;
    [Range(1, 100)]
    public float rarity = 1;

    [Header("EntityComponent")]
    public ParsedPath parsedInput;
    protected Entity entityBody;

    public IOEvent onOutput = new IOEvent();

    public int hackedTime { get; set; }

    public override void StartRegister()
    {
        base.StartRegister();
        entityBody = GetComponent<Entity>();
    }
    public virtual void OnInput(IComponentIO source, string input) {
        if (hackedTime > 0)
            return;
        entityBody = GetComponent<Entity>();
        if (entityBody == null)
        {
            Debug.Log($"OnInput: {source} {input} {entityBody}");
            return;
        }
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
    public virtual void OnBodyDeath()
    {
        PickupHandler.I.CreatePickup(GetBody().currentDirectory.transform, this);
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
    public bool GeneratesInLeafDirectory()
    {
        return generatesInLeafDirectory;
    }

    public bool GeneratesInBranchDirectory()
    {
        return generatesInBranchDirectory;
    }

    public bool GeneratesInPriorityDirectory()
    {
        return generatesInPriorityDirectory;
    }

    public float GetRarity()
    {
        return rarity;
    }

    public void InitPickup()
    {
        PickupHandler.I.InitPickup(this);
    }

    public string GetShortDescription()
    {
        return "";
    }

    public PickupType GetPickupType() => PickupType.COMPONENT;
}
public class IOEvent : UnityEvent<IComponentIO, string> { }
public interface IComponentIO {
    IOEvent GetOnOutputEvent();
    void OnInput(IComponentIO source, string input);
    int hackedTime { get; set; }
}