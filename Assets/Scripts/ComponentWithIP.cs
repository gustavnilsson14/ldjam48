﻿using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ComponentWithIP : MonoBehaviour
{
    public Directory currentDirectory;

    [TextArea(2, 10)]
    public string description;
    public int maxIP;
    protected int currentIP;
    public bool alive = true;
    public float challengeRating = 1;

    [Header("Events")]
    public TakeDamageEvent onTakeDamage = new TakeDamageEvent();
    public HealEvent onHeal = new HealEvent();
    public DeathEvent onDeath = new DeathEvent();
    protected virtual void Awake()
    {
        currentDirectory = GetComponentInParent<Directory>();
        currentIP = maxIP;
    }
    protected virtual void Start() {
        StartRegister();
    }
    /// <summary>
    /// Method for registering components etc.
    /// </summary>
    public virtual void StartRegister() { }
    public virtual bool TakeHit(IDamageSource source)
    { 
        return TakeHit(source, out int armorDamageTaken, out int bodyDamageTaken);
    }
    public virtual bool TakeHit(IDamageSource source, out int armorDamageTaken, out int bodyDamageTaken)
    {
        int remainingDamage = source.GetTotalDamage();
        armorDamageTaken = 0;
        bodyDamageTaken = 0;
        if (!alive)
            return false;
        if (ArmorAbsorbedHit(source, ref remainingDamage, out armorDamageTaken))
            return false;
        if (SurvivedHit(source, ref remainingDamage, out bodyDamageTaken))
            return false;
        return true;
    }
    protected virtual bool ArmorAbsorbedHit(IDamageSource source, ref int remainingDamage, out int damageTaken)
    {
        damageTaken = 0;
        if (!GetComponent<ArmorComponent>(out ArmorComponent armor))
            return false;
        if (!armor.IsProtecting(this, source))
            return false;
        return armor.SurvivedHit(source, ref remainingDamage, out damageTaken);
    }
    public virtual bool SurvivedHit(IDamageSource source, ref int remainingDamage, out int damageTaken)
    {
        damageTaken = Mathf.Clamp(remainingDamage, 0, currentIP);
        remainingDamage = Mathf.Clamp(remainingDamage - damageTaken, 0, int.MaxValue);
        currentIP -= damageTaken;
        if (currentIP <= 0)
        {
            Die();
            return false;
        }
        onTakeDamage.Invoke(damageTaken);
        return true;
    }

    public virtual bool Heal(int amount)
    {
        if (currentIP == maxIP)
        {
            return false;
        }
        currentIP = Mathf.Clamp(currentIP + amount,0,maxIP);
        onHeal.Invoke(amount);
        return true;
    }

    public virtual string GetName()
    {
        return name;
    }

    public virtual void Die()
    {
        alive = false;
        onDeath.Invoke();
        DropLoot();
        GameObject.Destroy(gameObject);
    }
    protected virtual void DropLoot()
    {
        List<IPickup> possiblePickups = new List<IPickup>(GetComponentsInChildren<IPickup>());
        float currentLootValueTotal = challengeRating * HostHandler.I.currentHost.lootValueMultiplier;
        bool canAfford = true;
        while (canAfford)
        {
            canAfford = false;
            if (possiblePickups.Count == 0)
                continue;
            IPickup pickup = possiblePickups[UnityEngine.Random.Range(0,possiblePickups.Count)];
            if (pickup.GetLootValue() > currentLootValueTotal)
                continue;
            canAfford = true;
            currentLootValueTotal -= pickup.GetLootValue();
            possiblePickups.Remove(pickup);
            PickupHandler.I.CreatePickup(currentDirectory.transform,pickup);
        }
    }
    public int GetCurrentIP()
    {
        return currentIP;
    }
    public virtual string GetDescription()
    {
        return description;
    }
    public virtual float GetDamageMultiplier()
    {
        float multiplier = 1;
        List<DirectoryModifier> charactersMultipliers = currentDirectory.GetModifiers().FindAll(modifier => modifier is DamageMultiplier);
        foreach (DirectoryModifier modifier in charactersMultipliers)
        {
            multiplier += (modifier as DamageMultiplier).multiplier;
        }
        return Mathf.Clamp(multiplier, 0, Mathf.Infinity);
    }

    public bool GetComponent<T>(out T result)
    {
        result = GetComponent<T>();
        if (result == null)
            return false;
        return true;
    }
}
public interface IDamageSource {
    int GetDamageBase();
    int GetTotalDamage();
    string GetDamageSourceName();
}

public class HealEvent : UnityEvent<int> { }
public class TakeDamageEvent : UnityEvent<int> { }
public class DeathEvent : UnityEvent { }
