using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcessHandler : MonoBehaviour
{
    public static ProcessHandler I;
    public List<IProcess> runningProcesses = new List<IProcess>();
    private void Awake()
    {
        ProcessHandler.I = this;
        HostHandler.I.onSsh.AddListener(OnSsh);
    }

    private void Start()
    {
        OnSsh(null);
    }

    private void OnSsh(SshKey sshKey)
    {
        runningProcesses.Clear();
        runningProcesses.AddRange(HostHandler.I.currentHost.GetComponentsInChildren<IProcess>());
        foreach (IProcess process in runningProcesses)
        {
            process.SetPid(GetUniquePid());
        }
    }
    private string GetUniquePid()
    {
        int iterations = 99;
        System.Random generator = new System.Random();
        while (iterations > 0)
        {
            iterations--;
            String pid = generator.Next(0, 1000000).ToString("D6");
            if (runningProcesses.Find(p => p.GetPid() == pid) != null)
                continue;
            return pid;
        }
        return "singularity";
    }
    public bool DisableProcess(string pid) {
        if (!GetProcess(out IProcess process, pid))
            return false;
        process.Disable();
        return true;
    }
    public bool GetProcess(out IProcess process, string pid) {
        process = runningProcesses.Find(p => p.GetPid() == pid && !p.IsDisabled());
        if (process == null)
            return false;
        return true;
    }
}

public interface IProcess
{
    void SetPid(string pid);
    string GetPid();
    void Disable();
    bool IsDisabled();
}