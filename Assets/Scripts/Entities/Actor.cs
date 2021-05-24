using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Actor : MonoBehaviour, IDamageable
{
    public int speed = 4;
    public bool isRealTime = false;
    protected float currentMomentum = 0;

    public ActorRunEvent onRun = new ActorRunEvent();
    public int maxIP;

    private void Start()
    {
        StartRegister();
    }
    public virtual void StartRegister()
    {
        IOTerminal.I.onCommand.AddListener(OnCommand);
        IOTerminal.I.onTerminalTimePast.AddListener(OnTerminalTimePast);
        Player.I.onRealTime.AddListener(OnRealTime);
        InitDamageable();
    }

    public void InitDamageable()
    {
        DamageHandler.I.InitDamageable(this);
    }
    protected virtual void OnCommand(Command command, ParsedCommand parsedCommand) { }
    protected virtual void OnTerminalTimePast(int terminalTimePast)
    {
        if (isRealTime)
            return;
        OnTimePast(terminalTimePast);
    }
    protected void OnRealTime()
    {
        if (!isRealTime)
            return;
        OnTimePast(Time.deltaTime);
    }
    protected virtual void OnTimePast(float time) {
        if (speed == 0)
            return;
        currentMomentum += time;
        int turnsToTake = Mathf.FloorToInt((float)currentMomentum / (float)speed);
        currentMomentum -= turnsToTake * speed;
        for (int i = 0; i < turnsToTake; i++)
        {
            Run();
        }
    }
    protected virtual void Run() {
        onRun.Invoke(this);
    }

    public virtual string GetName() => name;
    public virtual string GetDescription() => "";

    public int GetMaxIP() => maxIP;

    public GameObject GetGameObject() => gameObject;

    public bool IsBaseComponent() => false;

    public bool alive { get; set; }
    public int currentIP { get; set; }
    public TakeHitEvent onTakeHit { get; set; }
    public ArmorDamageEvent onArmorDamage { get; set; }
    public BodyDamageEvent onBodyDamage { get; set; }
    public DirectDamageEvent onDirectDamage { get; set; }
    public HitTakenEvent onHitTaken { get; set; }
    public HealEvent onHeal { get; set; }
    public DeathEvent onDeath { get; set; }
    Directory IDamageable.currentDirectory { get; set; }
}
public class ActorRunEvent : UnityEvent<Actor> { }