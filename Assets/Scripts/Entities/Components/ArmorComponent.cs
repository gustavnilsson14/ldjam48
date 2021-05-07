using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class ArmorComponent : EntityComponent
{
    public List<ComponentWithIP> protectedComponents;

    public bool IsProtecting(ComponentWithIP componentWithIP, IDamageSource source)
    {
        if (componentWithIP == this)
            return false;
        if (!protectedComponents.Contains(componentWithIP))
            return false;
        if (source is DirectoryModifier)
            return false;
        return true;
    }
}