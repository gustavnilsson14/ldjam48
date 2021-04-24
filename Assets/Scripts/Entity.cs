using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public Directory currentDirectory;
    private void Awake()
    {
        currentDirectory = GetComponentInParent<Directory>();
    }
    private void Start()
    {
        IOTerminal.I.onTerminalTimePast.AddListener(OnTerminalTimePast);
    }
    protected virtual void OnTerminalTimePast(int amount)
    {
        Debug.Log("protected virtual void OnTerminalTimePast(int "+ amount+")");
    }
}
