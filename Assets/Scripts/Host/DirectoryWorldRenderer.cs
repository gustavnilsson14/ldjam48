using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectoryWorldRenderer : MonoBehaviour
{
    private Animator animator;
    void Start()
    {
        Player.I.onMove.AddListener(PlayerMoved);
        Player.I.onTakeDamage.AddListener(PlayerTookDamage);

        animator = GetComponent<Animator>();
    }

    private void PlayerTookDamage(int amount)
    {
        RunAnimation("Take_Damage");
    }

    private void PlayerMoved(Directory directory)
    {
        
        if (directory.privilege == Directory.DirectoryPrivilege.DANGER)
            RunAnimation("Danger");
        if (directory.privilege == Directory.DirectoryPrivilege.FRIENDLY)
            RunAnimation("Friendly");
    }
    public void RunAnimation(string animation)
    {
        if (animator == null)
            return;

        animator.SetTrigger(animation);
    }
}
