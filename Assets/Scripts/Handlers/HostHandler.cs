using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HostHandler : MonoBehaviour
{
    public static HostHandler I;
    public Host hostPrefab;
    private List<Host> hosts = new List<Host>();
    private void Awake()
    {
        HostHandler.I = this;
        hosts.AddRange(GetComponentsInChildren<Host>());
        hosts.Add(CreateHost());
    }

    public List<Host> GetHosts(bool onlyAvailable = true)
    {
        return hosts.FindAll(host => (host.isAvailable || !onlyAvailable));
    }
    public Host CreateHost() {
        Host newHost = Instantiate(hostPrefab, transform).GetComponent<Host>();
        return newHost;
    }

}
