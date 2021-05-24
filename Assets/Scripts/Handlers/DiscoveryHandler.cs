﻿using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DiscoveryHandler : Handler
{
    public static DiscoveryHandler I { get; private set; }
    public List<IDiscoverable> discovered = new List<IDiscoverable>();
    public DiscoveryEvent onAnyDiscovery = new DiscoveryEvent();
    public DiscoveryEvent onAnyForget = new DiscoveryEvent();
    private void Awake()
    {
        DiscoveryHandler.I = this;
    }
    protected override void StartRegister()
    {
        base.StartRegister();
        Player.I.onMove.AddListener(OnPlayerMove);
    }

    public void InitDiscoverable(IDiscoverable discoverable)
    {
        discoverable.discovered = false;
        discoverable.onDiscover = new DiscoveryEvent();
        discoverable.onForget = new DiscoveryEvent();
        discoverable.onCat = new CatEvent();
        discoverable.currentDirectory = discoverable.GetGameObject().GetComponentInParent<Directory>();
    }

    public bool Discover(IDiscoverable discoverable)
    {
        if (discoverable == Player.I)
            return false;
        Debug.Log($"1 --- {discoverable}");
        if (discovered.Contains(discoverable))
            return false;
        Debug.Log($"2 --- {discoverable}");
        if (discoverable.currentDirectory != Player.I.currentDirectory && !(discoverable is ImageEntity))
            return false;
        Debug.Log($"3 --- {discoverable}");
        discoverable.discovered = true;
        discoverable.onDiscover.Invoke(discoverable, false);
        onAnyDiscovery.Invoke(discoverable, false);
        discovered.Add(discoverable);
        return true;
    }
    public void Forget(IDiscoverable discoverable)
    {
        if (discoverable == Player.I)
            return;
        discoverable.discovered = false;
        discoverable.onForget.Invoke(discoverable, false);
        onAnyForget.Invoke(discoverable, false);
    }
    public string GetCatDescription(IDiscoverable discoverable) {

        List<string> catDescription = new List<string> {
            StringUtil.GetBinaryStatic(),
            discoverable.GetFileName(),
            discoverable.GetShortDescription(),
        };
        catDescription = discoverable.FormatCatDescription(catDescription);
        discoverable.onCat.Invoke(catDescription);
        return string.Join("\n", catDescription);
    }
    private void OnPlayerMove(Directory arg0, Directory arg1)
    {
        ForgetAll();
    }
    public void ForgetAll()
    {
        discovered.ForEach(d => Forget(d));
        discovered.Clear();
    }

}
public interface IDiscoverable
{
    void InitDiscoverable();
    string GetName();
    GameObject GetGameObject();
    Directory currentDirectory { get; set; }
    DiscoveryEvent onDiscover { get; set; }
    DiscoveryEvent onForget { get; set; }
    CatEvent onCat { get; set; }
    bool discovered { get; set; }
    List<string> FormatCatDescription(List<string> catDescription);
    string GetShortDescription();
    string GetFileName();
}
public class DiscoveryEvent : UnityEvent<IDiscoverable, bool> { }
public class CatEvent : UnityEvent<List<string>> { }