using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Host : MonoBehaviour
{
    public bool isAvailable = false;
    public string userName = "haxxor";
    public List<PublicKey> keys = new List<PublicKey>();
    public Transform keysTransform;
    public float lootValueMultiplier = 1f;
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
}
