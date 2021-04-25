using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Host : MonoBehaviour
{
    public bool isAvailable = false;
    public string userName = "haxxor";
    public List<PublicKey> keys = new List<PublicKey>();
    public Transform KeysTransform;
    private void Awake()
    {
        keys.AddRange(GetComponentsInChildren<PublicKey>());
    }
    public Directory GetRootDirectory()
    {
        return GetComponentInChildren<Directory>();
    }
    public void SetUser(string userName)
    {
        this.userName = userName;
    }
    public void RegisterKey(PublicKey publicKey)
    {
        if (keys.Contains(publicKey))
            return;
        keys.Add(publicKey);
    }
}
