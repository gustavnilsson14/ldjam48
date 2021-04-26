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
        entity.onAttack.AddListener(Attack);
        entity.onPlayerEscape.AddListener(PlayerEscape);
        entity.onMove.AddListener(Move);

    }

    private void Move(Directory target, Directory origin)
    {
        EntityWorldHandler.I.RemoveChildFromSpawnPoint(entityParticle);
    }

    private void PlayerEscape()
    {
        EntityWorldHandler.I.RemoveChildFromSpawnPoint(entityParticle);
    }

    public void Render()
    {
        if(!EntityWorldHandler.I.RenderEntity(entityParticlePrefab, out entityParticle))
            return;
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
