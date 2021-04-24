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

    [TextArea(2,10)]
    public string description;

    [Header("Events")]
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
        
    }
    protected virtual void OnTerminalTimePast(int terminalTimePast)
    {
        
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
    public string GetCatDescription()
    {
        List<string> result = new List<string> { 
            GetBinaryStatic(),
            string.Format("IP: {0}", currentIP),
            description
        };
        return string.Join("\n", result);
    }

    private string GetBinaryStatic()
    {
        List<string> result = new List<string>();
        var rand = new System.Random();
        for (int i = 0; i < 10; i++)
        {
            result.Add(Convert.ToString(rand.Next(1024), 2).PadLeft(10, '0'));
        }
        return string.Join("\n", result);
    }
}
public class TakeDamageEvent : UnityEvent<int> { }
public class DeathEvent : UnityEvent { }