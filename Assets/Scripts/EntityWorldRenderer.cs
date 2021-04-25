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
        entity.onDiscover.AddListener(Discover);
    }


    public void Render()
    {
        EntityWorldHandler.I.RenderEntity(entityParticlePrefab, out entityParticle);  
        animator = entityParticle.GetComponent<Animator>();
    }

    private void Discover()
    {
        Render();
    }

    public void Attack()
    {
        RunAnimation("Attack");
    }

    public void TakeDamage(int damage)
    {
        RunAnimation("TakeDamage");
    }

    private void Die()
    {
        RunAnimation("Die");
    }


    public void RunAnimation(string animation)
    {
        if (animator == null)
            return;

        animator.SetTrigger(animation);
    }
}
