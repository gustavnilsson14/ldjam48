﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum EntityFaction
{
    HACKER,
    SECURITY,
    VIRUS
}

public class Entity : ComponentWithIP
{
    public EntityFaction faction;

    [TextArea(2, 10)]
    public string description;

    [Header("Events")]
    public MoveEvent onMove = new MoveEvent();
    public CatEvent onCat = new CatEvent();
    public DiscoverEvent onDiscover = new DiscoverEvent();

    protected override void Awake()
    {
        base.Awake();
        currentDirectory = GetComponentInParent<Directory>();
    }
    public void MoveTo(Directory directory)
    {
        currentDirectory = directory;
        transform.parent = directory.transform;
        onMove.Invoke(directory);
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

    public virtual void Discover()
    {
        onDiscover.Invoke();
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
public class DiscoverEvent : UnityEvent { }