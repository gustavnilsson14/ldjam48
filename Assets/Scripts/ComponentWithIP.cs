using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ComponentWithIP : MonoBehaviour
{
    public Directory currentDirectory;

    public int maxIP;
    protected int currentIP;

    [Header("Events")]
    public TakeDamageEvent onTakeDamage = new TakeDamageEvent();
    public HealEvent onHeal = new HealEvent();
    public DeathEvent onDeath = new DeathEvent();
    protected virtual void Awake()
    {
        currentDirectory = GetComponentInParent<Directory>();
        currentIP = maxIP;
    }
    public virtual bool TakeDamage(int amount)
    {
        currentIP -= amount;
        if (currentIP <= 0)
        {
            Die();
            return false;
        }
        onTakeDamage.Invoke(amount);
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

    public virtual void Die()
    {
        onDeath.Invoke();
        GameObject.Destroy(gameObject, 1f);
    }
    public int GetCurrentIP()
    {
        return currentIP;
    }
}

public class HealEvent : UnityEvent<int> { }
public class TakeDamageEvent : UnityEvent<int> { }
public class DeathEvent : UnityEvent { }