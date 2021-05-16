using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DdosKey : Expendable, IDamageSource
{
    public int damageBase = 5;
    public override void Consume(out string result)
    {
        base.Consume(out result);
        result = $"{GetComponentTypeName()} deals {damageBase} to;\n";
        List<string> damageResults = new List<string>();
        foreach (Entity entity in Player.I.currentDirectory.GetEntities().FindAll(e => e.faction != Player.I.faction && e.alive))
        {
            string damageString = $"    {entity.name}";
            if (DamageHandler.I.TakeHit(entity, this, out int armorDamageTaken, out int bodyDamageTaken))
                damageString = $"{damageString}, which crumbles to bits";
            damageResults.Add(damageString);
        }
        result = $"{result} {string.Join("\n", damageResults)}";
    }

    public int GetDamageBase()
    {
        return damageBase;
    }

    public string GetDamageSourceName()
    {
        return "ddoskey";
    }

    public int GetTotalDamage()
    {
        return GetDamageBase();
    }
}
