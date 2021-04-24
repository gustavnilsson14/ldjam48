using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Entity : MonoBehaviour
{
    public Directory currentDirectory;
    public int maxIP;
    protected int currentIP;

    [Header("EVents")]
    public TakeDamageEvent onTakeDamage = new TakeDamageEvent();
    public DeathEvent onDeath = new DeathEvent();

    private void Awake()
    {
        currentDirectory = GetComponentInParent<Directory>();
        currentIP = maxIP;
    }
    private void Start()
    {
        IOTerminal.I.onCommand.AddListener(OnCommand);
        IOTerminal.I.onTerminalTimePast.AddListener(OnTerminalTimePast);
    }
    protected virtual void OnCommand(Command command, ParsedCommand parsedCommand)
    {
        Debug.Log("OnCommand " + command.name + ")");
    }
    protected virtual void OnTerminalTimePast(int terminalTimePast)
    {
        Debug.Log("OnTerminalTimePast " + terminalTimePast + ")");
    }
    public bool TakeDamage(int amount) {
        currentIP -= amount;
        if (currentIP <= 0)
        {
            Die();
            return false;
        }
        onTakeDamage.Invoke(amount);
        return true;
    }
    public void Die()
    {
        onDeath.Invoke();
        GameObject.Destroy(gameObject, 1f);
    }
    public int GetCurrentIP() {
        return currentIP;
    }
}
public class TakeDamageEvent : UnityEvent<int> { }
public class DeathEvent : UnityEvent { }