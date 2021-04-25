﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SshKey : PublicKey
{
    protected Host targetHost;
    protected string userName;
    protected override void Awake()
    {
        base.Awake();
        userName = NameUtil.I.GetHackerName();
        targetHost = HostHandler.I.CreateHost();
    }
    public Host GetHost()
    {
        return targetHost;
    }
    public string GetUser()
    {
        return userName;
    }
    public override string GetName()
    {
        return $"{userName}@{targetHost.name}";
    }
    public override string GetUsageDescription()
    {
        return $"You can now use \"ssh {GetName()} to proceed deeper into cyberspace!\"";
    }
}
