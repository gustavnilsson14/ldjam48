using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectoryModifierComponent : EntityComponent
{
    public Component directoryModifierComponent;
    protected Component modifierInstance;

    private void Start()
    {
        modifierInstance = entityBody.currentDirectory.gameObject.AddComponent(directoryModifierComponent.GetType());
        entityBody.onMove.AddListener(OnBodyMove);
        entityBody.onDeath.AddListener(OnBodyDeath);
    }

    private void OnBodyDeath()
    {
        (modifierInstance as DirectoryModifier).DestroyMe();
    }

    private void OnBodyMove(Directory currentDirectory, Directory previousDirectory)
    {
        (modifierInstance as DirectoryModifier).DestroyMe();
        modifierInstance = entityBody.currentDirectory.gameObject.AddComponent(directoryModifierComponent.GetType());
    }
}
