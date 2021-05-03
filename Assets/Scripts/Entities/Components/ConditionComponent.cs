using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionComponent : AttackComponent, IConditionOrigin
{
    public Condition onHitCondition;
    protected override void Start()
    {
        base.Start();
        onHitCondition.SetToStatic();
    }
    protected override void DealDamage(ComponentWithIP target, int damage)
    {
        base.DealDamage(target, damage);
        Condition newCondition = target.gameObject.AddComponent(onHitCondition.GetType()) as Condition;
        newCondition.Init(this, onHitCondition);
    }
    public string GetSource()
    {
        string entityName = $"{entityBody.currentDirectory.GetFullPath()}/{entityBody.name}";
        string sourceName = entityBody.isDiscovered ? entityName : "something unknown";
        return $"Lingering effects from the attack of {sourceName}";
    }
}
