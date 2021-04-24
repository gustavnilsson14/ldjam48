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
        Render();

        entity = GetComponent<Entity>();
        if (entity == null)
            return;

        entity.onTakeDamage.AddListener(TakeDamage);
        entity.onDeath.AddListener(Die);
    }

   
    public void Render()
    {
        entityParticle = Instantiate(entityParticlePrefab);
        animator = entityParticle.GetComponent<Animator>();
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
