﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HostHandler : MonoBehaviour
{
    public static HostHandler I;
    public Host currentHost;
    //public Host hostPrefab;
    private List<Host> hosts = new List<Host>();
    private GenerateHost generateHost;
    public List<Host> exploredHosts = new List<Host>();
    public CommandEntity commandEntityPrefab;

    private int maxEntities = 1;
    private int maxRootDir = 2;
    private int maxSubDir = 2;
    private int maxDepth = 2;
    private int maxCommands = 1;
    private int maxDirectoryKeys = 1;

    public SshEvent onSsh = new SshEvent();
    private void Awake()
    {
        generateHost = GetComponent<GenerateHost>();
        HostHandler.I = this;
        hosts.AddRange(GetComponentsInChildren<Host>());
    }
    private void Start()
    {
        currentHost = generateHost.GenerateNewHost(HostType.LINUX, maxRootDir, maxSubDir, maxDepth);
        generateHost.PopulateHost(currentHost, maxEntities, maxCommands, maxDirectoryKeys);
        Player.I.MoveTo(currentHost.GetRootDirectory());
        Player.I.FullRestore();
        AudioHandler.I.PlayMusic();
        Player.I.name = currentHost.userName + ".lock";
        IOTerminal.I.RenderUserAndDir();
        exploredHosts.Add(currentHost);
    }
    public void OnSsh(SshKey sshKey)
    {
        Player.I.MoveTo(sshKey.GetHost().GetRootDirectory());
        currentHost = sshKey.GetHost();
        currentHost.SetUser(sshKey.GetUser());
        generateHost.PopulateHost(currentHost, maxEntities, maxCommands, maxDirectoryKeys);
        Player.I.LevelUp();
        Player.I.FullRestore();
        AudioHandler.I.PlayMusic();
        Player.I.name = currentHost.userName + ".lock";
        IOTerminal.I.RenderUserAndDir();
        exploredHosts.Add(currentHost);
        IOTerminal.I.DisplayLevelUp();
        onSsh.Invoke(sshKey);
    }
    public List<Host> GetHosts(bool onlyAvailable = true)
    {
        return hosts.FindAll(host => (host.isAvailable || !onlyAvailable));
    }
    public Host CreateHost() {
        MakeHostsHarder();
        Host newHost = generateHost.GenerateNewHost(HostType.LINUX, maxRootDir, maxSubDir, maxDepth);
        hosts.Add(newHost);
        return newHost;
    }
    public void MakeHostsHarder()
    {
        maxEntities = maxEntities + maxRootDir;
        maxRootDir++;
        maxSubDir++;
        maxDepth++;
        maxCommands++;
        maxDirectoryKeys++;
    }
}

public class SshEvent : UnityEvent<SshKey> { }