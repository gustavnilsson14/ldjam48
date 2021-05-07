using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionComponent : AttackComponent, IConditionOrigin
{
    public List<StoredObject> onHitConditions = new List<StoredObject>();
    public Transform onHitConditionsRoot;
    protected override void Start()
    {
        base.Start();
        if (onHitConditionsRoot == null)
            return;
        foreach (Condition onHitCondition in onHitConditionsRoot.GetComponents<Condition>())
        {
            ReflectionUtil.GetStoredObject(out StoredObject storedOnHitCondition, onHitCondition);
            onHitConditions.Add(storedOnHitCondition);
        }
        Destroy(onHitConditionsRoot.gameObject);
    }
    protected override void DealDamage(Entity target)
    {
        base.DealDamage(target);
        foreach (StoredObject storedOnHitCondition in onHitConditions)
        {
            Condition newCondition = target.gameObject.AddComponent(storedOnHitCondition.objectType) as Condition;
            if (newCondition == null)
                continue;
            ReflectionUtil.ApplyStoredObject(storedOnHitCondition, newCondition);
            newCondition.Init(this);
        }
    }
    public string GetSource()
    {
        string entityName = $"{entityBody.currentDirectory.GetFullPath()}/{entityBody.name}";
        string sourceName = entityBody.isDiscovered ? entityName : "something unknown";
        return $"Lingering effects from the attack of {sourceName}";
    }
}
