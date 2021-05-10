using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Host : MonoBehaviour, IDamageSource
{
    public bool isAvailable = false;
    public string userName = "haxxor";
    public List<PublicKey> keys = new List<PublicKey>();
    public Transform keysTransform;
    public float lootValueMultiplier = 1f;

    private void Start()
    {
        Register();
    }

    private void Register()
    {
        keys.AddRange(GetRootDirectory().GetComponentsInChildren<PublicKey>());
    }

    public void Init(int levelIndex)
    {
        name = NameUtil.I.GetHostName();
        if (!TryGetComponent<HostGenerator>(out HostGenerator hostGenerator))
            return;
        hostGenerator.Run(levelIndex);
    }
    public Directory GetRootDirectory()
    {
        return GetComponentInChildren<Directory>();
    }
    public void SetUser(string userName)
    {
        this.userName = userName;
    }
    public void RegisterKey(StoredObject key)
    {
        PublicKey newKey = gameObject.AddComponent(key.objectType) as PublicKey;
        if (!ReflectionUtil.ApplyStoredObject(key, newKey))
            return;
        keys.Add(newKey);
    }
    public bool GetDirectoryByPath(string path, out Directory directory)
    {
        directory = GetRootDirectory();
        if (!GetPathSegments(out List<string> pathSegments, path))
            return false;
        int iterations = 99;
        while (pathSegments.Count > 0 && iterations > 0)
        {
            iterations--;
            string pathSegment = pathSegments[0];
            pathSegments.RemoveAt(0);
            Transform child = directory.transform.Find(pathSegment);
            if (child == null)
                return false;
            Directory childDirectory = child.GetComponent<Directory>();
            if (childDirectory == null)
                return false;
            directory = childDirectory;
        }
        return true;
    }
    public bool GetPathSegments(out List<string> pathSegments, string path) {

        pathSegments = new List<string>();
        pathSegments.AddRange(path.Split(new string[] { "/" }, StringSplitOptions.None));
        if (pathSegments.Count == 0)
            return false;
        pathSegments.RemoveAt(0);
        if (pathSegments[0] != "root")
            return false;
        pathSegments.RemoveAt(0);
        return true;
    }
    public List<EntityFaction> GetPresentFactions()
    {
        List<EntityFaction> result = new List<EntityFaction>();
        foreach (Entity entity in GetComponentsInChildren<Entity>())
        {
            if (result.Contains(entity.faction))
                continue;
            result.Add(entity.faction);
        }
        return result;
    }
    public List<Directory> GetLeafDirectories()
    {
        return GetComponentsInChildren<Directory>().ToList().FindAll(d => d.GetChildren().Count == 0);
    }
    public List<Directory> GetLeafDirectories(Directory directory)
    {
        return directory.GetComponentsInChildren<Directory>().ToList().FindAll(d => d.GetChildren().Count == 0);
    }
    public List<Directory> GetBranchDirectories()
    {
        return GetComponentsInChildren<Directory>().ToList().FindAll(d => d.GetChildren().Count != 0 && d != GetRootDirectory());
    }
    public int GetDamageBase()
    {
        return 1;
    }
    public int GetTotalDamage()
    {
        return 1;
    }
    public string GetDamageSourceName()
    {
        return name;
    }
}