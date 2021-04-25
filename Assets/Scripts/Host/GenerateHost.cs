﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum HostType
{
    LINUX,
    WINDOWS,
    MAC
}

public class GenerateHost : MonoBehaviour
{
    public List<Entity> entitiesPrefabs = new List<Entity>();
    public DirectoryKey directoryKeyPrefab;
    public SshKey sshKeyPrefab;


    public Host linuxHostPrefab;
    public Host GenerateNewHost(HostType hosttype, int maxDirRoot, int maxDirSub, int maxDepth)
    {
        Host host = Instantiate(linuxHostPrefab, transform);
        host.name = "host" + Random.Range(0, 9999);
        CleanUnderRoot(host, maxDirRoot, maxDirSub, maxDepth);
        return host;
    }
    public IEnumerator WaitBeforePopulate(Host host, int maxEntities)
    {
        yield return null;
        Populate(host, maxEntities);
        GenerateSshKey(host);
        GenerateDirectoryKey(host);

    }
    public void PopulateHost(Host host, int maxEntities)
    {
        StartCoroutine(WaitBeforePopulate(host, maxEntities));
    }

    private void GenerateSshKey(Host host)
    {
        if(!GetRandomDirectory(host.transform, out Directory directory))
            return;

        SshKey sshKey = Instantiate(sshKeyPrefab, host.KeysTransform);
        KeyEntity keyEntity = sshKey.InstantiateEntityKey(directory.transform);
        keyEntity.publicKey = sshKey;
        keyEntity.name = $"{sshKey.GetHost().name}.ssh";
    }

    private void GenerateDirectoryKey(Host host)
    {
        if (!GetRandomDirectory(host.transform, out Directory directory))
            return;

        DirectoryKey directoryKey = Instantiate(directoryKeyPrefab, host.KeysTransform);
        KeyEntity keyEntity = directoryKeyPrefab.InstantiateEntityKey(directory.transform);
        keyEntity.publicKey = directoryKeyPrefab;
    }

    private void Populate(Host host, int maxEntities)
    {
        for(int i = 0; i < maxEntities; i++)
        {
            if (!GetRandomDirectory(host.transform, out Directory directory))
                return;

            if (!GetRandomEntity(out Entity entity))
                return;

            Instantiate(entity, directory.transform);
        }
    }
    
    public bool GetRandomEntity(out Entity entity)
    {
        entity = null;
        if (entitiesPrefabs.Count == 0)
            return false;

        entity = entitiesPrefabs[Random.Range(0, entitiesPrefabs.Count)];
        return true;
    }

    public bool GetRandomDirectory(Transform directoryTransform, out Directory directory)
    {
        directory = null;
        List<Directory> children = new List<Directory>();
        children.AddRange(directoryTransform.GetComponentsInChildren<Directory>());

        if (children.Count == 0)
            return false;

        directory = children[Random.Range(0, children.Count)];
        return true;
    }

    private void CleanUnderRoot(Host host, int maxDirRoot, int maxDirSub, int maxDepth)
    {
        CleanDepth(host.GetRootDirectory().transform, maxDepth, maxDirRoot, false, false);

        List<Directory> directories = GetFirstDepthChildren(host.GetRootDirectory().transform);
        foreach(Directory dir in directories)
        {
            CleanDepth(dir.transform, maxDepth, maxDirSub, true, true, 2);
        }
    }

    private void CleanDepth(Transform t, int maxDepth, int maxDir, bool randomNumber = false, bool recusiveClean = false, int depth = 1)
    {
        List<Directory> directories = GetFirstDepthChildren(t);

        if (maxDir >= directories.Count)
            return;

        if (depth > maxDepth)
            ClearDirectory(directories);

        int dirNumber = randomNumber ? Random.Range(0, maxDir + 1) : maxDir;
        for (int i = directories.Count; i > dirNumber; i--)
        {
            int index = Random.Range(0, directories.Count - 1);
            Directory dir = directories[index];
            directories.RemoveAt(index);
            Destroy(dir.gameObject);
        }

        if (!recusiveClean)
            return;

        depth++;
        foreach (Directory dir in directories)
        {
            CleanDepth(dir.transform, maxDepth, maxDir, randomNumber, recusiveClean, depth);
        }
    }

    private void ClearDirectory(List<Directory> directories)
    {
        foreach (Directory dir in directories)
        {
            Destroy(dir.gameObject);
        }
    }
    private List<Directory> GetFirstDepthChildren(Transform root)
    {
        List<Directory> children = new List<Directory>();
        foreach (Directory directory in root.GetComponentsInChildren<Directory>())
        {
          
            if (directory.transform.parent == root)
                children.Add(directory);        
        }

        return children;
    }


}
