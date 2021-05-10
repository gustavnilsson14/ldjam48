using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityWorldRenderer : MonoBehaviour
{
    public GameObject entityParticlePrefab;

    private Entity entity;
    private GameObject entityParticle;
    private Animator animator;

    private void Start()
    {
        entity = GetComponent<Entity>();
        if (entity == null)
            return;

        entity.onTakeDamage.AddListener(TakeDamage);
        entity.onDeath.AddListener(Die);
        entity.GetOnDiscover().AddListener(OnDiscover);
        entity.onAttack.AddListener(Attack);
        entity.GetOnForget().AddListener(OnForget);
        entity.onMove.AddListener(Move);
    }

    private void OnDiscover(IDiscoverable arg0, bool arg1)
    {
        Render();
    }

    private void OnForget(IDiscoverable arg0, bool arg1)
    {
        EntityWorldHandler.I.RemoveChildFromSpawnPoint(entityParticle);
    }

    private void Move(Directory target, Directory origin)
    {
        EntityWorldHandler.I.RemoveChildFromSpawnPoint(entityParticle);
    }

    public void Render()
    {
        if(!EntityWorldHandler.I.RenderEntity(entityParticlePrefab, out entityParticle))
            return;
        animator = entityParticle.GetComponent<Animator>();
    }

    public void Attack()
    {
        RunAnimation("Attack");
    }

    public void TakeDamage(int damage)
    {
        if (animator == null)
            return;
        animator.Play("TakeDamage");
    }

    private void Die()
    {
        EntityWorldHandler.I.RemoveChildFromSpawnPointDelay(entityParticle, 3);
        RunAnimation("Die");
    }

    public void RunAnimation(string animation)
    {
        if (animator == null)
            return;

        animator.SetTrigger(animation);
    }
}
