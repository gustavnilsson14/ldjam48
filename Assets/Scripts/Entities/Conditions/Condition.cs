using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Condition : Actor
{
    public int lifeTime = 30;
    protected float currentLifeTime;
    private IConditionOrigin origin;
    private bool isStatic = false;

    public override void StartRegister()
    {
        base.StartRegister();
        currentLifeTime = lifeTime;
    }
    public virtual void Init(IConditionOrigin origin) {
        this.origin = origin;
    }
    public void SetToStatic() {
        isStatic = true;
    }
    public bool IsActiveCondition() {
        if (isStatic)
            return false;
        return true;
    }
    protected override void OnTimePast(float time)
    {
        base.OnTimePast(time);
        if (isStatic)
            return;
        if (currentLifeTime == -1)
            return;
        currentLifeTime -= time;
        if (currentLifeTime > 0)
            return;
        DamageHandler.I.Die(this);
    }
    public string GetSource()
    {
        if (origin == null)
            return "Without Source";
        return origin.GetSource();
    }
}

public interface IConditionOrigin {
    string GetSource();
}
public interface ICommandDisabler
{
    string GetDescription();
    string GetSource();
    Command GetCommand();
}
