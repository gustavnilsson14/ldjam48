using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class NetstatCommand : Command
{
    public override bool Run(out string result, ParsedCommand parsedCommand)
    {
        if (!base.Run(out result, parsedCommand))
            return false;
        result = "Current connections;\n";
        result += string.Join("\n", HostHandler.I.exploredHosts.Select(host => $"{host.name} as {host.userName}")) + "\n";
        return true;
    }
}
