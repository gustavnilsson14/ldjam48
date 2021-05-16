using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DamageHandler : Handler
{
    public static DamageHandler I;
    public void InitDamageable(IDamageable target)
    {
        target.currentIP = target.GetMaxIP();
        target.alive = true;
        target.onArmorDamage = new ArmorDamageEvent();
        target.onBodyDamage = new BodyDamageEvent();
        target.onDirectDamage = new DirectDamageEvent();
        target.onHeal = new HealEvent();
        target.onDeath = new DeathEvent();
        target.onTakeHit = new TakeHitEvent();
        target.onHitTaken = new HitTakenEvent();
    }
    public bool TakeHit(IDamageable target, IDamageSource source)
    {
        return TakeHit(target, source, out int armorDamageTaken, out int bodyDamageTaken);
    }
    public bool TakeHit(IDamageable target, IDamageSource source, out int armorDamageTaken, out int bodyDamageTaken) {
        target.onTakeHit.Invoke(source);
        bool result = TakeDamage(target, source, out armorDamageTaken, out bodyDamageTaken);
        target.onHitTaken.Invoke(source, result, armorDamageTaken, bodyDamageTaken);
        return result;
    }
    public bool TakeDamage(IDamageable target, IDamageSource source, out int armorDamageTaken, out int bodyDamageTaken)
    {
        int remainingDamage = source.GetTotalDamage();
        armorDamageTaken = 0;
        bodyDamageTaken = 0;
        if (!target.alive)
            return false;
        if (ArmorAbsorbedHit(target, source, ref remainingDamage, out armorDamageTaken))
            return false;
        if (SurvivedHit(target, source, ref remainingDamage, out bodyDamageTaken))
            return false;
        return true;
    }
    public bool TakeDirectDamage(IDamageable target, IDamageSource source) {
        return TakeDirectDamage(target, source, out int damageTaken);
    }
    public bool TakeDirectDamage(IDamageable target, IDamageSource source, out int damageTaken)
    {
        damageTaken = Mathf.Clamp(source.GetTotalDamage(), 0, target.currentIP);
        target.currentIP -= damageTaken;
        bool result = target.currentIP > 0;
        target.onDirectDamage.Invoke(result, damageTaken);
        if (!result)
            Die(target);
        return result;
    }
    protected bool ArmorAbsorbedHit(IDamageable target, IDamageSource source, ref int remainingDamage, out int damageTaken)
    {
        damageTaken = 0;
        if (!target.GetGameObject().TryGetComponent<ArmorComponent>(out ArmorComponent armor))
            return false;
        if (!armor.IsProtecting(target, source))
            return false;
        bool result = DamageHandler.I.SurvivedHit(armor, source, ref remainingDamage, out damageTaken);
        target.onArmorDamage.Invoke(armor, result, damageTaken);
        return result;
    }
    public bool SurvivedHit(IDamageable target, IDamageSource source)
    {
        int remainingDamage = source.GetTotalDamage();
        return SurvivedHit(target, source, ref remainingDamage, out int damageTaken);
    }
    public bool SurvivedHit(IDamageable target, IDamageSource source, ref int remainingDamage, out int damageTaken)
    {
        damageTaken = Mathf.Clamp(remainingDamage, 0, target.currentIP);
        remainingDamage = Mathf.Clamp(remainingDamage - damageTaken, 0, int.MaxValue);
        target.currentIP -= damageTaken;
        bool result = target.currentIP > 0;
        target.onBodyDamage.Invoke(result, damageTaken);
        if (!result)
            Die(target);
        return result;
    }
    public float GetDamageMultiplier(IDamageable target)
    {
        float multiplier = 1;
        List<DirectoryModifier> charactersMultipliers = target.currentDirectory.GetModifiers().FindAll(modifier => modifier is DamageMultiplier);
        foreach (DirectoryModifier modifier in charactersMultipliers)
        {
            multiplier += (modifier as DamageMultiplier).multiplier;
        }
        return Mathf.Clamp(multiplier, 0, Mathf.Infinity);
    }

    public bool Heal(IDamageable target, int amount)
    {
        if (target.currentIP >= target.GetMaxIP())
        {
            return false;
        }
        target.currentIP = Mathf.Clamp(target.currentIP + amount, 0, target.GetMaxIP());
        target.onHeal.Invoke(amount);
        return true;
    }
    public virtual void Die(IDamageable target)
    {
        target.alive = false;
        target.onDeath.Invoke();
        if (target is IWorldPositionObject) {
            Destroy((target as IWorldPositionObject).instance, 1f);
            WorldPositionHandler.I.PlayAnimation((target as IWorldPositionObject),"Die");
        }
            
        if (target is ILootDropper)
            (target as ILootDropper).onLootDrop.Invoke((target as ILootDropper));
        GameObject.Destroy(target.GetGameObject());
    }
    /*
    /// <summary>
    /// Method for registering components etc.
    /// </summary>
    public virtual void StartRegister() { }
    public virtual string GetName()
    {
        return name;
    }
    public virtual string GetDescription()
    {
        return description;
    }
     
     */

}
public interface IDamageable {
    void InitDamageable();
    string GetName();
    int GetMaxIP();
    GameObject GetGameObject();
    Directory currentDirectory { get; set; }
    bool alive { get; set; }
    int currentIP { get; set; }
    TakeHitEvent onTakeHit { get; set; }
    ArmorDamageEvent onArmorDamage { get; set; }
    BodyDamageEvent onBodyDamage { get; set; }
    DirectDamageEvent onDirectDamage { get; set; }
    HitTakenEvent onHitTaken { get; set; }
    HealEvent onHeal { get; set; }
    DeathEvent onDeath { get; set; }
}
public interface IDamageSource
{
    int GetDamageBase();
    int GetTotalDamage();
    string GetDamageSourceName();
}
public class HealEvent : UnityEvent<int> { }
public class TakeHitEvent : UnityEvent<IDamageSource> { }
public class ArmorDamageEvent : UnityEvent<ArmorComponent, bool, int> { }
public class BodyDamageEvent : UnityEvent<bool, int> { }
public class DirectDamageEvent : UnityEvent<bool, int> { }
public class HitTakenEvent : UnityEvent<IDamageSource, bool, int, int> { }
public class DeathEvent : UnityEvent { }