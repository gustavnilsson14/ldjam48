using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Entity : ComponentWithIP
{
    public Directory currentDirectory;

    [TextArea(2,10)]
    public string description;

    [Header("Events")]
    public CatEvent onCat = new CatEvent();

    protected override void Awake()
    {
        base.Awake();
        currentDirectory = GetComponentInParent<Directory>();
    }
    public virtual string GetCatDescription()
    {
        List<string> result = new List<string> { 
            GetBinaryStatic(),
            string.Format("IP: {0}", currentIP),
            description
        };
        onCat.Invoke();
        return string.Join("\n", result);
    }
    protected string GetBinaryStatic()
    {
        List<string> result = new List<string>();
        var rand = new System.Random();
        for (int i = 0; i < 10; i++)
        {
            result.Add(Convert.ToString(rand.Next(1024), 2).PadLeft(10, '0'));
        }
        return string.Join("\n", result);
    }
}
public class CatEvent : UnityEvent { }