using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class ArmorComponent : EntityComponent
{
    public List<IDamageable> protectedComponents = new List<IDamageable>();
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
        if (source is DirectoryModifier)
            return false;
        if (target == this)
            return false;
        if (protectAllComponents)
            return true;
        if (!protectedComponents.Contains(target))
            return false;
        return true;
    }
}