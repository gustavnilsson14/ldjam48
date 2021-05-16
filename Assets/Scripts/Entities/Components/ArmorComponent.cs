using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class ArmorComponent : EntityComponent
{
    public List<IDamageable> protectedComponents = new List<IDamageable>();
    public List<IDamageable> protects;
    public bool protectAllComponents = false;

    public override void StartRegister()
    {
        base.StartRegister();
        if (!protectAllComponents)
            return;
        protectedComponents.Add(entityBody);
        protectedComponents.AddRange(GetComponents<EntityComponent>());
    }

    public bool IsProtecting(IDamageable target, IDamageSource source)
    {
        if (target == this)
            return false;
        if (!protects.Contains(target))
            return false;
        if (source is DirectoryModifier)
            return false;
        return true;
    }
}