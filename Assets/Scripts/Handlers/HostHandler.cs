using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public enum HostType
{
    LINUX,
    WINDOWS,
    MAC,
    TUTORIAL
}

public class HostHandler : Handler
{
    public static HostHandler I;
    public Host currentHost;
    //public Host hostPrefab;
    private List<Host> hosts = new List<Host>();
    public List<Host> exploredHosts = new List<Host>();
    public CommandEntity commandEntityPrefab;

    public SshEvent onSsh = new SshEvent();
    
    [Header("Host Prefabs")]
    public Host tutorialHostPrefab;

    public Host GetNextHost(HostType hostType = HostType.LINUX)
    {
        if (hostType == HostType.TUTORIAL)
            return Instantiate(tutorialHostPrefab, transform).GetComponent<Host>();
        return Instantiate(tutorialHostPrefab, transform).GetComponent<Host>();
    }
    protected override void StartRegister()
    {
        base.StartRegister();
        hosts.AddRange(GetComponentsInChildren<Host>());
        currentHost = GetNextHost(HostType.TUTORIAL);
        Player.I.MoveTo(currentHost.GetRootDirectory());
        currentHost.Init(3);
    }

    public void OnSsh(SshKey sshKey)
    {
        Debug.Log(sshKey);
        exploredHosts.Add(currentHost);
        Player.I.MoveTo(sshKey.GetHost().GetRootDirectory());
        currentHost = sshKey.GetHost();
        currentHost.SetUser(sshKey.GetUser());
        currentHost.Init(3);
        onSsh.Invoke(sshKey);
    }
    public List<Host> GetHosts(bool onlyAvailable = true)
    {
        return hosts.FindAll(host => (host.isAvailable || !onlyAvailable));
    }
}

public class SshEvent : UnityEvent<SshKey> { }
public interface IGeneratedHostInhabitant
{
    bool GeneratesInLeafDirectory();
    bool GeneratesInBranchDirectory();
    bool GeneratesInPriorityDirectory();
    float GetRarity();
}