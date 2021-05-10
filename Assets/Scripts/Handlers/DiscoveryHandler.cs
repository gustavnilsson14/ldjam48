using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DiscoveryHandler : Handler
{
    public static DiscoveryHandler I { get; private set; }
    public List<IDiscoverable> discovered = new List<IDiscoverable>();

    private void Awake()
    {
        DiscoveryHandler.I = this;
    }
    protected override void StartRegister()
    {
        base.StartRegister();
        Player.I.onMove.AddListener(OnPlayerMove);
    }

    public bool Discover(IDiscoverable discoverable)
    {
        if (discovered.Contains(discoverable))
            return false;
        discoverable.Discover();
        discovered.Add(discoverable);
        return true;
    }
    private void OnPlayerMove(Directory arg0, Directory arg1)
    {
        discovered.ForEach(d => d.Forget());
        discovered.Clear();
    }

}
public interface IDiscoverable
{
    string GetName();
    bool IsDiscovered();
    void Discover();
    void Forget();
}
public class DiscoveryEvent : UnityEvent<IDiscoverable, bool> { }