using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HostHandler : MonoBehaviour
{
    public static HostHandler I;
    public Host currentHost;
    //public Host hostPrefab;
    private List<Host> hosts = new List<Host>();
    private GenerateHost generateHost;
    public List<Host> exploredHosts = new List<Host>();
    private void Awake()
    {
        generateHost = GetComponent<GenerateHost>();
        HostHandler.I = this;
        hosts.AddRange(GetComponentsInChildren<Host>());
    }

    private void Start()
    {
        currentHost = generateHost.GenerateNewHost(HostType.LINUX, 4, 3, 2);
        generateHost.PopulateHost(currentHost, 5);
        Player.I.MoveTo(currentHost.GetRootDirectory());
        Player.I.FullRestore();
        IOTerminal.I.RenderUserAndDir();
        exploredHosts.Add(currentHost);
    }

    public void onSsh(SshKey sshKey)
    {
        Player.I.MoveTo(sshKey.GetHost().GetRootDirectory());
        currentHost = sshKey.GetHost();
        currentHost.SetUser(sshKey.GetUser());
        generateHost.PopulateHost(currentHost, 5);
        Player.I.FullRestore();
        IOTerminal.I.RenderUserAndDir();
        exploredHosts.Add(currentHost);
    }
    public List<Host> GetHosts(bool onlyAvailable = true)
    {
        return hosts.FindAll(host => (host.isAvailable || !onlyAvailable));
    }
    public Host CreateHost() {
        Host newHost = generateHost.GenerateNewHost(HostType.LINUX, 4, 3, 2);
        //generateHost.PopulateHost(newHost, 5);
        hosts.Add(newHost);
        return newHost;
    }

}
