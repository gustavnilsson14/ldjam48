using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class DirectoryBox : Handler
{
    public static DirectoryBox I;
    private Animator animator;

    protected override void StartRegister()
    {
        base.StartRegister(); 
        animator = GetComponent<Animator>();
        
        Player.I.onBodyDamage.AddListener(OnPlayerDamage);
        Player.I.onMove.AddListener(OnPlayerMove);
    }

    private void OnPlayerDamage(bool arg0, int arg1)
    {
        animator.Play("OnPlayerDamage");
    }

    private void OnPlayerMove(Directory currentDirectory, Directory previousDirectory)
    {
        string direction = currentDirectory.GetDepth() > previousDirectory.GetDepth() ? "Up" : "Down";
        animator.Play($"OnPlayerMove{direction}");
    }
    /*
     
        if (Player.I.IsSafeInDirectory(directory))
        {
            RunAnimation("Friendly");
            return;
        }
        RunAnimation("Danger");

     */
}
