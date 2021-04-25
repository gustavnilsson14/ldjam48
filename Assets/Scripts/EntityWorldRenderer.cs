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
    private bool isDiscovered = false;

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
        if(!EntityWorldHandler.I.RenderEntity(entityParticlePrefab, out entityParticle))
            return;
        animator = entityParticle.GetComponent<Animator>();
    }

    private void Discover()
    {
        if (isDiscovered)
            return;

        isDiscovered = true;
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
