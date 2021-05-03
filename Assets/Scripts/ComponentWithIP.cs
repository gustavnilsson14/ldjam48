using System;
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

    [Header("Events")]
    public TakeDamageEvent onTakeDamage = new TakeDamageEvent();
    public HealEvent onHeal = new HealEvent();
    public DeathEvent onDeath = new DeathEvent();
    protected virtual void Awake()
    {
        currentDirectory = GetComponentInParent<Directory>();
        currentIP = maxIP;
    }
    public virtual bool TakeDamage(int amount, string source = "", string overrideTextLine = "")
    {
        if (!alive)
            return false;
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

    public virtual string GetName()
    {
        return name;
    }

    public virtual void Die()
    {
        alive = false;
        onDeath.Invoke();
        GameObject.Destroy(gameObject, 1f);
    }
    public int GetCurrentIP()
    {
        return currentIP;
    }
    public virtual string GetDescription()
    {
        return description;
    }
}

public class HealEvent : UnityEvent<int> { }
public class TakeDamageEvent : UnityEvent<int> { }
public class DeathEvent : UnityEvent { }
