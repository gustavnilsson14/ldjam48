using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ComponentWithIP : MonoBehaviour
{
    public int maxIP;
    protected int currentIP;

    [Header("Events")]
    public TakeDamageEvent onTakeDamage = new TakeDamageEvent();
    public DeathEvent onDeath = new DeathEvent();
    protected virtual void Awake()
    {
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

public class TakeDamageEvent : UnityEvent<int> { }
public class DeathEvent : UnityEvent { }