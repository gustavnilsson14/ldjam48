using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeysCommand : Command
{
    public override bool Run(out string result, ParsedCommand parsedCommand)
    {
        if (!base.Run(out result, parsedCommand))
            return false;
        List<PublicKey> allAvailableKeys = new List<PublicKey>();
        allAvailableKeys.AddRange(HostHandler.I.currentHost.keys.FindAll(key => key.isAvailable));
        allAvailableKeys.Sort((a, b) => (a is SshKey) ? 1 : 0);
        if (allAvailableKeys.Count == 0)
        {
            result = "You have no keys";
            return true;
        }
        foreach (PublicKey key in allAvailableKeys)
        {
            string keyType = "Directory key";
            if (key is SshKey)
                keyType = "Ssh key";
            result += $"{keyType}: {key.GetName()}\n";
        }
        return true;
    }
}
