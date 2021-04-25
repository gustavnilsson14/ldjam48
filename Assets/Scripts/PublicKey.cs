using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PublicKey : MonoBehaviour
{
    public bool isAvailable = false;
    protected virtual void Awake() { }
    public virtual string GetName() { return name; }
    public virtual string GetUsageDescription() { return ""; }
}
