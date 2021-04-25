using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HostHandler : MonoBehaviour
{
    public static HostHandler I;
    public Host currentHost;
    public Host hostPrefab;
    private List<Host> hosts = new List<Host>();
    private GenerateHost generateHost;
    private void Awake()
    {
        generateHost = GetComponent<GenerateHost>();
        HostHandler.I = this;
        hosts.AddRange(GetComponentsInChildren<Host>());
    }

    private void Start()
    {
        currentHost = generateHost.GenerateNewHost(HostType.LINUX, 4, 3, 2);
        //generateHost.PopulateHost(currentHost, 5);
        Player.I.MoveTo(currentHost.GetRootDirectory());
        Player.I.FullRestore();
        IOTerminal.I.RenderUserAndDir();
    }
    public void onSsh(SshKey sshKey)
    {
        Debug.Log("public void onSsh(SshKey sshKey)" + sshKey.GetHost().name);
        Player.I.MoveTo(sshKey.GetHost().GetRootDirectory());
        currentHost = sshKey.GetHost();
        currentHost.SetUser(sshKey.GetUser());
        Player.I.FullRestore();
        IOTerminal.I.RenderUserAndDir();
    }
    public List<Host> GetHosts(bool onlyAvailable = true)
    {
        return hosts.FindAll(host => (host.isAvailable || !onlyAvailable));
    }
    public Host CreateHost() {
        Host newHost = Instantiate(hostPrefab, transform).GetComponent<Host>();
        hosts.Add(newHost);
        return newHost;
    }

}
