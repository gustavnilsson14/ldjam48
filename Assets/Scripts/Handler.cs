using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Handler : MonoBehaviour
{
    private void Awake()
    {
        FieldInfo field = GetType().GetField("I", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        field.SetValue(null, this);
    }
    private void Start()
    {
        StartRegister();
    }

    protected virtual void StartRegister() { }
}
