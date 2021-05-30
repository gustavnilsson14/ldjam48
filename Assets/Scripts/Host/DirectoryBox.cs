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
        DiscoveryHandler.I.onAnyDiscovery.AddListener(OnAnyDiscovery);
        DiscoveryHandler.I.onAnyForget.AddListener(OnAnyForget);
        DamageHandler.I.onAnyDeath.AddListener(OnAnyDeath);
    }

    private void OnAnyDeath(IDamageable arg0)
    {
        DisplayCurrentDirectory(Player.I.currentDirectory);
    }

    private void OnAnyForget(IDiscoverable arg0, bool arg1)
    {
        DisplayCurrentDirectory(Player.I.currentDirectory);
    }

    private void OnAnyDiscovery(IDiscoverable arg0, bool arg1)
    {
        DisplayCurrentDirectory(Player.I.currentDirectory);
    }

    private void OnPlayerDamage(bool arg0, int arg1)
    {
        StartCoroutine(OnPlayerDamage());
    }

    private IEnumerator OnPlayerDamage() {
        yield return new WaitForSeconds(0.3f);
        animator.Play("OnPlayerDamage");
    }
    private void OnPlayerMove(Directory currentDirectory, Directory previousDirectory)
    {
        string direction = currentDirectory.GetDepth() > previousDirectory.GetDepth() ? "Up" : "Down";
        animator.Play($"OnPlayerMove{direction}");
        DisplayCurrentDirectory(currentDirectory);
    }

    private void DisplayCurrentDirectory(Directory currentDirectory)
    {
        animator.Play(GetMoodAnimationName(currentDirectory));
    }
    private string GetMoodAnimationName(Directory currentDirectory) {
        if (Player.I.IsSafeInDirectory(currentDirectory))
            return "MoodSafety";
        if (currentDirectory.GetEntities().Find(e => e.faction != Player.I.faction && e.discovered && e.alive))
            return "MoodEnemies";
        return "MoodEmpty";
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
